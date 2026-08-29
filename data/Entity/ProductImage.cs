using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.domain.entity;

// ==========================================================
// ProductImage Entity Class
// ----------------------------------------------------------
// This class represents images associated with a product.
// Each record stores the image path and links to its product.
//
// Inherits from GeneralSharedInfoWithId which provides:
// - Id (Guid primary key)
// - NO CreatedAt or UpdatedAt (lightweight entity)
//
// Each object created from this class represents one row
// inside the ProductImages table.
//
// Example:
// Id                   = "f47ac10b-58cc-4372-a567-0e02b2c3d479"
// ProductId            = "a8d7f9e2-5c3b-4a6d-8f1e-7b2c4d9e8f3a"
// Path                 = "products/electronics/phone_front.jpg"
// ==========================================================
public class ProductImage : GeneralSharedInfoWithId
{
    // ------------------------------------------------------
    // Image Path
    // ------------------------------------------------------
    // The file path, URL, or CDN location of the product image.
    // Required field - cannot be null or empty.
    //
    // String type for maximum flexibility:
    // - File path: "products/electronics/phone_front.jpg"
    // - URL: "https://cdn.example.com/products/phone.jpg"
    // - Cloud storage: "https://s3.amazonaws.com/bucket/product.png"
    //
    // Examples:
    // "products/electronics/iphone_15_front.jpg"
    // "products/clothing/tshirt_red.png"
    // "https://images.example.com/products/headphones.webp"
    // "uploads/products/store123/product_001.jpg"
    //
    // Use Cases:
    // - Product gallery display
    // - Thumbnail generation
    // - Product catalog
    // - Marketing materials
    // ------------------------------------------------------
    public string Path { get; set; }

    // ------------------------------------------------------
    // Product Identifier
    // ------------------------------------------------------
    // Unique identifier (GUID) of the product this image
    // belongs to.
    //
    // Required foreign key - cannot be null.
    // References the Product entity.
    //
    // Example:
    // "a8d7f9e2-5c3b-4a6d-8f1e-7b2c4d9e8f3a" (product ID)
    // ------------------------------------------------------
    public Guid ProductId { get; set; }

    // ------------------------------------------------------
    // Navigation Properties
    // ------------------------------------------------------
    // Navigation property to the product this image belongs to.
    // Required navigation property (null! indicates it will
    // be populated by Entity Framework).
    //
    // Example:
    // ProductImage.Product.Name
    // ProductImage.Product.Description
    // ------------------------------------------------------
    public virtual Product Product { get; set; } = null!;
}