using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.domain.entity;

// ==========================================================
// ProductVariant Entity Class
// ----------------------------------------------------------
// This class represents specific product variant instances.
// Each record defines a particular variant option (e.g., "Large",
// "Red") with its attributes and relationships to products
// and orders.
//
// Inherits from GeneralSharedInfoWithId which provides:
// - Id (Guid primary key)
// - NO CreatedAt or UpdatedAt (lightweight entity)
//
// Each object created from this class represents one row
// inside the ProductVariants table.
//
// Example:
// Id                   = "f47ac10b-58cc-4372-a567-0e02b2c3d479"
// Percentage           = 0
// VariantId            = "a8d7f9e2-5c3b-4a6d-8f1e-7b2c4d9e8f3a"
// Name                 = "Large"
// ProductId            = "b5c9f8d3-2a7e-4b1d-9f6c-8e4d3c2a1b5f"
// ==========================================================
public class ProductVariant : GeneralSharedInfoWithId
{
    // ==========================================================
    // Core Fields
    // ==========================================================

    // ------------------------------------------------------
    // Percentage
    // ------------------------------------------------------
    // Represents a percentage value associated with this variant.
    // Could be used for:
    // - Discount percentage for this variant
    // - Markup percentage over base product
    // - Commission percentage
    // - Tax percentage variation
    //
    // Type: int (whole number percentage)
    // Range: 0 to 100 (or possibly -100 to 100)
    //
    // Examples:
    // 0     = No percentage adjustment
    // 10    = 10% markup/discount
    // -5    = 5% discount
    // 15    = 15% premium
    //
    // Use Cases:
    // - Price calculation: BasePrice + (BasePrice * Percentage / 100)
    // - Discount application: Price - (Price * Percentage / 100)
    // - Reporting and analytics
    // ------------------------------------------------------
    public int Percentage { get; set; }

    // ------------------------------------------------------
    // Variant Identifier
    // ------------------------------------------------------
    // Unique identifier (GUID) of the master variant definition.
    // References the Variant entity which defines the variant
    // type/group (e.g., "Size", "Color", "Style").
    //
    // Required foreign key - cannot be null.
    // References the Variant entity.
    //
    // Example:
    // "a8d7f9e2-5c3b-4a6d-8f1e-7b2c4d9e8f3a" (variant ID)
    //
    // Use Cases:
    // - Grouping variants by type (all Size variants)
    // - Displaying variant categories
    // - Reporting by variant type
    // ------------------------------------------------------
    public Guid VariantId { get; set; }

    // ------------------------------------------------------
    // Variant Name
    // ------------------------------------------------------
    // The specific option name for this variant instance.
    // Required field - cannot be null or empty.
    //
    // Examples:
    // "Large"
    // "Red"
    // "Cotton"
    // "16GB"
    // "Silver"
    // "Extra Large"
    // "Black"
    // "2XL"
    // "Rose Gold"
    //
    // Use Cases:
    // - Display in product detail pages
    // - Order selection UI
    // - Receipts and invoices
    // - Customer communication
    // ------------------------------------------------------
    public string Name { get; set; }

    // ------------------------------------------------------
    // Product Identifier
    // ------------------------------------------------------
    // Unique identifier (GUID) of the product this variant
    // belongs to.
    //
    // Required foreign key - cannot be null.
    // References the Product entity.
    //
    // Example:
    // "b5c9f8d3-2a7e-4b1d-9f6c-8e4d3c2a1b5f" (product ID)
    // ------------------------------------------------------
    public Guid ProductId { get; set; }

    // ==========================================================
    // Navigation Properties
    // ==========================================================

    // ------------------------------------------------------
    // Variant Relationship
    // ------------------------------------------------------
    // Navigation property to the master variant definition.
    // Provides access to the variant type/group information.
    //
    // Nullable - may be null if Variant not loaded.
    // Default initialized to null.
    //
    // Example:
    // ProductVariant.Variant.Name (e.g., "Size")
    // ProductVariant.Variant.IsRequired
    // ProductVariant.Variant.DisplayOrder
    //
    // Usage:
    // var variantType = productVariant.Variant?.Name;
    // ------------------------------------------------------
    public virtual Variant? Variant { get; set; } = null;

    // ------------------------------------------------------
    // Product Relationship
    // ------------------------------------------------------
    // Navigation property to the parent product.
    // Provides access to product details.
    //
    // Nullable - may be null if Product not loaded.
    // Default initialized to null.
    //
    // Example:
    // ProductVariant.Product.Name
    // ProductVariant.Product.Description
    // ProductVariant.Product.Price
    //
    // Usage:
    // var productName = productVariant.Product?.Name;
    // ------------------------------------------------------
    public virtual Product? Product { get; set; } = null;

    // ------------------------------------------------------
    // Order Products Variants Relationship
    // ------------------------------------------------------
    // Navigation property for all order items that reference
    // this product variant.
    //
    // One product variant can appear in many orders.
    // Initialized as empty collection to prevent null reference.
    //
    // Example:
    // ProductVariant.OrderProductsVariants
    //   ├─ Order #1001 - 2 units
    //   ├─ Order #1002 - 1 unit
    //   └─ Order #1003 - 3 units
    //
    // Usage:
    // var orderCount = productVariant.OrderProductsVariants.Count;
    // var totalSold = productVariant.OrderProductsVariants
    //     .Sum(opv => opv.OrderItem.Quantity);
    // ------------------------------------------------------
    public virtual ICollection<OrderProductsVariant> OrderProductsVariants { get; set; }
        = new List<OrderProductsVariant>();

    // ==========================================================
    // Computed Properties (Recommended)
    // ==========================================================

    /*
    [NotMapped]
    public decimal PriceAdjustment => Product != null 
        ? Product.Price * (Percentage / 100m) 
        : 0;

    [NotMapped]
    public int AdjustedPrice => Product != null 
        ? Product.Price + (int)(Product.Price * Percentage / 100) 
        : 0;

    [NotMapped]
    public string DisplayName => Variant != null 
        ? $"{Variant.Name}: {Name}" 
        : Name;
    */
}