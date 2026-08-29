using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.domain.entity;

// ==========================================================
// PaymentType Entity Class
// ----------------------------------------------------------
// This class represents payment methods available in the system.
// Each record defines a payment type with its name, thumbnail,
// and configuration flags.
//
// Inherits from GeneralShredInfo which provides:
// - Id (Guid primary key)
// - CreatedAt (creation timestamp)
// - UpdatedAt (modification timestamp) - Full audit trail
//
// Each object created from this class represents one row
// inside the PaymentTypes table.
//
// Example:
// Id                   = "f47ac10b-58cc-4372-a567-0e02b2c3d479"
// Name                 = "Cash on Delivery"
// IsHashCheckOperation = false
// Thumbnail            = "payment/cash.png"
// UserId               = "b5c9f8d3-2a7e-4b1d-9f6c-8e4d3c2a1b5f"
// CreatedAt            = 2026-08-27 14:30:00.000
// UpdatedAt            = null
// ==========================================================
public class PaymentType : GeneralShredInfo
{
    // ==========================================================
    // Payment Method Information
    // ==========================================================

    // ------------------------------------------------------
    // Payment Type Name
    // ------------------------------------------------------
    // The display name of the payment method shown to users.
    // Required field - cannot be null or empty.
    //
    // Examples:
    // "Cash on Delivery"
    // "Credit Card"
    // "Debit Card"
    // "Mobile Wallet"
    // "Bank Transfer"
    // "PayPal"
    // "Apple Pay"
    // "Google Pay"
    // "Stripe"
    // "PayFast"
    // "InstaPay"
    // "Vodafone Cash"
    // "Fawry"
    // ------------------------------------------------------
    public string Name { get; set; }

    // ------------------------------------------------------
    // Hash Check Operation Flag
    // ------------------------------------------------------
    // Indicates whether this payment type requires hash-based
    // verification or checksum validation.
    //
    // Default: false (no hash check required)
    //
    // Use Cases:
    // - Payment gateway integration
    // - Security verification
    // - Transaction validation
    // - Fraud prevention
    //
    // When true, the system will perform additional hash
    // verification before processing payments.
    //
    // Examples:
    // false = No hash verification needed (e.g., Cash on Delivery)
    // true  = Hash verification required (e.g., Credit Card, PayPal)
    // ------------------------------------------------------
    public bool IsHashCheckOperation { get; set; }

    // ------------------------------------------------------
    // Payment Type Thumbnail
    // ------------------------------------------------------
    // Image path, URL, or icon representation of the payment type.
    // Used for UI display in payment selection screens.
    //
    // Required field - cannot be null or empty.
    //
    // String type for maximum flexibility:
    // - File path: "payment/cash.png"
    // - URL: "https://cdn.example.com/payment/credit-card.svg"
    // - Base64: "data:image/png;base64,iVBORw0KGgo..."
    // - Icon name: "fa-credit-card"
    //
    // Examples:
    // "payment/cash_icon.png"
    // "payment/credit_card.svg"
    // "payment/mobile_wallet.jpg"
    // "https://cdn.storage.com/payment/paypal.png"
    // "payment_types/bank_transfer.svg"
    //
    // Use Cases:
    // - Display in payment selection UI
    // - Branding and recognition
    // - Mobile app icons
    // - Receipts and invoices
    // ------------------------------------------------------
    public string Thumbnail { get; set; }

    // ==========================================================
    // Foreign Keys
    // ==========================================================

    // ------------------------------------------------------
    // User Identifier
    // ------------------------------------------------------
    // Unique identifier (GUID) of the user who created or
    // owns this payment type.
    //
    // Required foreign key - cannot be null.
    // Links the payment type to the user who configured it.
    //
    // Example:
    // "b5c9f8d3-2a7e-4b1d-9f6c-8e4d3c2a1b5f" (user ID)
    //
    // Use Cases:
    // - User-specific payment methods
    // - Store-specific configurations
    // - Audit trail
    // - Ownership verification
    // - Multi-tenant payment types
    // ------------------------------------------------------
    public Guid UserId { get; set; }

    // ==========================================================
    // Navigation Properties
    // ==========================================================

    // ------------------------------------------------------
    // User Relationship
    // ------------------------------------------------------
    // Navigation property to the user who owns this payment type.
    // Provides access to user details and preferences.
    //
    // Nullable - may be null if user is deleted or not loaded.
    // Default initialized to null.
    //
    // Example:
    // PaymentType.User.FullName
    // PaymentType.User.Phone
    // PaymentType.User.Email
    //
    // Usage:
    // var ownerName = paymentType.User?.FullName ?? "Unknown";
    // ------------------------------------------------------
    public virtual User? User { get; set; } = null;
}