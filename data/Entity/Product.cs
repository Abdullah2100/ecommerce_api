using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.domain.entity;

// ==========================================================
// Product Entity Class
// ----------------------------------------------------------
// This class represents products available for sale in the system.
// Each record tracks product details including name, description,
// pricing, inventory, and associated media.
//
// Inherits from GeneralShredInfo which provides:
// - Id (Guid primary key)
// - CreatedAt (creation timestamp)
// - UpdatedAt (modification timestamp) - Full audit trail
//
// Each object created from this class represents one row
// inside the Products table.
//
// Example:
// Id                   = "f47ac10b-58cc-4372-a567-0e02b2c3d479"
// Name                 = "Wireless Headphones"
// Description          = "Premium noise-cancelling headphones"
// Thumbnail            = "products/headphones.jpg"
// SubcategoryId        = "b5c9f8d3-2a7e-4b1d-9f6c-8e4d3c2a1b5f"
// StoreId              = "a8d7f9e2-5c3b-4a6d-8f1e-7b2c4d9e8f3a"
// Price                = 9999 (stored in cents = $99.99)
// Quantity             = 50
// Symbol               = "$"
// CreatedAt            = 2026-08-27 14:30:00.000
// UpdatedAt            = null
// ==========================================================
public class Product : GeneralShredInfo
{
    // ==========================================================
    // Basic Information
    // ==========================================================

    // ------------------------------------------------------
    // Product Name
    // ------------------------------------------------------
    // The display name of the product as shown to customers.
    // Required field - cannot be null or empty.
    //
    // Examples:
    // "Wireless Bluetooth Headphones"
    // "Premium Leather Wallet"
    // "Organic Cotton T-Shirt"
    // "Smartphone X Pro"
    // "Gourmet Coffee Beans"
    //
    // Use Cases:
    // - Product display in UI
    // - Search and filtering
    // - Order items
    // - Catalog management
    // - SEO and marketing
    // ------------------------------------------------------
    public string Name { get; set; }

    // ------------------------------------------------------
    // Product Description
    // ------------------------------------------------------
    // Detailed description of the product including features,
    // specifications, and benefits.
    // Required field - cannot be null or empty.
    //
    // Examples:
    // "Experience crystal-clear audio with our premium noise-cancelling
    //  headphones. Features include 40-hour battery life, comfortable
    //  ear cushions, and high-quality sound drivers."
    //
    // Use Cases:
    // - Product detail pages
    // - SEO content
    // - Customer information
    // - Marketing materials
    // ------------------------------------------------------
    public string Description { get; set; }

    // ------------------------------------------------------
    // Product Thumbnail
    // ------------------------------------------------------
    // The primary image path, URL, or base64 representation
    // for the product display.
    // Required field - cannot be null or empty.
    //
    // Examples:
    // "products/headphones_thumbnail.jpg"
    // "https://cdn.example.com/products/wallet.png"
    // "uploads/products/tshirt_front.webp"
    //
    // Use Cases:
    // - Product listing display
    // - Search results
    // - Category browsing
    // - Social media sharing
    // - Quick previews
    // ------------------------------------------------------
    public string Thumbnail { get; set; }

    // ==========================================================
    // Categorization
    // ==========================================================

    // ------------------------------------------------------
    // Subcategory Identifier
    // ------------------------------------------------------
    // Unique identifier (GUID) of the subcategory this
    // product belongs to.
    //
    // Required foreign key - cannot be null.
    // References the SubCategory entity.
    //
    // Example:
    // "b5c9f8d3-2a7e-4b1d-9f6c-8e4d3c2a1b5f" (subcategory ID)
    //
    // Use Cases:
    // - Product categorization
    // - Navigation and browsing
    // - Filtering and search
    // - Analytics and reporting
    // ------------------------------------------------------
    public Guid SubcategoryId { get; set; }

    // ------------------------------------------------------
    // Store Identifier
    // ------------------------------------------------------
    // Unique identifier (GUID) of the store that sells this product.
    //
    // Required foreign key - cannot be null.
    // References the Store entity.
    //
    // Example:
    // "a8d7f9e2-5c3b-4a6d-8f1e-7b2c4d9e8f3a" (store ID)
    //
    // Use Cases:
    // - Multi-store product management
    // - Store-specific inventory
    // - Store-specific pricing
    // - Store catalog management
    // ------------------------------------------------------
    public Guid StoreId { get; set; }

    // ==========================================================
    // Pricing & Inventory
    // ==========================================================

    // ------------------------------------------------------
    // Product Price
    // ------------------------------------------------------
    // The price of the product in the smallest currency unit
    // (e.g., cents, piasters).
    // Type: int for precision and performance.
    //
    // Required field - cannot be null.
    // Must be greater than or equal to 0.
    //
    // Examples:
    // 9999 = $99.99 (if stored in cents)
    // 15000 = $150.00
    // 500 = $5.00
    // 0 = Free product
    //
    // Use Cases:
    // - Order total calculation
    // - Pricing display
    // - Promotions and discounts
    // - Financial reporting
    // ------------------------------------------------------
    public int Price { get; set; }

    // ------------------------------------------------------
    // Product Quantity/Stock
    // ------------------------------------------------------
    // The available inventory count for this product.
    // Nullable - may be null if inventory tracking is disabled
    // or unlimited stock.
    //
    // Default: null
    //
    // Examples:
    // 50 = 50 units available
    // 0 = Out of stock
    // null = Unlimited stock or not tracked
    //
    // Use Cases:
    // - Inventory management
    // - Stock availability checking
    // - Order fulfillment
    // - Reorder alerts
    // ------------------------------------------------------
    public int? Quantity { get; set; } = null;

    // ------------------------------------------------------
    // Currency Symbol
    // ------------------------------------------------------
    // The currency symbol used for this product.
    // Required field - cannot be null or empty.
    //
    // Examples:
    // "$" - US Dollar
    // "€" - Euro
    // "£" - British Pound
    // "ج.م" - Egyptian Pound
    // "ر.س" - Saudi Riyal
    //
    // Use Cases:
    // - Price formatting
    // - Multi-currency support
    // - International sales
    // ------------------------------------------------------
    public String Symbol { get; set; }

    // ==========================================================
    // Navigation Properties
    // ==========================================================

    // ------------------------------------------------------
    // Product Variants Relationship
    // ------------------------------------------------------
    // Navigation property for all variants of this product.
    // Variants include options like size, color, material, etc.
    //
    // One product can have many variants.
    // Initialized as empty collection to prevent null reference.
    //
    // Example:
    // Product.ProductVariants
    //   ├─ Size: Large (+$2.00)
    //   ├─ Color: Red (+$0.00)
    //   └─ Material: Premium (+$5.00)
    //
    // Usage:
    // var basePrice = product.Price;
    // var maxVariantPrice = product.ProductVariants
    //     .Max(v => v.AdditionalPrice);
    // ------------------------------------------------------
    public virtual ICollection<ProductVariant> ProductVariants { get; set; }
        = new List<ProductVariant>();

    // ------------------------------------------------------
    // Product Images Relationship
    // ------------------------------------------------------
    // Navigation property for all images of this product.
    // Provides multiple product images for galleries.
    //
    // One product can have many images.
    // Initialized as empty collection to prevent null reference.
    //
    // Example:
    // Product.ProductImages
    //   ├─ Image #1: Front view (primary)
    //   ├─ Image #2: Back view
    //   ├─ Image #3: Side view
    //   └─ Image #4: Packaging
    //
    // Usage:
    // var primaryImage = product.ProductImages
    //     .FirstOrDefault(i => i.IsPrimary);
    // ------------------------------------------------------
    public virtual ICollection<ProductImage> ProductImages { get; set; }
        = new List<ProductImage>();

    // ------------------------------------------------------
    // SubCategory Relationship
    // ------------------------------------------------------
    // Navigation property to the subcategory this product
    // belongs to.
    //
    // Required navigation property (null! indicates it will
    // be populated by Entity Framework).
    //
    // Example:
    // Product.SubCategory.Name
    // Product.SubCategory.Category.Name
    //
    // Usage:
    // var categoryName = product.SubCategory?.Category?.Name;
    // ------------------------------------------------------
    public virtual SubCategory SubCategory { get; set; } = null!;

    // ------------------------------------------------------
    // Store Relationship
    // ------------------------------------------------------
    // Navigation property to the store that sells this product.
    //
    // Required navigation property (null! indicates it will
    // be populated by Entity Framework).
    //
    // Example:
    // Product.Store.StoreName
    // Product.Store.Currency
    //
    // Usage:
    // var storeName = product.Store.StoreName;
    // var storeCurrency = product.Store.Currency;
    // ------------------------------------------------------
    public virtual Store Store { get; set; } = null!;
}