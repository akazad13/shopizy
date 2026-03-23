using Microsoft.EntityFrameworkCore;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Categories.ValueObjects;
using Shopizy.Domain.Products;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.Infrastructure.Common.Persistence;
using Shopizy.Infrastructure.Common.Specifications;
using Shopizy.Infrastructure.Products.Specifications;

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
    /// <param name="productIds">Optional product Ids filter</param>
    /// <param name="name">Optional product name filter.</param>
    /// <param name="categoryIds">Optional list of category IDs to filter by.</param>
    /// <param name="averageRating">Optional minimum average rating filter.</param>
    /// <param name="minPrice">Optional minimum price filter.</param>
    /// <param name="maxPrice">Optional maximum price filter.</param>
    /// <param name="inStockOnly">Optional filter to only in-stock products.</param>
    /// <param name="sortBy">Optional sort order.</param>
    /// <param name="pageNumber">The page number.</param>
    /// <param name="pageSize">The page size.</param>
    /// <returns>A list of products matching the criteria.</returns>
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
        var query = ApplySpec(new ProductsByCriteriaSpec(productIds, name, categoryIds, averageRating, minPrice, maxPrice, inStockOnly));

        query = sortBy switch
        {
            "price_asc" => query.OrderBy(p => p.UnitPrice.Amount),
            "price_desc" => query.OrderByDescending(p => p.UnitPrice.Amount),
            "newest" => query.OrderByDescending(p => p.CreatedOn),
            "best_rated" => query.OrderByDescending(p => p.AverageRating.Value),
            "most_reviewed" => query.OrderByDescending(p => p.AverageRating.NumRatings),
            _ => query
        };

        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves a product by its unique identifier, including reviews.
    /// </summary>
    /// <param name="id">The product identifier.</param>
    /// <returns>The product if found; otherwise, null.</returns>
    public Task<Product?> GetProductByIdAsync(ProductId id)
    {
        return _dbContext
            .Products.Include(p => p.ProductReviews)
            .ThenInclude(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    /// <summary>
    /// Retrieves multiple products by their identifiers.
    /// </summary>
    /// <param name="ids">The list of product identifiers.</param>
    /// <returns>A list of products.</returns>
    public async Task<IReadOnlyList<Product>> GetProductsByIdsAsync(IReadOnlyList<ProductId> ids)
    {
        return await _dbContext.Products
            .Where(p => ids.Contains(p.Id))
            .AsNoTracking()
            .ToListAsync();
    }

    public Task<bool> IsProductExistAsync(ProductId id)
    {
        return _dbContext.Products.AnyAsync(p => p.Id == id);
    }

    /// <summary>
    /// Retrieves a list of all distinct product brands.
    /// </summary>
    /// <returns>A list of brand names.</returns>
    public async Task<IReadOnlyList<string>> GetBrandsAsync()
    {
        return await _dbContext.Products
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
        return ApplySpec(new ProductsByCriteriaSpec(productIds, name, categoryIds, averageRating, minPrice, maxPrice, inStockOnly))
            .CountAsync();
    }

    /// <summary>
    /// Gets the total count of products in the database.
    /// </summary>
    /// <returns>The total product count.</returns>
    public Task<int> GetTotalProductCountAsync()
    {
        return _dbContext.Products.CountAsync();
    }

    public async Task<IReadOnlyList<Product>> GetLowStockProductsAsync(int threshold)
    {
        return await _dbContext.Products
            .Where(p => p.StockQuantity <= threshold)
            .Take(10)
            .ToListAsync();
    }

    /// <summary>
    /// Adds a new product to the database.
    /// </summary>
    /// <param name="product">The product to add.</param>
    public async Task AddAsync(Product product)
    {
        await _dbContext.Products.AddAsync(product);
    }

    /// <summary>
    /// Updates an existing product in the database.
    /// </summary>
    /// <param name="product">The product to update.</param>
    public void Update(Product product)
    {
        _dbContext.Update(product);
    }

    /// <summary>
    /// Removes a product from the database.
    /// </summary>
    /// <param name="product">The product to remove.</param>
    public void Remove(Product product)
    {
        _dbContext.Remove(product);
    }

    /// <summary>
    /// Removes multiple products from the database.
    /// </summary>
    /// <param name="products">The list of products to remove.</param>
    public void RemoveRange(IList<Product> products)
    {
        _dbContext.RemoveRange(products);
    }

    /// <summary>
    /// Applies a specification to the product query.
    /// </summary>
    /// <param name="spec">The specification to apply.</param>
    /// <returns>A queryable of products.</returns>
    private IQueryable<Product> ApplySpec(Specification<Product> spec)
    {
        return SpecificationEvaluator.GetQuery(_dbContext.Products, spec);
    }
}
