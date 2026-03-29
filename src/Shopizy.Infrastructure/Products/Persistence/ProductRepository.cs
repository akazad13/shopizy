using Microsoft.EntityFrameworkCore;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Categories.ValueObjects;
using Shopizy.Domain.Products;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.Infrastructure.Common.Persistence;

namespace Shopizy.Infrastructure.Products.Persistence;

/// <summary>
/// Repository for managing product data persistence.
/// </summary>
public class ProductRepository(AppDbContext dbContext) : IProductRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    /// <summary>
    /// Retrieves a paginated list of products based on search criteria.
    /// </summary>
    public async Task<IReadOnlyList<Product>?> GetProductsAsync(
        IReadOnlyList<ProductId>? productIds,
        string? name,
        IReadOnlyList<CategoryId>? categoryIds,
        decimal? averageRating,
        decimal? minPrice,
        decimal? maxPrice,
        bool? inStockOnly,
        string? sortBy,
        int pageNumber,
        int pageSize
    )
    {
        var query = BuildProductCriteriaQuery(productIds, name, categoryIds, averageRating, minPrice, maxPrice, inStockOnly);

        query = sortBy switch
        {
            "price_asc" => query.OrderBy(p => p.UnitPrice.Amount),
            "price_desc" => query.OrderByDescending(p => p.UnitPrice.Amount),
            "newest" => query.OrderByDescending(p => p.CreatedOn),
            "best_rated" => query.OrderByDescending(p => p.AverageRating.Value),
            "most_reviewed" => query.OrderByDescending(p => p.AverageRating.NumRatings),
            _ => query.OrderByDescending(p => p.CreatedOn)
        };

        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves a product by its unique identifier, including reviews.
    /// </summary>
    public Task<Product?> GetProductByIdAsync(ProductId id)
    {
        return _dbContext
            .Products.Include(p => p.ProductReviews)
            .ThenInclude(p => p.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public Task<Product?> GetProductByIdForUpdateAsync(ProductId id)
    {
        return _dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
    }

    /// <summary>
    /// Retrieves multiple products by their identifiers.
    /// </summary>
    public async Task<IReadOnlyList<Product>> GetProductsByIdsAsync(IReadOnlyList<ProductId> ids)
    {
        return await _dbContext.Products
            .Where(p => ids.Contains(p.Id))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Product>> GetProductsByIdsForUpdateAsync(IReadOnlyList<ProductId> ids)
    {
        return await _dbContext.Products
            .Where(p => ids.Contains(p.Id))
            .ToListAsync();
    }

    public Task<bool> IsProductExistAsync(ProductId id)
    {
        return _dbContext.Products.AnyAsync(p => p.Id == id);
    }

    /// <summary>
    /// Retrieves a list of all distinct product brands.
    /// </summary>
    public async Task<IReadOnlyList<string>> GetBrandsAsync()
    {
        return await _dbContext.Products
            .AsNoTracking()
            .Select(p => p.Brand)
            .Distinct()
            .ToListAsync();
    }

    public Task<int> CountProductsAsync(
        IReadOnlyList<ProductId>? productIds,
        string? name,
        IReadOnlyList<CategoryId>? categoryIds,
        decimal? averageRating,
        decimal? minPrice,
        decimal? maxPrice,
        bool? inStockOnly
    )
    {
        return BuildProductCriteriaQuery(productIds, name, categoryIds, averageRating, minPrice, maxPrice, inStockOnly)
            .CountAsync();
    }

    public async Task<(IReadOnlyList<Product> Products, int TotalCount)> GetProductsWithCountAsync(
        IReadOnlyList<ProductId>? productIds,
        string? name,
        IReadOnlyList<CategoryId>? categoryIds,
        decimal? averageRating,
        decimal? minPrice,
        decimal? maxPrice,
        bool? inStockOnly,
        string? sortBy,
        int pageNumber,
        int pageSize
    )
    {
        var query = BuildProductCriteriaQuery(productIds, name, categoryIds, averageRating, minPrice, maxPrice, inStockOnly);
        var totalCount = await query.CountAsync();

        var sortedQuery = sortBy switch
        {
            "price_asc" => query.OrderBy(p => p.UnitPrice.Amount),
            "price_desc" => query.OrderByDescending(p => p.UnitPrice.Amount),
            "newest" => query.OrderByDescending(p => p.CreatedOn),
            "best_rated" => query.OrderByDescending(p => p.AverageRating.Value),
            "most_reviewed" => query.OrderByDescending(p => p.AverageRating.NumRatings),
            _ => query.OrderByDescending(p => p.CreatedOn)
        };

        var products = await sortedQuery
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();

        return (products, totalCount);
    }

    /// <summary>
    /// Gets the total count of products in the database.
    /// </summary>
    public Task<int> GetTotalProductCountAsync()
    {
        return _dbContext.Products.CountAsync();
    }

    public async Task<IReadOnlyList<Product>> GetLowStockProductsAsync(int threshold)
    {
        return await _dbContext.Products
            .Where(p => p.StockQuantity <= threshold)
            .OrderBy(p => p.StockQuantity)
            .Take(10)
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// Adds a new product to the database.
    /// </summary>
    public async Task AddAsync(Product product)
    {
        await _dbContext.Products.AddAsync(product);
    }

    /// <summary>
    /// Updates an existing product in the database.
    /// </summary>
    public void Update(Product product)
    {
        _dbContext.Update(product);
    }

    /// <summary>
    /// Removes a product from the database.
    /// </summary>
    public void Remove(Product product)
    {
        _dbContext.Remove(product);
    }

    /// <summary>
    /// Removes multiple products from the database.
    /// </summary>
    public void RemoveRange(IList<Product> products)
    {
        _dbContext.RemoveRange(products);
    }

    private IQueryable<Product> BuildProductCriteriaQuery(
        IReadOnlyList<ProductId>? productIds,
        string? name,
        IReadOnlyList<CategoryId>? categoryIds,
        decimal? averageRating,
        decimal? minPrice,
        decimal? maxPrice,
        bool? inStockOnly
    )
    {
        var query = _dbContext.Products.Include(p => p.ProductImages).AsQueryable();

        if (productIds is not null) query = query.Where(p => productIds.Contains(p.Id));
        if (name is not null) query = query.Where(p => p.Name.Contains(name));
        if (categoryIds is not null) query = query.Where(p => categoryIds.Contains(p.CategoryId));
        if (averageRating is not null) query = query.Where(p => p.AverageRating.Value >= averageRating);
        if (minPrice is not null) query = query.Where(p => p.UnitPrice.Amount >= minPrice);
        if (maxPrice is not null) query = query.Where(p => p.UnitPrice.Amount <= maxPrice);
        if (inStockOnly is true) query = query.Where(p => p.StockQuantity > 0);

        return query;
    }
}
