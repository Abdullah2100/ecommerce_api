using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

 
namespace api.domain.entity;

// ==========================================================
// Category Entity Class
// ----------------------------------------------------------
// This class represents product or service categories within
// the system. Each record defines a category that can contain
// multiple sub-categories, and is owned by a specific user.
//
// Inherits from GeneralShredInfo which provides:
// - Id (Guid primary key)
// - CreatedAt (creation timestamp)
// - UpdatedAt (modification timestamp) - Full audit trail
//
// Each object created from this class represents one row
// inside the Categories table.
//
// Example:
// Id                   = "f47ac10b-58cc-4372-a567-0e02b2c3d479"
// Name                 = "Electronics"
// OwnerId              = "b5c9f8d3-2a7e-4b1d-9f6c-8e4d3c2a1b5f"
// Image                = "categories/electronics.jpg"
// IsBlocked            = false
// CreatedAt            = 2026-08-27 14:30:00.000
// UpdatedAt            = null
// ==========================================================
public class Category : GeneralShredInfo
{
    // ==========================================================
    // Basic Information
    // ==========================================================

    // ------------------------------------------------------
    // Category Name
    // ------------------------------------------------------
    // The display name of the category. This is the primary
    // identifier shown to users in the UI.
    //
    // Required field - cannot be null or empty.
    //
    // Examples:
    // "Electronics"
    // "Clothing & Fashion"
    // "Home & Garden"
    // "Books & Media"
    // "Food & Beverage"
    // "Health & Beauty"
    // "Sports & Outdoors"
    // "Toys & Games"
    // "Automotive"
    // "Pet Supplies"
    //
    // Use Cases:
    // - Product categorization
    // - Navigation menu
    // - Search filtering
    // - Reporting and analytics
    // - Inventory management
    // ------------------------------------------------------
    public string Name { get; set; }

    // ------------------------------------------------------
    // Category Image
    // ------------------------------------------------------
    // The image path, URL, or base64 representation for the
    // category. Used for visual representation in the UI.
    //
    // Required field - cannot be null or empty.
    //
    // String type for maximum flexibility:
    // - File path: "categories/electronics.jpg"
    // - URL: "https://cdn.example.com/categories/fashion.png"
    // - Base64: "data:image/png;base64,iVBORw0KGgo..."
    // - Cloud storage: "https://s3.amazonaws.com/bucket/category.jpg"
    //
    // Examples:
    // "categories/electronics_2026.jpg"
    // "https://images.example.com/categories/clothing.png"
    // "uploads/categories/store123/electronics.webp"
    // "https://cdn.storage.com/categories/books.svg"
    //
    // Use Cases:
    // - Category thumbnails
    // - Menu icons
    // - Banner images
    // - Marketing materials
    // - Mobile app display
    // ------------------------------------------------------
    public string Image { get; set; }

    // ==========================================================
    // Status & Administrative Fields
    // ==========================================================

    // ------------------------------------------------------
    // Block Status
    // ------------------------------------------------------
    // Indicates whether the category is currently blocked
    // or deactivated. When true, the category is hidden from
    // users and cannot be used for new products.
    //
    // Default: false (category is active)
    //
    // Use cases:
    // - Category is no longer in use
    // - Temporary deactivation
    // - Policy violations
    // - Seasonal categories
    // - Content moderation
    //
    // Effects:
    // - Category hidden from UI
    // - Products may be moved or hidden
    // - Reporting may exclude blocked categories
    // - New products cannot be added
    //
    // Examples:
    // false = Category is active and visible
    // true  = Category is blocked/hidden
    // ------------------------------------------------------
    public bool IsBlocked { get; set; } = false;

    // ==========================================================
    // Foreign Keys
    // ==========================================================

    // ------------------------------------------------------
    // Owner Identifier
    // ------------------------------------------------------
    // Unique identifier (GUID) of the user who owns this category.
    // Each category belongs to a specific user (store owner,
    // merchant, or administrator).
    //
    // Required foreign key - cannot be null.
    //
    // Example:
    // "b5c9f8d3-2a7e-4b1d-9f6c-8e4d3c2a1b5f" (user ID)
    //
    // Use Cases:
    // - User-specific categories
    // - Multi-vendor categorization
    // - Personalized catalog
    // - Access control
    // - Ownership verification
    // ------------------------------------------------------
    public Guid OwnerId { get; set; }

    // ==========================================================
    // Navigation Properties
    // ==========================================================

    // ------------------------------------------------------
    // User (Owner) Relationship
    // ------------------------------------------------------
    // Navigation property to the user who owns this category.
    // Provides access to user details and preferences.
    //
    // Required navigation property (null! indicates it will
    // be populated by Entity Framework).
    //
    // Example:
    // Category.User.FullName
    // Category.User.Phone
    // Category.User.Email
    //
    // Usage:
    // var ownerName = category.User.FullName;
    // var ownerPhone = category.User.Phone;
    // ------------------------------------------------------
    public virtual User User { get; set; } = null!;

    // ------------------------------------------------------
    // Sub-Categories Relationship
    // ------------------------------------------------------
    // Navigation property for all sub-categories belonging
    // to this category. Represents the hierarchical structure.
    //
    // One category can have many sub-categories.
    // Initialized as empty List to prevent null reference.
    //
    // Example:
    // Category.SubCategories
    //   ├─ Electronics
    //   │   ├─ Smartphones
    //   │   ├─ Laptops
    //   │   └─ Accessories
    //   └─ Clothing
    //       ├─ Men's Wear
    //       ├─ Women's Wear
    //       └─ Kids' Wear
    //
    // Usage:
    // var subCategories = category.SubCategories
    //     .Where(sc => !sc.IsBlocked)
    //     .Select(sc => sc.Name)
    //     .ToList();
    // ------------------------------------------------------
    public virtual ICollection<SubCategory> SubCategories { get; set; }
        = new List<SubCategory>();
}