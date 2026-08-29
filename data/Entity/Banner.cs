using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.domain.entity;
 

// ==========================================================
// Banner Entity Class
// ----------------------------------------------------------
// This class represents promotional banners or advertisements
// displayed within the system. Each record stores banner image
// information, expiration date, and association with a store.
//
// Inherits from GeneralSharedInfoWithCreatedAt which provides:
// - Id (Guid primary key)
// - CreatedAt (creation timestamp)
//
// Each object created from this class represents one row
// inside the Banners table.
//
// Example:
// Id                   = "f47ac10b-58cc-4372-a567-0e02b2c3d479"
// StoreId              = "a8d7f9e2-5c3b-4a6d-8f1e-7b2c4d9e8f3a"
// Image                = "banners/summer_sale_2026.png"
// EndAt                = 2026-09-30 23:59:59.999
// CreatedAt            = 2026-08-01 09:00:00.000
// ==========================================================
public class Banner : GeneralSharedInfoWithCreatedAt
{
    // ==========================================================
    // Banner Content
    // ==========================================================

    // ------------------------------------------------------
    // Banner Image
    // ------------------------------------------------------
    // The image path, URL, or base64 representation of the
    // banner image. This can be a file path, CDN URL, or
    // encoded image data.
    //
    // Required field - cannot be null or empty.
    //
    // String type used for maximum flexibility:
    // - File path: "banners/summer_sale_2026.png"
    // - URL: "https://cdn.example.com/banners/promo.jpg"
    // - Base64: "data:image/png;base64,iVBORw0KGgo..."
    // - Cloud storage: "https://s3.amazonaws.com/bucket/banner.jpg"
    //
    // Examples:
    // "banners/summer_promo.png"
    // "https://images.example.com/banners/new-year-2026.jpg"
    // "uploads/banners/store123/banner_001.png"
    // "https://cdn.storage.com/banners/black-friday.webp"
    //
    // Use Cases:
    // - Promotional campaigns
    // - Seasonal offers
    // - Event announcements
    // - New product launches
    // - Store-specific advertising
    // ------------------------------------------------------
    public String Image { get; set; }

    // ==========================================================
    // Banner Duration
    // ==========================================================

    // ------------------------------------------------------
    // Banner End Date
    // ------------------------------------------------------
    // The date and time when this banner expires and should
    // no longer be displayed. Banners are automatically
    // considered inactive after this date.
    //
    // Required field - must be set on creation.
    // Should be in UTC format for consistency.
    // Column Type: "Timestamp" in the database.
    //
    // Examples:
    // 2026-09-30 23:59:59.999 (End of September promotion)
    // 2026-12-31 23:59:59.999 (Year-end sale)
    // 2027-01-01 00:00:00.000 (New Year start)
    //
    // Use Cases:
    // - Time-limited promotions
    // - Seasonal campaigns
    // - Scheduled advertising
    // - Automatic expiry management
    // - Compliance with advertising regulations
    //
    // Query Example:
    // var activeBanners = await context.Banners
    //     .Where(b => b.EndAt > DateTime.UtcNow)
    //     .ToListAsync();
    // ------------------------------------------------------
    [Column(TypeName = "Timestamp")]
    public DateTime EndAt { get; set; }

    // ==========================================================
    // Foreign Keys
    // ==========================================================

    // ------------------------------------------------------
    // Store Identifier
    // ------------------------------------------------------
    // Unique identifier (GUID) of the store that this banner
    // belongs to. Banners are store-specific and only displayed
    // to users of that store.
    //
    // Required foreign key - cannot be null.
    //
    // Example:
    // "a8d7f9e2-5c3b-4a6d-8f1e-7b2c4d9e8f3a" (store ID)
    //
    // Use Cases:
    // - Store-specific promotions
    // - Regional advertising
    // - Branch-specific offers
    // - Multi-store banner management
    // ------------------------------------------------------
    public Guid StoreId { get; set; }

    // ==========================================================
    // Navigation Properties
    // ==========================================================

    // ------------------------------------------------------
    // Store Relationship
    // ------------------------------------------------------
    // Navigation property to the store that owns this banner.
    // Provides access to store details and configuration.
    //
    // Required navigation property (null! indicates it will
    // be populated by Entity Framework).
    //
    // Example:
    // Banner.Store.StoreName
    // Banner.Store.Locale
    // Banner.Store.Currency
    //
    // Usage:
    // var storeName = banner.Store.StoreName;
    // var storeLocale = banner.Store.Locale;
    // ------------------------------------------------------
    public virtual Store Store { get; set; } = null!;
}