using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.domain.entity;
 
// ==========================================================
// Delivery Entity Class
// ----------------------------------------------------------
// This class represents delivery personnel or drivers in the
// system. Each record tracks a delivery person's information,
// status, device details, and associations with orders and users.
//
// Inherits from GeneralShredInfo which provides:
// - Id (Guid primary key)
// - CreatedAt (creation timestamp)
// - UpdatedAt (modification timestamp) - Full audit trail
//
// Each object created from this class represents one row
// inside the Deliveries table.
//
// Example:
// Id                   = "f47ac10b-58cc-4372-a567-0e02b2c3d479"
// UserId               = "b5c9f8d3-2a7e-4b1d-9f6c-8e4d3c2a1b5f"
// DeviceToken          = "ExponentPushToken[xxxxxxxxxxxxxxxxxxxxxx]"
// IsAvailable          = true
// IsBlocked            = false
// Thumbnail            = "delivery/avatar_john.jpg"
// BelongTo             = null
// CreatedAt            = 2026-08-27 14:30:00.000
// UpdatedAt            = null
// ==========================================================
public class Delivery : GeneralShredInfo
{
    // ==========================================================
    // Primary Key (Override)
    // ----------------------------------------------------------
    // Note: GeneralShredInfo already has an Id property,
    // but this class explicitly redefines it with [Key] attribute
    // for clarity and explicit mapping.
    // ==========================================================

    // ------------------------------------------------------
    // Delivery Identifier
    // ------------------------------------------------------
    // Unique identifier for each delivery person record.
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
    // User Association
    // ==========================================================

    // ------------------------------------------------------
    // User Identifier
    // ------------------------------------------------------
    // Unique identifier (GUID) of the user who is a delivery person.
    // Links the delivery record to the user account.
    //
    // Required foreign key - cannot be null.
    // The user must have a valid account in the system.
    //
    // Example:
    // "b5c9f8d3-2a7e-4b1d-9f6c-8e4d3c2a1b5f" (user ID)
    //
    // Use Cases:
    // - Authentication
    // - Access control
    // - Personal information
    // - Contact details
    // - Payment processing
    // ------------------------------------------------------
    public Guid UserId { get; set; }

    // ==========================================================
    // Device & Communication
    // ==========================================================

    // ------------------------------------------------------
    // Device Token
    // ------------------------------------------------------
    // Push notification token for the delivery person's device.
    // Used for sending real-time notifications about new orders,
    // updates, and status changes.
    //
    // Nullable - may be null if not registered or not using
    // push notifications.
    //
    // Examples:
    // "ExponentPushToken[xxxxxxxxxxxxxxxxxxxxxx]" (Expo)
    // "fcm-token-xxxxxxxxxxxxxxxx" (Firebase Cloud Messaging)
    // "APNS-token-xxxxxxxxxxxxxxxx" (Apple Push Notification)
    // null (No device registered)
    //
    // Use Cases:
    // - Order assignment notifications
    // - Status updates
    // - Real-time tracking
    // - Emergency alerts
    // - Communication with dispatcher
    // ------------------------------------------------------
    public string? DeviceToken { get; set; } = null;

    // ==========================================================
    // Status & Availability
    // ==========================================================

    // ------------------------------------------------------
    // Availability Status
    // ------------------------------------------------------
    // Indicates whether the delivery person is currently
    // available to accept new orders.
    //
    // Default: true (available for work)
    //
    // Examples:
    // true  = Available for new orders
    // false = Unavailable/Off-duty/Busy
    //
    // Use Cases:
    // - Order assignment logic
    // - Work shift management
    // - Break times
    // - After-hours status
    // ------------------------------------------------------
    public bool IsAvailable { get; set; } = true;

    // ------------------------------------------------------
    // Block Status
    // ------------------------------------------------------
    // Indicates whether the delivery person is blocked from
    // using the system. When true, they cannot accept orders
    // or perform delivery operations.
    //
    // Default: false (not blocked)
    //
    // Use cases:
    // - Policy violations
    // - Performance issues
    // - Fraud investigation
    // - Temporary suspension
    // - Account deactivation
    //
    // Examples:
    // false = Delivery person can work normally
    // true  = Delivery person is blocked/suspended
    // ------------------------------------------------------
    public bool IsBlocked { get; set; } = false;

    // ==========================================================
    // Profile & Media
    // ==========================================================

    // ------------------------------------------------------
    // Delivery Person Thumbnail
    // ------------------------------------------------------
    // Image path, URL, or base64 representation of the
    // delivery person's profile picture or avatar.
    //
    // Nullable - may be null if no image is set.
    //
    // Examples:
    // "delivery/avatar_john_doe.jpg"
    // "https://cdn.example.com/delivery/john_doe.png"
    // "uploads/delivery/avatars/delivery_123.webp"
    // null (No image set)
    //
    // Use Cases:
    // - Profile display in app
    // - Customer-facing identification
    // - Team management
    // - Reporting
    // ------------------------------------------------------
    public string? Thumbnail { get; set; }

    // ==========================================================
    // Organizational/Group Association
    // ==========================================================

    // ------------------------------------------------------
    // Belong To (Group/Team/Store)
    // ------------------------------------------------------
    // Identifier for the group, team, store, or organization
    // this delivery person belongs to.
    //
    // Nullable - may be null if not assigned to any group.
    //
    // Examples:
    // "e7d3f4a2-8b1c-4e3d-9f5a-6c8b2d1e4f7a" (Store ID)
    // "f47ac10b-58cc-4372-a567-0e02b2c3d479" (Team ID)
    // null (Independent/Freelance)
    //
    // Use Cases:
    // - Store-specific delivery
    // - Team management
    // - Zone assignment
    // - Reporting by group
    // - Resource allocation
    // ------------------------------------------------------
    public Guid? BelongTo { get; set; } = null;

    // ==========================================================
    // Navigation Properties
    // ==========================================================

    // ------------------------------------------------------
    // Address Relationship
    // ------------------------------------------------------
    // Navigation property to the delivery person's primary
    // or current address. Used for location tracking and
    // assignments.
    //
    // Nullable - may be null if address not set.
    //
    // Example:
    // Delivery.Address.Title
    // Delivery.Address.Longitude
    // Delivery.Address.Latitude
    // Delivery.Address.IsCurrent
    //
    // Usage:
    // var currentLocation = delivery.Address;
    // ------------------------------------------------------
    public virtual Address? Address { get; set; } = null;

    // ------------------------------------------------------
    // User Relationship
    // ------------------------------------------------------
    // Navigation property to the user account of this
    // delivery person. Provides access to user details.
    //
    // Required navigation property (null! indicates it will
    // be populated by Entity Framework).
    //
    // Example:
    // Delivery.User.FullName
    // Delivery.User.Phone
    // Delivery.User.Email
    //
    // Usage:
    // var name = delivery.User.FullName;
    // var phone = delivery.User.Phone;
    // ------------------------------------------------------
    public virtual User User { get; set; } = null!;

    // ------------------------------------------------------
    // Orders Relationship
    // ------------------------------------------------------
    // Navigation property for all orders assigned to or
    // delivered by this delivery person.
    //
    // Nullable - may be null if no orders assigned.
    // Initialized as empty List to prevent null reference.
    //
    // Example:
    // Delivery.Orders
    //   ├─ Order #1001 - Delivered
    //   ├─ Order #1002 - In Progress
    //   └─ Order #1003 - Pending
    //
    // Usage:
    // var activeOrders = delivery.Orders
    //     .Where(o => o.Status == "InProgress")
    //     .ToList();
    //
    // var completedCount = delivery.Orders
    //     .Count(o => o.Status == "Delivered");
    // ------------------------------------------------------
    public virtual ICollection<Order>? Orders { get; set; }
        = new List<Order>();
}