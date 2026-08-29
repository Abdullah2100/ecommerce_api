using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.domain.entity;

// ==========================================================
// Order Entity Class
// ----------------------------------------------------------
// This class represents customer orders placed in the system.
// Each record tracks an order's details including location,
// pricing, status, payment, delivery information, and items.
//
// Inherits from GeneralShredInfo which provides:
// - Id (Guid primary key)
// - CreatedAt (creation timestamp)
// - UpdatedAt (modification timestamp) - Full audit trail
//
// Each object created from this class represents one row
// inside the Orders table.
//
// Example:
// Id                   = "f47ac10b-58cc-4372-a567-0e02b2c3d479"
// UserId               = "b5c9f8d3-2a7e-4b1d-9f6c-8e4d3c2a1b5f"
// Longitude            = 30.0444
// Latitude             = 31.2357
// TotalPrice           = 15000 (stored as cents = $150.00)
// Symbol               = "$"
// Status               = 1 (Pending)
// PaymentTypeId        = "e7d3f4a2-8b1c-4e3d-9f5a-6c8b2d1e4f7a"
// DeliveryId           = null
// CreatedAt            = 2026-08-27 14:30:00.000
// ==========================================================
public class Order : GeneralShredInfo
{
    // ==========================================================
    // Location Information
    // ==========================================================

    // ------------------------------------------------------
    // Delivery Longitude
    // ------------------------------------------------------
    // The longitude coordinate for the delivery destination.
    // Used for mapping, route optimization, and delivery tracking.
    //
    // Required field - cannot be null.
    // Uses decimal for high precision.
    // Range: -180 to +180 degrees
    //
    // Examples:
    // 30.0444 (Cairo, Egypt)
    // -74.0060 (New York, USA)
    // 2.3522 (Paris, France)
    //
    // Use Cases:
    // - Map display
    // - Delivery route planning
    // - Distance calculations
    // - Geofencing
    // ------------------------------------------------------
    public decimal Longitude { get; set; }

    // ------------------------------------------------------
    // Delivery Latitude
    // ------------------------------------------------------
    // The latitude coordinate for the delivery destination.
    // Used for mapping, route optimization, and delivery tracking.
    //
    // Required field - cannot be null.
    // Uses decimal for high precision.
    // Range: -90 to +90 degrees
    //
    // Examples:
    // 31.2357 (Cairo, Egypt)
    // 40.7128 (New York, USA)
    // 48.8566 (Paris, France)
    //
    // Use Cases:
    // - Map display
    // - Delivery route planning
    // - Distance calculations
    // - Geofencing
    // ------------------------------------------------------
    public decimal Latitude { get; set; }

    // ==========================================================
    // Customer Information
    // ==========================================================

    // ------------------------------------------------------
    // User Identifier
    // ------------------------------------------------------
    // Unique identifier (GUID) of the customer who placed
    // this order. Links the order to the user account.
    //
    // Required foreign key - cannot be null.
    //
    // Example:
    // "b5c9f8d3-2a7e-4b1d-9f6c-8e4d3c2a1b5f" (user ID)
    //
    // Use Cases:
    // - Customer identification
    // - Order history
    // - User analytics
    // - Customer support
    // - Loyalty programs
    // ------------------------------------------------------
    public Guid UserId { get; set; }

    // ==========================================================
    // Pricing Information
    // ==========================================================

    // ------------------------------------------------------
    // Total Order Price
    // ------------------------------------------------------
    // The total price of the order including all items,
    // taxes, fees, and delivery charges.
    //
    // Type: long
    // Stored in the smallest currency unit (e.g., cents, piasters)
    // to avoid floating-point precision issues.
    //
    // Examples:
    // 15000 = $150.00 (if using cents)
    // 1000  = $10.00
    // 2999  = $29.99
    // 500   = $5.00
    //
    // Calculation:
    // TotalPrice = (Items.Sum(i => i.Price * i.Quantity) + 
    //               DistanceFee + Tax) * (1 - Discount)
    //
    // Use Cases:
    // - Payment processing
    // - Invoicing
    // - Reporting
    // - Analytics
    // ------------------------------------------------------
    public long TotalPrice { get; set; }

    // ------------------------------------------------------
    // Currency Symbol
    // ------------------------------------------------------
    // The currency symbol used for this order.
    // Required field - determines how prices are displayed.
    //
    // Examples:
    // "$" - US Dollar
    // "€" - Euro
    // "£" - British Pound
    // "ج.م" - Egyptian Pound
    //
    // Use Cases:
    // - Price display
    // - Receipt generation
    // - Currency conversion
    // - Financial reporting
    // ------------------------------------------------------
    public string Symbol { get; set; }

    // ==========================================================
    // Status & State
    // ==========================================================

    // ------------------------------------------------------
    // Order Status
    // ------------------------------------------------------
    // Current status of the order represented as an integer.
    // Maps to an enum for status management.
    //
    // Required field - cannot be null.
    //
    // Common Status Values:
    // 0 = Pending
    // 1 = Confirmed
    // 2 = Preparing
    // 3 = Ready for Delivery
    // 4 = In Transit
    // 5 = Delivered
    // 6 = Cancelled
    // 7 = Failed
    // 8 = Returned
    //
    // Use Cases:
    // - Order tracking
    // - Workflow management
    // - Status updates
    // - Notifications
    // - Reporting
    // ------------------------------------------------------
    public int Status { get; set; }

    // ------------------------------------------------------
    // Distance to User
    // ------------------------------------------------------
    // The distance from the store/restaurant to the user's
    // delivery location. Used for calculating delivery fees
    // and estimated delivery time.
    //
    // Type: int
    // Stored in meters or kilometers depending on configuration.
    // Default: 0
    //
    // Examples:
    // 5000 = 5 km
    // 1500 = 1.5 km
    // 300  = 300 meters
    // 0    = Distance not calculated yet
    //
    // Use Cases:
    // - Delivery fee calculation
    // - Estimated delivery time
    // - Route optimization
    // - Reporting analytics
    // ------------------------------------------------------
    public int DistanceToUser { get; set; } = 0;

    // ------------------------------------------------------
    // Distance Fee
    // ------------------------------------------------------
    // The delivery fee charged based on distance.
    // Calculated from DistanceToUser.
    //
    // Type: int
    // Stored in the smallest currency unit.
    // Default: 0
    //
    // Examples:
    // 500 = $5.00 delivery fee
    // 250 = $2.50 delivery fee
    // 0   = Free delivery
    //
    // Use Cases:
    // - Delivery pricing
    // - Order total calculation
    // - Reporting
    // - Analytics
    // ------------------------------------------------------
    public int DistanceFee { get; set; } = 0;

    // ------------------------------------------------------
    // Failure Flag
    // ------------------------------------------------------
    // Indicates whether the order delivery has failed.
    // Used for tracking failed deliveries for analytics
    // and customer support.
    //
    // Default: false (order is not failed)
    //
    // Use Cases:
    // - Failed delivery tracking
    // - Customer support
    // - Analytics
    // - Quality control
    // - Refund processing
    // ------------------------------------------------------
    public bool IsFail { get; set; } = false;

    // ==========================================================
    // Payment Information
    // ==========================================================

    // ------------------------------------------------------
    // Payment Type Identifier
    // ------------------------------------------------------
    // Unique identifier (GUID) of the payment method used
    // for this order.
    //
    // Required foreign key - cannot be null.
    // References the PaymentType entity.
    //
    // Examples:
    // "e7d3f4a2-8b1c-4e3d-9f5a-6c8b2d1e4f7a" (payment type ID)
    //
    // Payment Types:
    // - Cash on Delivery
    // - Credit Card
    // - Debit Card
    // - Mobile Wallet
    // - Bank Transfer
    // - Loyalty Points
    // ------------------------------------------------------
    public Guid PaymentTypeId { get; set; }

    // ==========================================================
    // Delivery Information
    // ==========================================================

    // ------------------------------------------------------
    // Delivery Identifier
    // ------------------------------------------------------
    // Unique identifier (GUID) of the delivery person assigned
    // to this order. Nullable for orders not yet assigned.
    //
    // Nullable - may be null if not assigned yet.
    // References the Delivery entity.
    //
    // Examples:
    // "a8d7f9e2-5c3b-4a6d-8f1e-7b2c4d9e8f3a" (delivery ID)
    // null (not assigned yet)
    //
    // Use Cases:
    // - Driver assignment
    // - Delivery tracking
    // - Performance monitoring
    // - Fleet management
    // ------------------------------------------------------
    public Guid? DeliveryId { get; set; } = null;

    // ==========================================================
    // Navigation Properties
    // ==========================================================

    // ------------------------------------------------------
    // Payment Type Relationship
    // ------------------------------------------------------
    // Navigation property to the payment method used.
    // Provides access to payment type details.
    //
    // Required navigation property (null! indicates it will
    // be populated by Entity Framework).
    //
    // Example:
    // Order.PaymentType.Name
    // Order.PaymentType.Code
    // ------------------------------------------------------
    public virtual PaymentType PaymentType { get; set; } = null!;

    // ------------------------------------------------------
    // User Relationship
    // ------------------------------------------------------
    // Navigation property to the customer who placed the order.
    // Provides access to user details.
    //
    // Required navigation property (null! indicates it will
    // be populated by Entity Framework).
    //
    // Example:
    // Order.User.FullName
    // Order.User.Phone
    // Order.User.Email
    // ------------------------------------------------------
    public virtual User User { get; set; } = null!;

    // ------------------------------------------------------
    // Delivery Relationship
    // ------------------------------------------------------
    // Navigation property to the delivery person assigned.
    // Nullable - may be null if not assigned yet.
    //
    // Example:
    // Order.DeliveredBy.User.FullName
    // Order.DeliveredBy.DeviceToken
    // ------------------------------------------------------
    public virtual Delivery? DeliveredBy { get; set; } = null;

    // ------------------------------------------------------
    // Order Items Relationship
    // ------------------------------------------------------
    // Navigation property for all items in this order.
    // Represents the products/services purchased.
    //
    // One order can have many items.
    // Initialized as empty collection to prevent null reference.
    //
    // Example:
    // Order.Items
    //   ├─ Product A: 2 x $10.00
    //   ├─ Product B: 1 x $25.00
    //   └─ Product C: 3 x $5.00
    //
    // Usage:
    // var subtotal = order.Items.Sum(i => i.Price * i.Quantity);
    // var itemCount = order.Items.Count;
    // ------------------------------------------------------
    public virtual ICollection<OrderItem> Items { get; set; }
        = new List<OrderItem>();
}