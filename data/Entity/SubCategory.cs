using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.domain.entity;

// ==========================================================
// SubCategory Entity Class
// ----------------------------------------------------------
// This class represents sub-categories of products within a store.
// Each record defines a specific product category and links to
// its parent category and store.
//
// Inherits from GeneralShredInfo which provides:
// - Id (Guid primary key)
// - CreatedAt (creation timestamp)
// - UpdatedAt (modification timestamp) - Full audit trail
//
// Each object created from this class represents one row
// inside the SubCategories table.
//
// Example:
// Id                   = "f47ac10b-58cc-4372-a567-0e02b2c3d479"
// Name                 = "Smartphones"
// StoreId              = "a8d7f9e2-5c3b-4a6d-8f1e-7b2c4d9e8f3a"
// CategoryId           = "b5c9f8d3-2a7e-4b1d-9f6c-8e4d3c2a1b5f"
// CreatedAt            = 2026-08-27 14:30:00.000
// UpdatedAt            = null
// ==========================================================
public class SubCategory : GeneralShredInfo
{
    // ==========================================================
    // Basic Information
    // ==========================================================

    // ------------------------------------------------------
    // SubCategory Name
    // ------------------------------------------------------
    // The display name of the sub-category.
    // Required field - cannot be null or empty.
    //
    // Examples:
    // "Smartphones"
    // "Laptops"
    // "Men's Clothing"
    // "Women's Shoes"
    // "Kitchen Appliances"
    // "Home Decor"
    // "Books"
    // "Toys"
    // "Automotive Parts"
    // "Pet Supplies"
    // "Office Furniture"
    // "Garden Tools"
    //
    // Use Cases:
    // - Product categorization
    // - Navigation and browsing
    // - Search and filtering
    // - Reports and analytics
    // - Inventory management
    // ------------------------------------------------------
    public string Name { get; set; }

    // ==========================================================
    // Foreign Keys
    // ==========================================================

    // ------------------------------------------------------
    // Store Identifier
    // ------------------------------------------------------
    // Unique identifier (GUID) of the store that owns this
    // sub-category.
    //
    // Required foreign key - cannot be null.
    // References the Store entity.
    //
    // Example:
    // "a8d7f9e2-5c3b-4a6d-8f1e-7b2c4d9e8f3a" (store ID)
    //
    // Use Cases:
    // - Store-specific categories
    // - Multi-store product organization
    // - Store catalog management
    // - Access control
    // ------------------------------------------------------
    public Guid StoreId { get; set; }

    // ------------------------------------------------------
    // Category Identifier
    // ------------------------------------------------------
    // Unique identifier (GUID) of the parent category.
    // Links this sub-category to its parent category.
    //
    // Required foreign key - cannot be null.
    // References the Category entity.
    //
    // Example:
    // "b5c9f8d3-2a7e-4b1d-9f6c-8e4d3c2a1b5f" (category ID)
    //
    // Use Cases:
    // - Category hierarchy
    // - Breadcrumb navigation
    // - Grouping sub-categories
    // - Reports by category
    // ------------------------------------------------------
    public Guid CategoryId { get; set; }

    // ==========================================================
    // Navigation Properties
    // ==========================================================

    // ------------------------------------------------------
    // Store Relationship
    // ------------------------------------------------------
    // Navigation property to the store that owns this
    // sub-category.
    //
    // Nullable - may be null if not loaded.
    // Default initialized to null.
    //
    // Example:
    // SubCategory.Store.Name
    // SubCategory.Store.WallpaperImage
    // SubCategory.Store.IsBlocked
    //
    // Usage:
    // var storeName = subCategory.Store?.Name;
    // ------------------------------------------------------
    public virtual Store? Store { get; set; } = null;

    // ------------------------------------------------------
    // Category Relationship
    // ------------------------------------------------------
    // Navigation property to the parent category.
    // Provides access to category details.
    //
    // Nullable - may be null if not loaded.
    // Default initialized to null.
    //
    // Example:
    // SubCategory.Category.Name
    // SubCategory.Category.Image
    // SubCategory.Category.IsBlocked
    //
    // Usage:
    // var categoryName = subCategory.Category?.Name;
    // ------------------------------------------------------
    public virtual Category? Category { get; set; } = null;

    // ------------------------------------------------------
    // Products Relationship
    // ------------------------------------------------------
    // Navigation property for all products in this sub-category.
    // One sub-category can have many products.
    // Initialized as empty collection to prevent null reference.
    //
    // Example:
    // SubCategory.Products
    //   ├─ iPhone 15 Pro
    //   ├─ Samsung Galaxy S24
    //   ├─ Google Pixel 8
    //   └─ OnePlus 12
    //
    // Usage:
    // var productCount = subCategory.Products.Count;
    // var activeProducts = subCategory.Products
    //     .Where(p => p.IsActive && !p.IsDeleted)
    //     .ToList();
    // ------------------------------------------------------
    public virtual ICollection<Product> Products { get; set; }
        = new List<Product>();
}