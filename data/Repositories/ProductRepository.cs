using api.application;
using api.domain.entity;
using data.Interface;
using Microsoft.EntityFrameworkCore;

namespace data.Repositories;

/// <summary>
/// Provides data-access operations for <see cref="Product"/> entities.
/// </summary>
/// <remarks>
/// This repository uses Entity Framework Core through <see cref="AppDbContext"/>
/// and implements <see cref="IProductRepository"/>.
/// 
/// Read operations generally use <see cref="EntityFrameworkQueryableExtensions.AsNoTracking{TEntity}(IQueryable{TEntity})"/>
/// to avoid unnecessary change tracking and <see cref="EntityFrameworkQueryableExtensions.AsSplitQuery{TEntity}(IQueryable{TEntity})"/>
/// when loading multiple related collections.
/// </remarks>
public class ProductRepository(
    AppDbContext context
) : IProductRepository
{
    /// <summary>
    /// Retrieves a paginated collection of products.
    /// </summary>
    /// <param name="page">The one-based page number.</param>
    /// <param name="length">The maximum number of products to return.</param>
    /// <returns>A collection containing the products for the requested page.</returns>
    public async Task<ICollection<Product>> GetAllAsync(int page, int length)
    {
        return await context.Products
            .AsNoTracking()
            .Include(pro => pro.SubCategory)
            .Include(pro => pro.ProductImages)
            .Include(pro => pro.ProductVariants)
            .AsSplitQuery()
            .Skip((page - 1) * length)
            .Take(length)
            .OrderDescending()
            .ToListAsync();
    }

    /// <summary>
    /// Adds a new product to the current DbContext.
    /// </summary>
    /// <param name="entity">The product entity to add.</param>
    /// <remarks>
    /// The changes are not persisted to the database until the DbContext
    /// is saved, typically through <c>SaveChangesAsync()</c>.
    /// </remarks>
    public void Add(Product entity)
    {
        context.Products.Add(entity);
    }

    /// <summary>
    /// Updates the persisted product properties using the supplied entity.
    /// </summary>
    /// <param name="entity">The product containing the updated values.</param>
    public void Update(Product entity)
    {
        var product = new Product()
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            SubcategoryId = entity.SubcategoryId,
            Price = entity.Price,
            UpdatedAt = entity.UpdatedAt,
            Thumbnail = entity.Thumbnail,
            Symbol = entity.Symbol,
            StoreId = entity.StoreId
        };

        context.Products.Update(product);
    }

    /// <summary>
    /// Deletes a product identified by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the product.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when a product with the specified ID does not exist.
    /// </exception>
    public void Delete(Guid id)
    {
        var product = context.Products.Find(id);

        if (product == null)
            throw new ArgumentNullException();

        context.Products.Remove(product);
    }

    /// <summary>
    /// Deletes a collection of products from the current DbContext.
    /// </summary>
    /// <param name="products">The products to remove.</param>
    public void Delete(ICollection<Product> products)
    {
        context.Products.RemoveRange(products);
    }

    /// <summary>
    /// Retrieves a product by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the product.</param>
    /// <returns>
    /// The matching product with its store, subcategory, images, and variants,
    /// or <c>null</c> if no product is found.
    /// </returns>
    public async Task<Product?> GetProduct(Guid id)
    {
        return await context.Products
            .AsNoTracking()
            .Include(pro => pro.Store)
            .Include(pro => pro.SubCategory)
            .Include(pro => pro.ProductImages)
            .Include(pro => pro.ProductVariants)
            .AsSplitQuery()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    /// <summary>
    /// Retrieves a product by its ID and associated store ID.
    /// </summary>
    /// <param name="id">The unique identifier of the product.</param>
    /// <param name="storeId">The unique identifier of the store.</param>
    /// <returns>
    /// The matching product with its related entities, or <c>null</c> if no match exists.
    /// </returns>
    public async Task<Product?> GetProduct(Guid id, Guid storeId)
    {
        return await context.Products
            .AsNoTracking()
            .Include(pro => pro.Store)
            .Include(pro => pro.SubCategory)
            .Include(pro => pro.ProductImages)
            .Include(pro => pro.ProductVariants)
            .AsSplitQuery()
            .FirstOrDefaultAsync(p => p.Id == id && p.StoreId == storeId);
    }

    /// <summary>
    /// Gets the total number of products.
    /// </summary>
    /// <returns>The total number of products in the database.</returns>
    public Task<int> GetProduct()
    {
        return context.Products.CountAsync();
    }

    /// <summary>
    /// Gets the total number of products.
    /// </summary>
    /// <returns>The total product count, or <c>null</c> if applicable.</returns>
    public async Task<int?> GetProductPages()
    {
        return await context.Products.CountAsync();
    }

    /// <summary>
    /// Retrieves a product by its ID and verifies that the product belongs
    /// to a store owned by the specified user.
    /// </summary>
    /// <param name="id">The unique identifier of the product.</param>
    /// <param name="userId">The unique identifier of the store owner.</param>
    /// <returns>
    /// The matching product with its related entities, or <c>null</c> if no match exists.
    /// </returns>
    public async Task<Product?> GetProductByUser(Guid id, Guid userId)
    {
        return await context.Products
            .AsNoTracking()
            .Include(pro => pro.Store)
            .Include(pro => pro.SubCategory)
            .Include(pro => pro.ProductImages)
            .Include(pro => pro.ProductVariants)
            .AsSplitQuery()
            .FirstOrDefaultAsync(p => p.Id == id && p.Store.UserId == userId);
    }

    /// <summary>
    /// Retrieves products belonging to a specific store and subcategory using pagination.
    /// </summary>
    /// <param name="storeId">The unique identifier of the store.</param>
    /// <param name="subCategoryId">The unique identifier of the subcategory.</param>
    /// <param name="pageNum">The one-based page number.</param>
    /// <param name="pageSize">The maximum number of products to return.</param>
    /// <returns>A paginated collection of products matching the store and subcategory.</returns>
    public async Task<ICollection<Product>> GetProducts(
        Guid storeId,
        Guid subCategoryId,
        int pageNum,
        int pageSize)
    {
        return await context.Products
            .AsNoTracking()
            .Include(pro => pro.Store)
            .Include(pro => pro.SubCategory)
            .Include(pro => pro.ProductImages)
            .Include(pro => pro.ProductVariants)
            .AsSplitQuery()
            .Where(p => p.StoreId == storeId && p.SubcategoryId == subCategoryId)
            .Skip((pageNum - 1) * pageSize)
            .Take(pageSize)
            .OrderDescending()
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves products belonging to a specific store using pagination.
    /// </summary>
    /// <param name="storeId">The unique identifier of the store.</param>
    /// <param name="pageNum">The one-based page number.</param>
    /// <param name="pageSize">The maximum number of products to return.</param>
    /// <returns>A paginated collection of products belonging to the specified store.</returns>
    public async Task<ICollection<Product>> GetProducts(
        Guid storeId,
        int pageNum,
        int pageSize)
    {
        return await context.Products
            .AsNoTracking()
            .Include(pro => pro.Store)
            .Include(pro => pro.SubCategory)
            .Include(pro => pro.ProductImages)
            .Include(pro => pro.ProductVariants)
            .AsSplitQuery()
            .Where(p => p.StoreId == storeId)
            .Skip((pageNum - 1) * pageSize)
            .Take(pageSize)
            .OrderDescending()
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves a paginated collection of products and loads their variants.
    /// </summary>
    /// <param name="page">The one-based page number.</param>
    /// <param name="length">The maximum number of products to return.</param>
    /// <returns>
    /// A collection of products for the requested page. An empty collection is
    /// returned when an exception occurs.
    /// </returns>
    public async Task<ICollection<Product>> GetProducts(int page, int length)
    {
        try
        {
            var products = await context.Products
                .AsNoTracking()
                .Include(pro => pro.Store)
                .Include(pro => pro.SubCategory)
                .Include(pro => pro.ProductImages)
                .Include(pro => pro.ProductVariants)
                .AsSplitQuery()
                .Skip((page - 1) * length)
                .Take(length)
                .OrderDescending()
                .ToListAsync();

            if (products.Count==0)
                return new List<Product>();

            for (int i = 0; i < products.Count; i++)
            {
                products[i].ProductVariants = await context.ProductVariants
                    .Include(pr => pr.Variant)
                    .Where(p => p.ProductId == products[i].Id)
                    .ToListAsync();
            }

            return products;
        }
        catch (System.Exception ex)
        {
            Console.WriteLine(ex.Message);
            return new List<Product>();
        }
    }

    /// <summary>
    /// Retrieves a specified number of random products.
    /// </summary>
    /// <param name="randomNumber">The maximum number of random products to return.</param>
    /// <returns>A collection containing randomly selected products.</returns>
    public async Task<ICollection<Product>> GetProducts(int randomNumber)
    {
        return await context
            .Products
            .OrderBy(x => Guid.NewGuid())
            .Take(randomNumber)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves products belonging to a specific category using pagination.
    /// </summary>
    /// <param name="categoryId">The unique identifier of the category.</param>
    /// <param name="pageNum">The one-based page number.</param>
    /// <param name="pageSize">The maximum number of products to return.</param>
    /// <returns>
    /// A paginated collection of products whose subcategory belongs to the specified category.
    /// </returns>
    public async Task<ICollection<Product>> GetProductsByCategory(
        Guid categoryId,
        int pageNum,
        int pageSize)
    {
        return await context.Products
            .AsNoTracking()
            .Include(pro => pro.Store)
            .Include(pro => pro.SubCategory)
            .Include(pro => pro.ProductImages)
            .Include(pro => pro.ProductVariants)
            .AsSplitQuery()
            .Where(p => p.SubCategory.CategoryId == categoryId)
            .Skip((pageNum - 1) * pageSize)
            .Take(pageSize)
            .OrderDescending()
            .ToListAsync();
    }

    /// <summary>
    /// Determines whether a product exists with the specified ID.
    /// </summary>
    /// <param name="id">The unique identifier of the product.</param>
    /// <returns>
    /// <c>true</c> if the product exists; otherwise, <c>false</c>.
    /// </returns>
    public async Task<bool> IsExist(Guid id)
    {
        return await context.Products.FindAsync(id) != null;
    }
}
