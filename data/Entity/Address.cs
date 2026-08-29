using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.domain.entity;

// ==========================================================
// Address Entity Class
// ----------------------------------------------------------
// This class represents a physical address associated with
// a user, customer, store, or any other entity in the system.
// Each record stores location information including geographic
// coordinates, a descriptive title, and current status.
//
// Inherits from GeneralShredInfo which provides:
// - Id (Guid primary key)
// - CreatedAt (creation timestamp)
// - UpdatedAt (modification timestamp) - Full audit trail
//
// Each object created from this class represents one row
// inside the Addresses table.
//
// Example:
// Id                   = "f47ac10b-58cc-4372-a567-0e02b2c3d479"
// OwnerId              = "b5c9f8d3-2a7e-4b1d-9f6c-8e4d3c2a1b5f"
// Title                = "Home Address"
// Longitude            = 30.0444
// Latitude             = 31.2357
// IsCurrent            = true
// CreatedAt            = 2026-08-27 14:30:00.000
// UpdatedAt            = null
// ==========================================================
public class Address : GeneralShredInfo
{
    // ==========================================================
    // Primary Key (Override)
    // ----------------------------------------------------------
    // Note: GeneralShredInfo already has an Id property,
    // but this class explicitly redefines it with [Key]
    // attribute for clarity and explicit mapping.
    // ==========================================================

    // ------------------------------------------------------
    // Address Identifier
    // ------------------------------------------------------
    // Unique identifier for each address record.
    // Marked with [Key] attribute as the primary key.
    // Overrides the inherited Id property from the base class.
    //
    // Uses Guid for global uniqueness and distributed
    // system compatibility.
    //
    // Examples:
    // "f47ac10b-58cc-4372-a567-0e02b2c3d479"
    // "a8d7f9e2-5c3b-4a6d-8f1e-7b2c4d9e8f3a"
    //
    // Note: This property is redefined here, but it's
    // recommended to remove it and use the inherited one
    // to avoid duplication.
    // ------------------------------------------------------
    [Key]
    public new Guid Id { get; set; }

    // ==========================================================
    // Geographic Coordinates
    // ==========================================================

    // ------------------------------------------------------
    // Longitude
    // ------------------------------------------------------
    // Geographic longitude coordinate (east-west position).
    // Used for mapping, distance calculations, and geofencing.
    //
    // Type: decimal? (Nullable)
    // - decimal for high precision
    // - nullable to support addresses without coordinates
    //
    // Range: -180 to +180 degrees
    // Negative = West, Positive = East
    //
    // Database precision recommended: decimal(10,7)
    // Provides precision up to approximately 1cm.
    //
    // Examples:
    // 30.0444 (Cairo, Egypt)
    // -74.0060 (New York, USA)
    // 2.3522 (Paris, France)
    // null (No coordinates available)
    //
    // Use Cases:
    // - Map display
    // - Distance calculations
    // - Geofencing
    // - Delivery route optimization
    // - Address verification
    // ------------------------------------------------------
    public decimal? Longitude { get; set; } = null;

    // ------------------------------------------------------
    // Latitude
    // ------------------------------------------------------
    // Geographic latitude coordinate (north-south position).
    // Used for mapping, distance calculations, and geofencing.
    //
    // Type: decimal? (Nullable)
    // - decimal for high precision
    // - nullable to support addresses without coordinates
    //
    // Range: -90 to +90 degrees
    // Negative = South, Positive = North
    //
    // Database precision recommended: decimal(10,7)
    // Provides precision up to approximately 1cm.
    //
    // Examples:
    // 31.2357 (Cairo, Egypt)
    // 40.7128 (New York, USA)
    // 48.8566 (Paris, France)
    // null (No coordinates available)
    //
    // Use Cases:
    // - Map display
    // - Distance calculations
    // - Geofencing
    // - Delivery route optimization
    // - Address verification
    // ------------------------------------------------------
    public decimal? Latitude { get; set; } = null;

    // ==========================================================
    // Address Information
    // ==========================================================

    // ------------------------------------------------------
    // Address Title
    // ------------------------------------------------------
    // A descriptive name or label for the address.
    // Helps users identify and manage multiple addresses.
    //
    // Required field - cannot be null or empty.
    //
    // Examples:
    // "Home Address"
    // "Office - Main Branch"
    // "Delivery Location"
    // "Warehouse #3"
    // "Apartment 5B"
    // "Work Place"
    // "Parents' House"
    // "Summer Home"
    // "Billing Address"
    //
    // Use Cases:
    // - Display in address selection dropdowns
    // - Quick identification in UI
    // - Grouping and sorting addresses
    // - User-friendly address management
    // ------------------------------------------------------
    public string Title { get; set; }

    // ------------------------------------------------------
    // Current Address Flag
    // ------------------------------------------------------
    // Indicates whether this is the current/active address
    // for the owner. Only one address should be marked as
    // current per owner at any given time.
    //
    // Default: false
    //
    // Examples:
    // true  = This is the current address
    // false = This is a previous/alternative address
    //
    // Use Cases:
    // - Primary delivery address
    // - Billing address
    // - Default location selection
    // - Address history tracking
    // - User's current residence
    // - Shipping default
    //
    // Business Rule:
    // When adding a new address with IsCurrent = true,
    // automatically set all other addresses of the same
    // owner to IsCurrent = false.
    // ------------------------------------------------------
    public bool IsCurrent { get; set; } = false;

    // ==========================================================
    // Foreign Keys
    // ==========================================================

    // ------------------------------------------------------
    // Owner Identifier
    // ------------------------------------------------------
    // Unique identifier of the entity that owns this address.
    // Supports polymorphic relationships with different
    // entity types (Users, Stores, Employees, etc.).
    //
    // Required field - cannot be null.
    //
    // Examples:
    // "b5c9f8d3-2a7e-4b1d-9f6c-8e4d3c2a1b5f" (User ID)
    // "a8d7f9e2-5c3b-4a6d-8f1e-7b2c4d9e8f3a" (Store ID)
    // "e7d3f4a2-8b1c-4e3d-9f5a-6c8b2d1e4f7a" (Employee ID)
    //
    // Use Cases:
    // - User's shipping addresses
    // - Store's location
    // - Employee's residence
    // - Delivery points
    // - Billing addresses
    // - Customer addresses
    // ------------------------------------------------------
    public Guid OwnerId { get; set; }

    // ==========================================================
    // Navigation Properties (Optional Enhancements)
    // ==========================================================

    // Note: The current implementation doesn't include
    // navigation properties. Consider adding them based on
    // your entity relationships.

    /*
    // Example 1: User Owner (for user addresses)
    [ForeignKey(nameof(OwnerId))]
    public virtual User? User { get; set; }

    // Example 2: Store Owner (for store locations)
    [ForeignKey(nameof(OwnerId))]
    public virtual Store? Store { get; set; }

    // Example 3: Polymorphic with OwnerType
    // If you need to track different owner types
    public string? OwnerType { get; set; }
    */
}