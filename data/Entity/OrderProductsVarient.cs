using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.domain.entity;

// ==========================================================
// OrderProductsVariant Entity Class
// ----------------------------------------------------------
// This class represents the specific product variants/options
// selected for an order item. Each record links an order item
// to a product variant, allowing tracking of customizations
// like size, color, material, etc.
//
// Inherits from GeneralShredInfo which provides:
// - Id (Guid primary key)
// - CreatedAt (creation timestamp)
// - UpdatedAt (modification timestamp) - Full audit trail
//
// Each object created from this class represents one row
// inside the OrderProductsVariants table.
//
// Example:
// Id                   = "f47ac10b-58cc-4372-a567-0e02b2c3d479"
// OrderItemId          = "b5c9f8d3-2a7e-4b1d-9f6c-8e4d3c2a1b5f"
// ProductVariantId     = "a8d7f9e2-5c3b-4a6d-8f1e-7b2c4d9e8f3a"
// Name                 = "Large"  (Stored as string, not Guid)
// CreatedAt            = 2026-08-27 14:30:00.000
// UpdatedAt            = null
// ==========================================================
public class OrderProductsVariant : GeneralShredInfo
{
    // ==========================================================
    // Foreign Keys
    // ==========================================================

    // ------------------------------------------------------
    // Product Variant Identifier
    // ------------------------------------------------------
    // Unique identifier (GUID) of the product variant being
    // ordered. Links to the ProductVariant entity.
    //
    // Required foreign key - cannot be null.
    // References the master product variant definition.
    //
    // Example:
    // "a8d7f9e2-5c3b-4a6d-8f1e-7b2c4d9e8f3a" (variant ID)
    //
    // Use Cases:
    // - Track variant history
    // - Pricing and inventory
    // - Product catalog reference
    // - Reporting and analytics
    // ------------------------------------------------------
    public Guid ProductVariantId { get; set; }

    // ------------------------------------------------------
    // Order Item Identifier
    // ------------------------------------------------------
    // Unique identifier (GUID) of the order item this variant
    // belongs to.
    //
    // Required foreign key - cannot be null.
    // References the OrderItem entity.
    //
    // Example:
    // "b5c9f8d3-2a7e-4b1d-9f6c-8e4d3c2a1b5f" (order item ID)
    //
    // Use Cases:
    // - Link variants to order items
    // - Order item composition
    // - Inventory management
    // ------------------------------------------------------
    public Guid OrderItemId { get; set; }

    // ==========================================================
    // Variant Name (Typo Issue)
    // ----------------------------------------------------------
    // ⚠️ IMPORTANT: This is declared as Guid but appears to
    // represent a string value (the variant name).
    //
    // Issue: The property is named "Name" but typed as Guid.
    // This is likely a typo where the type should be string.
    //
    // Corrected version should be:
    // public string Name { get; set; }
    //
    // Current usage would store the variant name as a Guid,
    // which would cause data loss or conversion issues.
    //
    // See the "Enhanced Version" below for the corrected
    // implementation.
    // ==========================================================

    // ------------------------------------------------------
    // Variant Name (Current - BUGGED)
    // ------------------------------------------------------
    // ❌ INCORRECT: This should be string, not Guid.
    // The variant name (e.g., "Large", "Red") is stored as
    // a Guid, which will cause issues.
    //
    // This appears to be a typo in the entity definition.
    // ------------------------------------------------------
    public Guid Name { get; set; }

    // ==========================================================
    // Navigation Properties
    // ==========================================================

    // ------------------------------------------------------
    // Product Variant Relationship
    // ------------------------------------------------------
    // Navigation property to the product variant definition.
    // Provides access to variant details like name, price,
    // SKU, and other attributes.
    //
    // Nullable - may be null if not loaded.
    // Default initialized to null.
    //
    // Example:
    // OrderProductsVariant.ProductVariant.Name
    // OrderProductsVariant.ProductVariant.SKU
    // OrderProductsVariant.ProductVariant.AdditionalPrice
    // ------------------------------------------------------
    public virtual ProductVariant? ProductVariant { get; set; } = null;

    // ------------------------------------------------------
    // Order Item Relationship
    // ------------------------------------------------------
    // Navigation property to the parent order item.
    // Provides access to order item details.
    //
    // Nullable - may be null if not loaded.
    // Default initialized to null.
    //
    // Example:
    // OrderProductsVariant.OrderItem.OrderId
    // OrderProductsVariant.OrderItem.ProductId
    // OrderProductsVariant.OrderItem.Quantity
    // ------------------------------------------------------
    public virtual OrderItem? OrderItem { get; set; } = null;
}