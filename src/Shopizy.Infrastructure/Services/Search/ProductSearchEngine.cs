using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Domain.Brands.ValueObjects;
using Shopizy.Domain.Categories.ValueObjects;
using Shopizy.Domain.Products;
using Shopizy.Infrastructure.Common.Persistence;

namespace Shopizy.Infrastructure.Services.Search;

/// <summary>
/// Dedicated search engine providing tokenization, synonym resolution, Levenshtein fuzzy matching, and multi-dimensional faceted filtering.
/// </summary>
public class ProductSearchEngine(AppDbContext dbContext, ILogger<ProductSearchEngine> logger)
    : IProductSearchEngine
{
    private static readonly Action<ILogger, string?, int, Exception?> LogSearchExecuted =
        LoggerMessage.Define<string?, int>(
            LogLevel.Information,
            new EventId(1, nameof(SearchProductsAsync)),
            "Executed faceted search for term: '{SearchTerm}', found {ResultCount} items"
        );

    private static readonly Dictionary<string, HashSet<string>> Synonyms = new(
        StringComparer.OrdinalIgnoreCase
    )
    {
        ["phone"] = ["smartphone", "mobile", "cellphone", "iphone", "android"],
        ["smartphone"] = ["phone", "mobile", "cellphone", "iphone", "android"],
        ["mobile"] = ["phone", "smartphone", "cellphone"],
        ["shoe"] = ["shoes", "sneaker", "sneakers", "footwear", "boot", "boots"],
        ["shoes"] = ["shoe", "sneaker", "sneakers", "footwear", "boot", "boots"],
        ["sneaker"] = ["shoe", "shoes", "sneakers", "footwear"],
        ["sneakers"] = ["shoe", "shoes", "sneaker", "footwear"],
        ["shirt"] = ["tshirt", "t-shirt", "tee", "top", "apparel"],
        ["tshirt"] = ["shirt", "t-shirt", "tee", "top"],
        ["tee"] = ["shirt", "tshirt", "t-shirt", "top"],
        ["laptop"] = ["notebook", "computer", "macbook", "pc"],
        ["watch"] = ["smartwatch", "timepiece", "wrist watch"],
        ["smartwatch"] = ["watch", "timepiece", "apple watch", "galaxy watch"],
        ["hoodie"] = ["sweatshirt", "jacket", "sweater", "pullover"],
        ["bag"] = ["backpack", "handbag", "purse", "tote"],
    };

    private readonly AppDbContext _dbContext = dbContext;
    private readonly ILogger<ProductSearchEngine> _logger = logger;

    public async Task<ProductSearchResultDto> SearchProductsAsync(
        ProductSearchQueryDto query,
        CancellationToken cancellationToken = default
    )
    {
        var categoryMap = await _dbContext
            .Categories.AsNoTracking()
            .ToDictionaryAsync(c => c.Id, c => c.Name, cancellationToken);

        var brandMap = await _dbContext
            .Brands.AsNoTracking()
            .ToDictionaryAsync(b => b.Id, b => b.Name, cancellationToken);

        var allProducts = await _dbContext
            .Products.Include(p => p.ProductImages)
            .Include(p => p.ProductReviews)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        // 1. Expand search tokens and synonyms
        var searchTokens = ExtractSearchTokens(query.SearchTerm);
        var expandedTerms = ExpandSynonyms(searchTokens);

        // 2. Score and filter candidates
        var candidates = new List<(Product Product, int Score)>();
        foreach (var product in allProducts)
        {
            if (
                query.CategoryIds is { Count: > 0 }
                && !query.CategoryIds.Contains(product.CategoryId.Value)
            )
            {
                continue;
            }

            if (
                query.BrandIds is { Count: > 0 }
                && (product.BrandId is null || !query.BrandIds.Contains(product.BrandId.Value))
            )
            {
                continue;
            }

            if (query.MinPrice.HasValue && product.UnitPrice.Amount < query.MinPrice.Value)
            {
                continue;
            }

            if (query.MaxPrice.HasValue && product.UnitPrice.Amount > query.MaxPrice.Value)
            {
                continue;
            }

            if (query.InStockOnly == true && product.StockQuantity <= 0)
            {
                continue;
            }

            if (query.MinRating.HasValue && product.AverageRating.Value < query.MinRating.Value)
            {
                continue;
            }

            var categoryName = categoryMap.GetValueOrDefault(product.CategoryId);
            var brandName = product.BrandId is not null
                ? brandMap.GetValueOrDefault(product.BrandId)
                : null;

            var score = CalculateProductScore(
                product,
                categoryName,
                brandName,
                searchTokens,
                expandedTerms
            );
            if (score > 0 || searchTokens.Count == 0)
            {
                candidates.Add((product, score));
            }
        }

        // 3. Compute dynamic facet buckets across matching candidates
        var facets = ComputeFacets(candidates.Select(c => c.Product), categoryMap, brandMap);

        // 4. Compute suggested keywords if search term was provided
        var suggestions =
            searchTokens.Count > 0 ? GenerateKeywordSuggestions(searchTokens, allProducts) : [];

        // 5. Apply sorting
        IEnumerable<Product> sortedProducts = query.SortBy switch
        {
            "price_asc" => candidates
                .OrderBy(c => c.Product.UnitPrice.Amount)
                .Select(c => c.Product),
            "price_desc" => candidates
                .OrderByDescending(c => c.Product.UnitPrice.Amount)
                .Select(c => c.Product),
            "rating_desc" => candidates
                .OrderByDescending(c => c.Product.AverageRating.Value)
                .Select(c => c.Product),
            "newest" => candidates
                .OrderByDescending(c => c.Product.CreatedOn)
                .Select(c => c.Product),
            _ => candidates
                .OrderByDescending(c => c.Score)
                .ThenByDescending(c => c.Product.AverageRating.Value)
                .Select(c => c.Product),
        };

        var matchedList = sortedProducts.ToList();
        var totalCount = matchedList.Count;
        var pageNumber = Math.Max(1, query.PageNumber);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        var pagedItems = matchedList
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductSearchResultItemDto(
                Id: p.Id.Value,
                Name: p.Name,
                Description: p.Description,
                Price: p.UnitPrice.Amount,
                Currency: p.UnitPrice.Currency.ToString(),
                CategoryId: p.CategoryId.Value,
                CategoryName: categoryMap.GetValueOrDefault(p.CategoryId),
                BrandId: p.BrandId?.Value,
                BrandName: p.BrandId is not null ? brandMap.GetValueOrDefault(p.BrandId) : null,
                StockQuantity: p.StockQuantity,
                AverageRating: p.AverageRating.Value,
                TotalReviews: p.ProductReviews?.Count ?? 0,
                ImageUrls: p.ProductImages?.Select(img => img.ImageUrl).ToList() ?? [],
                Tags: !string.IsNullOrWhiteSpace(p.Tags)
                    ? p
                        .Tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(t => t.Trim())
                        .ToList()
                    : []
            ))
            .ToList();

        LogSearchExecuted(_logger, query.SearchTerm, totalCount, null);

        return new ProductSearchResultDto(
            Items: pagedItems.AsReadOnly(),
            TotalCount: totalCount,
            PageNumber: pageNumber,
            PageSize: pageSize,
            TotalPages: totalPages,
            Facets: facets,
            SuggestedKeywords: suggestions
        );
    }

    private static List<string> ExtractSearchTokens(string? searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return [];
        }

        return searchTerm
            .Split([' ', ',', '-', '/', '.'], StringSplitOptions.RemoveEmptyEntries)
            .Select(t => t.Trim().ToLowerInvariant())
            .Where(t => t.Length > 1)
            .Distinct()
            .ToList();
    }

    private static HashSet<string> ExpandSynonyms(List<string> tokens)
    {
        var expanded = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var token in tokens)
        {
            expanded.Add(token);
            if (Synonyms.TryGetValue(token, out var synonyms))
            {
                foreach (var syn in synonyms)
                {
                    expanded.Add(syn);
                }
            }
        }
        return expanded;
    }

    private static int CalculateProductScore(
        Product product,
        string? categoryName,
        string? brandName,
        List<string> searchTokens,
        HashSet<string> expandedTerms
    )
    {
        if (searchTokens.Count == 0)
        {
            return 1;
        }

        var score = 0;
        var nameLower = product.Name.ToLowerInvariant();
        var descLower = product.Description.ToLowerInvariant();
        var catLower = categoryName?.ToLowerInvariant() ?? "";
        var brandLower = brandName?.ToLowerInvariant() ?? "";
        var tagStrings = !string.IsNullOrWhiteSpace(product.Tags)
            ? product
                .Tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim().ToLowerInvariant())
                .ToList()
            : [];

        foreach (var term in expandedTerms)
        {
            // Exact full phrase / token matches
            if (nameLower.Contains(term, StringComparison.OrdinalIgnoreCase))
            {
                score += 50;
            }
            if (tagStrings.Any(t => t.Contains(term, StringComparison.OrdinalIgnoreCase)))
            {
                score += 30;
            }
            if (
                catLower.Contains(term, StringComparison.OrdinalIgnoreCase)
                || brandLower.Contains(term, StringComparison.OrdinalIgnoreCase)
            )
            {
                score += 20;
            }
            if (descLower.Contains(term, StringComparison.OrdinalIgnoreCase))
            {
                score += 10;
            }

            // Fuzzy Levenshtein matching on words
            var nameWords = nameLower.Split(' ');
            foreach (var word in nameWords)
            {
                if (word.Length >= 4 && term.Length >= 4)
                {
                    var dist = LevenshteinDistance(word, term);
                    if (dist == 1)
                        score += 25;
                    else if (dist == 2)
                        score += 10;
                }
            }
        }

        return score;
    }

    private static IReadOnlyList<SearchFacetDto> ComputeFacets(
        IEnumerable<Product> products,
        Dictionary<CategoryId, string> categoryMap,
        Dictionary<BrandId, string> brandMap
    )
    {
        var productList = products.ToList();
        var facets = new List<SearchFacetDto>();

        // 1. Categories
        var categoryFacets = productList
            .GroupBy(p => p.CategoryId)
            .Select(g => new FacetValueDto(
                g.Key.Value.ToString(),
                categoryMap.GetValueOrDefault(g.Key) ?? "Other",
                g.Count()
            ))
            .OrderByDescending(f => f.Count)
            .ToList();
        facets.Add(new SearchFacetDto("Category", categoryFacets.AsReadOnly()));

        // 2. Brands
        var brandFacets = productList
            .Where(p => p.BrandId is not null)
            .GroupBy(p => p.BrandId!)
            .Select(g => new FacetValueDto(
                g.Key.Value.ToString(),
                brandMap.GetValueOrDefault(g.Key) ?? "Other",
                g.Count()
            ))
            .OrderByDescending(f => f.Count)
            .ToList();
        facets.Add(new SearchFacetDto("Brand", brandFacets.AsReadOnly()));

        // 3. Price Ranges
        var priceRanges = new List<FacetValueDto>
        {
            new("under_25", "Under $25", productList.Count(p => p.UnitPrice.Amount < 25)),
            new(
                "25_to_50",
                "$25 to $50",
                productList.Count(p => p.UnitPrice.Amount >= 25 && p.UnitPrice.Amount < 50)
            ),
            new(
                "50_to_100",
                "$50 to $100",
                productList.Count(p => p.UnitPrice.Amount >= 50 && p.UnitPrice.Amount < 100)
            ),
            new(
                "100_to_200",
                "$100 to $200",
                productList.Count(p => p.UnitPrice.Amount >= 100 && p.UnitPrice.Amount < 200)
            ),
            new("200_plus", "$200 & Above", productList.Count(p => p.UnitPrice.Amount >= 200)),
        };
        facets.Add(
            new SearchFacetDto("Price", priceRanges.Where(r => r.Count > 0).ToList().AsReadOnly())
        );

        // 4. Rating Tiers
        var ratingTiers = new List<FacetValueDto>
        {
            new("4_star", "4★ & above", productList.Count(p => p.AverageRating.Value >= 4.0m)),
            new("3_star", "3★ & above", productList.Count(p => p.AverageRating.Value >= 3.0m)),
            new("2_star", "2★ & above", productList.Count(p => p.AverageRating.Value >= 2.0m)),
            new("1_star", "1★ & above", productList.Count(p => p.AverageRating.Value >= 1.0m)),
        };
        facets.Add(
            new SearchFacetDto("Rating", ratingTiers.Where(r => r.Count > 0).ToList().AsReadOnly())
        );

        // 5. In Stock
        var stockFacets = new List<FacetValueDto>
        {
            new("in_stock", "In Stock", productList.Count(p => p.StockQuantity > 0)),
            new("out_of_stock", "Out of Stock", productList.Count(p => p.StockQuantity <= 0)),
        };
        facets.Add(
            new SearchFacetDto(
                "Availability",
                stockFacets.Where(s => s.Count > 0).ToList().AsReadOnly()
            )
        );

        return facets.AsReadOnly();
    }

    private static IReadOnlyList<string> GenerateKeywordSuggestions(
        List<string> searchTokens,
        List<Product> allProducts
    )
    {
        var dictionary = allProducts
            .SelectMany(p => p.Name.Split([' ', ',', '-']))
            .Select(w => w.Trim().ToLowerInvariant())
            .Where(w => w.Length >= 4)
            .Distinct()
            .ToList();

        var suggestions = new List<string>();
        foreach (var token in searchTokens)
        {
            foreach (var word in dictionary)
            {
                if (
                    !string.Equals(token, word, StringComparison.OrdinalIgnoreCase)
                    && LevenshteinDistance(token, word) == 1
                )
                {
                    suggestions.Add(word);
                }
            }
        }

        return suggestions.Distinct().Take(5).ToList().AsReadOnly();
    }

    private static int LevenshteinDistance(string s, string t)
    {
        if (string.IsNullOrEmpty(s))
            return t?.Length ?? 0;
        if (string.IsNullOrEmpty(t))
            return s.Length;

        var d = new int[s.Length + 1, t.Length + 1];

        for (var i = 0; i <= s.Length; i++)
            d[i, 0] = i;
        for (var j = 0; j <= t.Length; j++)
            d[0, j] = j;

        for (var i = 1; i <= s.Length; i++)
        {
            for (var j = 1; j <= t.Length; j++)
            {
                var cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                d[i, j] = Math.Min(
                    Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                    d[i - 1, j - 1] + cost
                );
            }
        }

        return d[s.Length, t.Length];
    }
}
