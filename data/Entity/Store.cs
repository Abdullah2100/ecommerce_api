using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

 
namespace api.domain.entity;

// ==========================================================
// Store Entity Class
// ----------------------------------------------------------
// This class represents a store/business in the system.
// Each record tracks store information including name, images,
// status, ownership, and relationships with other entities.
//
// Inherits from GeneralShredInfo which provides:
// - Id (Guid primary key)
// - CreatedAt (creation timestamp)
// - UpdatedAt (modification timestamp) - Full audit trail
//
// Each object created from this class represents one row
// inside the Stores table.
//
// Example:
// Id                   = "f47ac10b-58cc-4372-a567-0e02b2c3d479"
// Name                 = "Tech Store Downtown"
// WallpaperImage       = "stores/tech_store_wallpaper.jpg"
// SmallImage           = "stores/tech_store_logo.png"
// IsBlock              = false
// UserId               = "b5c9f8d3-2a7e-4b1d-9f6c-8e4d3c2a1b5f"
// CreatedAt            = 2026-08-27 14:30:00.000
// UpdatedAt            = null
// ==========================================================
public class Store : GeneralShredInfo
{
    // ==========================================================
    // Basic Information
    // ==========================================================

    // ------------------------------------------------------
    // Store Name
    // ------------------------------------------------------
    // The display name of the store/business.
    // Required field - cannot be null or empty.
    //
    // Examples:
    // "Tech Store Downtown"
    // "Fashion Boutique"
    // "Home & Garden Mart"
    // "Electronics World"
    // "Book Haven"
    // "Coffee Shop Express"
    // "Sports Gear Outlet"
    // "Pharmacy Plus"
    //
    // Use Cases:
    // - Store identification
    // - Display in UI
    // - Search and filtering
    // - Reports and analytics
    // - Customer communication
    // ------------------------------------------------------
    public string Name { get; set; }

    // ------------------------------------------------------
    // Store Wallpaper Image
    // ------------------------------------------------------
    // The full-size background/cover image for the store.
    // Used as the main visual header on store pages.
    // Required field - cannot be null or empty.
    //
    // String type for maximum flexibility:
    // - File path: "stores/tech_store_wallpaper.jpg"
    // - URL: "https://cdn.example.com/stores/wallpaper.png"
    // - Cloud storage: "https://s3.amazonaws.com/bucket/store.jpg"
    //
    // Examples:
    // "stores/tech_store_wallpaper.jpg"
    // "https://images.example.com/stores/fashion_bg.png"
    // "uploads/stores/store123/wallpaper_2026.webp"
    //
    // Use Cases:
    // - Store page header
    // - Branding
    // - Visual identity
    // - Marketing materials
    // ------------------------------------------------------
    public string WallpaperImage { get; set; }

    // ------------------------------------------------------
    // Store Small Image
    // ------------------------------------------------------
    // The small thumbnail/logo image for the store.
    // Used as the store's profile picture or icon.
    // Required field - cannot be null or empty.
    //
    // String type for maximum flexibility:
    // - File path: "stores/tech_store_logo.png"
    // - URL: "https://cdn.example.com/stores/logo.svg"
    // - Cloud storage: "https://s3.amazonaws.com/bucket/store_icon.jpg"
    //
    // Examples:
    // "stores/tech_store_logo.png"
    // "https://images.example.com/stores/fashion_logo.svg"
    // "uploads/stores/store123/logo_2026.webp"
    //
    // Use Cases:
    // - Store list display
    // - Search results
    // - Navigation
    // - Order items
    // - Notifications
    // ------------------------------------------------------
    public string SmallImage { get; set; }

    // ==========================================================
    // Status & Ownership
    // ==========================================================

    // ------------------------------------------------------
    // Block Status
    // ------------------------------------------------------
    // Indicates whether the store is blocked/suspended.
    // Default: true (store is blocked by default)
    //
    // When blocked:
    // - Store is hidden from users
    // - No new orders can be placed
    // - Products are hidden
    // - Store cannot be edited
    //
    // Examples:
    // true  = Store is blocked/suspended
    // false = Store is active and operational
    //
    // Use Cases:
    // - New stores (pending approval) - default true
    // - Suspension for policy violations
    // - Temporary closure
    // - Permanent deactivation
    // - Fraud prevention
    // ------------------------------------------------------
    public bool IsBlock { get; set; } = true;

    // ------------------------------------------------------
    // User Identifier
    // ------------------------------------------------------
    // Unique identifier (GUID) of the user who owns/manages
    // this store.
    //
    // Required foreign key - cannot be null.
    // References the User entity.
    //
    // Example:
    // "b5c9f8d3-2a7e-4b1d-9f6c-8e4d3c2a1b5f" (user ID)
    //
    // Use Cases:
    // - Store ownership
    // - Access control
    // - Multi-store management
    // - Reporting and analytics
    // - Audit trail
    // ------------------------------------------------------
    public Guid UserId { get; set; }

    // ==========================================================
    // Navigation Properties
    // ==========================================================

    // ------------------------------------------------------
    // Addresses Relationship
    // ------------------------------------------------------
    // Navigation property for all addresses associated with
    // this store.
    //
    // One store can have many addresses.
    // Initialized as empty collection to prevent null reference.
    //
    // Examples:
    // Store.Addresses
    //   ├─ Main Branch (Current)
    //   ├─ Warehouse
    //   └─ Secondary Location
    //
    // Usage:
    // var mainAddress = store.Addresses
    //     .FirstOrDefault(a => a.IsCurrent);
    // var addressCount = store.Addresses.Count;
    // ------------------------------------------------------
    public virtual ICollection<Address> Addresses { get; set; }
        = new List<Address>();

    // ------------------------------------------------------
    // SubCategories Relationship
    // ------------------------------------------------------
    // Navigation property for all subcategories this store
    // offers products in.
    //
    // One store can have many subcategories.
    // Initialized as empty collection to prevent null reference.
    //
    // Examples:
    // Store.SubCategories
    //   ├─ Electronics
    //   │   ├─ Phones
    //   │   └─ Laptops
    //   └─ Accessories
    //       ├─ Cases
    //       └─ Chargers
    //
    // Usage:
    // var categories = store.SubCategories
    //     .Where(sc => !sc.IsDeleted)
    //     .Select(sc => sc.Name)
    //     .ToList();
    // ------------------------------------------------------
    public virtual ICollection<SubCategory> SubCategories { get; set; }
        = new List<SubCategory>();

    // ------------------------------------------------------
    // Banners Relationship
    // ------------------------------------------------------
    // Navigation property for all promotional banners of this store.
    //
    // One store can have many banners.
    // Initialized as empty collection to prevent null reference.
    //
    // Examples:
    // Store.Banners
    //   ├─ Summer Sale Banner (Active)
    //   ├─ New Arrivals Banner (Active)
    //   └─ Clearance Sale Banner (Expired)
    //
    // Usage:
    // var activeBanners = store.Banners
    //     .Where(b => b.EndAt > DateTime.UtcNow)
    //     .ToList();
    // ------------------------------------------------------
    public virtual ICollection<Banner> Banners { get; set; }
        = new List<Banner>();

    // ------------------------------------------------------
    // Products Relationship
    // ------------------------------------------------------
    // Navigation property for all products offered by this store.
    //
    // One store can have many products.
    // Initialized as empty collection to prevent null reference.
    //
    // Examples:
    // Store.Products
    //   ├─ iPhone 15 Pro
    //   ├─ Samsung Galaxy S24
    //   ├─ AirPods Pro
    //   └─ MacBook Pro
    //
    // Usage:
    // var productCount = store.Products.Count;
    // var activeProducts = store.Products
    //     .Where(p => p.IsActive && !p.IsDeleted)
    //     .ToList();
    // ------------------------------------------------------
    public virtual ICollection<Product> Products { get; set; }
        = new List<Product>();

    // ------------------------------------------------------
    // Order Items Relationship (⚠️ Typo)
    // ------------------------------------------------------
    // Navigation property for all order items from this store.
    //
    // NOTE: Property name "OddrderItems" appears to be a typo.
    // Should be "OrderItems" for consistency.
    //
    // One store can have many order items.
    // Initialized as empty collection to prevent null reference.
    //
    // Examples:
    // Store.OrderItems
    //   ├─ Order #1001 - Item 1
    //   ├─ Order #1001 - Item 2
    //   └─ Order #1002 - Item 1
    //
    // Usage:
    // var totalSales = store.OrderItems
    //     .Sum(oi => oi.Price * oi.Quantity);
    // ------------------------------------------------------
    public virtual ICollection<OrderItem> OddrderItems { get; set; }
        = new List<OrderItem>();

    // ------------------------------------------------------
    // User Relationship
    // ------------------------------------------------------
    // Navigation property to the user who owns this store.
    // Provides access to user details.
    //
    // Required navigation property (null! indicates it will
    // be populated by Entity Framework).
    //
    // Example:
    // Store.User.FullName
    // Store.User.Phone
    // Store.User.Email
    //
    // Note: Property name is "user" (lowercase) - inconsistent
    // with PascalCase naming convention.
    // Recommended: "User" (uppercase)
    // ------------------------------------------------------
    public virtual User user { get; set; } = null!;
}