using Microsoft.EntityFrameworkCore;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Categories.ValueObjects;
using Shopizy.Domain.Products;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.Infrastructure.Common.Persistence;
using Shopizy.Infrastructure.Common.Specifications;
using Shopizy.Infrastructure.Products.Specifications;

namespace shopizy.Infrastructure.Products.Persistence;

/// <summary>
/// Repository for managing product data persistence.
/// </summary>
public class ProductRepository(AppDbContext dbContext) : IProductRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    /// <summary>
    /// Retrieves a paginated list of products based on search criteria.
    /// </summary>
    /// <param name="name">Optional product name filter.</param>
    /// <param name="categoryIds">Optional list of category IDs to filter by.</param>
    /// <param name="averageRating">Optional minimum average rating filter.</param>
    /// <param name="pageNumber">The page number.</param>
    /// <param name="pageSize">The page size.</param>
    /// <returns>A list of products matching the criteria.</returns>
    public Task<List<Product>> GetProductsAsync(
        string? name,
        IList<CategoryId>? categoryIds,
        decimal? averageRating,
        int pageNumber,
        int pageSize
    )
    {
        return ApplySpec(new ProductsByCriteriaSpec(name, categoryIds, averageRating))
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
    public Task<List<Product>> GetProductsByIdsAsync(IList<ProductId> ids)
    {
        return ApplySpec(new ProductsByIdsSpec(ids)).ToListAsync();
    }

    /// <summary>
    /// Checks if a product exists by its identifier.
    /// </summary>
    /// <param name="id">The product identifier.</param>
    /// <returns>True if the product exists; otherwise, false.</returns>
    public Task<bool> IsProductExistAsync(ProductId id)
    {
        return _dbContext.Products.AnyAsync(p => p.Id == id);
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
    /// Commits all pending changes to the database.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The number of state entries written to the database.</returns>
    public Task<int> CommitAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
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
