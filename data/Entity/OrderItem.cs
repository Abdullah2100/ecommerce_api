using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.domain.entity;

// ==========================================================
// EnOrderItemStatus Enumeration
// ----------------------------------------------------------
// Defines the possible states of an order item during its
// lifecycle from order placement to delivery.
// ==========================================================
public enum EnOrderItemStatus
{
    // ------------------------------------------------------
    // Cancelled
    // ------------------------------------------------------
    // The item has been cancelled/removed from the order.
    // Reasons: User cancellation, out of stock, store cancellation.
    // ------------------------------------------------------
    Cancelled,

    // ------------------------------------------------------
    // InProgress
    // ------------------------------------------------------
    // The item is currently being prepared or processed.
    // This is the default state when the order is placed.
    // ------------------------------------------------------
    InProgress,

    // ------------------------------------------------------
    // Excepted
    // ------------------------------------------------------
    // (Note: Likely a typo for "Accepted")
    // The item has been accepted and confirmed by the store.
    // Store has validated that they can fulfill this item.
    // ------------------------------------------------------
    Excepted,

    // ------------------------------------------------------
    // ReceivedByDelivery
    // ------------------------------------------------------
    // The item has been picked up by the delivery person.
    // It is now in transit to the customer.
    // ------------------------------------------------------
    ReceivedByDelivery
}

// ==========================================================
// OrderItem Entity Class
// ----------------------------------------------------------
// This class represents individual items within an order.
// Each record tracks a specific product, its quantity,
// price, variants, and current status.
//
// Inherits from GeneralShredInfo which provides:
// - Id (Guid primary key)
// - CreatedAt (creation timestamp)
// - UpdatedAt (modification timestamp) - Full audit trail
//
// Each object created from this class represents one row
// inside the OrderItems table.
//
// Example:
// Id                   = "f47ac10b-58cc-4372-a567-0e02b2c3d479"
// OrderId              = "a8d7f9e2-5c3b-4a6d-8f1e-7b2c4d9e8f3a"
// ProductId            = "b5c9f8d3-2a7e-4b1d-9f6c-8e4d3c2a1b5f"
// StoreId              = "e7d3f4a2-8b1c-4e3d-9f5a-6c8b2d1e4f7a"
// Price                = 29.99
// Quantity             = 2
// Status               = EnOrderItemStatus.InProgress
// CreatedAt            = 2026-08-27 14:30:00.000
// ==========================================================
public class OrderItem : GeneralShredInfo
{
    // ==========================================================
    // Core Fields
    // ==========================================================

    // ------------------------------------------------------
    // Order Identifier
    // ------------------------------------------------------
    // Unique identifier (GUID) of the parent order.
    // Links this item to its associated order.
    //
    // Required foreign key - cannot be null.
    // References the Order entity.
    //
    // Example:
    // "a8d7f9e2-5c3b-4a6d-8f1e-7b2c4d9e8f3a" (order ID)
    // ------------------------------------------------------
    public Guid OrderId { get; set; }

    // ------------------------------------------------------
    // Product Identifier
    // ------------------------------------------------------
    // Unique identifier (GUID) of the product being ordered.
    // Links to the product catalog.
    //
    // Required foreign key - cannot be null.
    // References the Product entity.
    //
    // Example:
    // "b5c9f8d3-2a7e-4b1d-9f6c-8e4d3c2a1b5f" (product ID)
    // ------------------------------------------------------
    public Guid ProductId { get; set; }

    // ------------------------------------------------------
    // Store Identifier
    // ------------------------------------------------------
    // Unique identifier (GUID) of the store fulfilling
    // this item. Different items in the same order may
    // come from different stores.
    //
    // Required foreign key - cannot be null.
    // References the Store entity.
    //
    // Example:
    // "e7d3f4a2-8b1c-4e3d-9f5a-6c8b2d1e4f7a" (store ID)
    //
    // Use Cases:
    // - Multi-store orders
    // - Store-specific inventory
    // - Store-specific pricing
    // - Store-specific fulfillment
    // ------------------------------------------------------
    public Guid StoreId { get; set; }

    // ==========================================================
    // Pricing & Quantity
    // ==========================================================

    // ------------------------------------------------------
    // Item Price
    // ------------------------------------------------------
    // The price per unit for this item at the time of order.
    // Stored as decimal for precise financial calculations.
    //
    // Required field - cannot be null.
    // This is a snapshot of the product price at order time.
    // Should include any item-level discounts or promotions.
    //
    // Examples:
    // 29.99 = $29.99 per item
    // 15.50 = $15.50 per item
    // 99.99 = $99.99 per item
    //
    // Usage:
    // var lineTotal = price * quantity;
    // ------------------------------------------------------
    public decimal Price { get; set; }

    // ------------------------------------------------------
    // Item Quantity
    // ------------------------------------------------------
    // The number of units of this product ordered.
    // Must be a positive integer.
    //
    // Required field - cannot be null or zero.
    // Minimum value: 1
    //
    // Examples:
    // 1 = Single item
    // 2 = Two units
    // 5 = Five units
    //
    // Usage:
    // var totalPrice = Price * Quantity;
    // var subtotal = order.Items.Sum(i => i.Price * i.Quantity);
    // ------------------------------------------------------
    public int Quantity { get; set; }

    // ==========================================================
    // Status Tracking
    // ==========================================================

    // ------------------------------------------------------
    // Item Status
    // ------------------------------------------------------
    // Current status of this specific order item.
    // Tracks the item's progress through fulfillment.
    //
    // Default: EnOrderItemStatus.InProgress
    //
    // Status Flow:
    // InProgress → Excepted → ReceivedByDelivery → (Delivered)
    // InProgress → Cancelled (if cancelled before fulfillment)
    // Excepted → Cancelled (if cancelled after acceptance)
    //
    // Status Values:
    // - Cancelled: Item was cancelled
    // - InProgress: Item is being processed
    // - Excepted: Item was accepted by store (typo: "Accepted")
    // - ReceivedByDelivery: Item picked up by delivery
    //
    // Use Cases:
    // - Order tracking
    // - Fulfillment workflow
    // - Status notifications
    // - Analytics and reporting
    // ------------------------------------------------------
    public virtual EnOrderItemStatus Status { get; set; } = EnOrderItemStatus.InProgress;

    // ==========================================================
    // Navigation Properties
    // ==========================================================

    // ------------------------------------------------------
    // Order Relationship
    // ------------------------------------------------------
    // Navigation property to the parent order.
    // Provides access to order-level information.
    //
    // Required navigation property (null! indicates it will
    // be populated by Entity Framework).
    //
    // Example:
    // OrderItem.Order.TotalPrice
    // OrderItem.Order.Status
    // OrderItem.Order.User.FullName
    // ------------------------------------------------------
    public virtual Order Order { get; set; } = null!;

    // ------------------------------------------------------
    // Store Relationship
    // ------------------------------------------------------
    // Navigation property to the store fulfilling this item.
    // Provides access to store details and configuration.
    //
    // Required navigation property (null! indicates it will
    // be populated by Entity Framework).
    //
    // Example:
    // OrderItem.Store.StoreName
    // OrderItem.Store.Address
    // OrderItem.Store.Currency
    // ------------------------------------------------------
    public virtual Store Store { get; set; } = null!;

    // ------------------------------------------------------
    // Product Relationship
    // ------------------------------------------------------
    // Navigation property to the product being ordered.
    // Provides access to product details and catalog info.
    //
    // Required navigation property (null! indicates it will
    // be populated by Entity Framework).
    //
    // Example:
    // OrderItem.Product.Name
    // OrderItem.Product.Description
    // OrderItem.Product.Image
    // ------------------------------------------------------
    public virtual Product Product { get; set; } = null!;

    // ------------------------------------------------------
    // Product Variants Relationship
    // ------------------------------------------------------
    // Navigation property for all variants of this product
    // in this order item. Represents product options like
    // size, color, flavor, etc.
    //
    // One order item can have many variants.
    // Initialized as empty collection to prevent null reference.
    //
    // Example:
    // OrderItem.OrderProductsVariants
    //   ├─ Size: Large (+$2.00)
    //   ├─ Color: Red (+$0.00)
    //   └─ Extra Cheese: Yes (+$1.50)
    //
    // Usage:
    // var totalVariantsPrice = orderItem.OrderProductsVariants
    //     .Sum(v => v.PriceAdjustment ?? 0);
    // ------------------------------------------------------
    public virtual ICollection<OrderProductsVariant> OrderProductsVariants { get; set; }
        = new List<OrderProductsVariant>();

    // ==========================================================
    // Computed Properties (Recommended)
    // ==========================================================

    /*
    [NotMapped]
    public decimal LineTotal => Price * Quantity;

    [NotMapped]
    public decimal Subtotal => LineTotal + OrderProductsVariants
        .Sum(v => v.PriceAdjustment ?? 0);

    [NotMapped]
    public bool IsActive => Status != EnOrderItemStatus.Cancelled;

    [NotMapped]
    public bool IsDelivered => Status == EnOrderItemStatus.ReceivedByDelivery;

    [NotMapped]
    public string StatusDisplay => Status switch
    {
        EnOrderItemStatus.InProgress => "In Progress",
        EnOrderItemStatus.Excepted => "Accepted",
        EnOrderItemStatus.ReceivedByDelivery => "Picked Up",
        EnOrderItemStatus.Cancelled => "Cancelled",
        _ => Status.ToString()
    };
    */
}