using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.domain.entity;

// ==========================================================
// User Entity Class
// ----------------------------------------------------------
// This class represents users of the system including customers,
// administrators, and store owners. Each record stores user
// credentials, personal information, and relationships.
//
// Inherits from GeneralShredInfo which provides:
// - Id (Guid primary key)
// - CreatedAt (creation timestamp)
// - UpdatedAt (modification timestamp) - Full audit trail
//
// Each object created from this class represents one row
// inside the Users table.
//
// Example:
// Id                   = "f47ac10b-58cc-4372-a567-0e02b2c3d479"
// Name                 = "John Doe"
// Phone                = "+1234567890"
// Email                = "john.doe@example.com"
// Password             = "hashed_password_here"
// IsBlocked            = false
// DeviceToken          = "ExponentPushToken[xxxxxxxxxx]"
// IsUser               = true
// Thumbnail            = "users/john_doe.jpg"
// CreatedAt            = 2026-08-27 14:30:00.000
// UpdatedAt            = null
// ==========================================================
public class User : GeneralShredInfo
{
    // ==========================================================
    // Personal Information
    // ==========================================================

    // ------------------------------------------------------
    // User Name
    // ------------------------------------------------------
    // The full name of the user.
    // Required field - cannot be null or empty.
    //
    // Examples:
    // "John Doe"
    // "Jane Smith"
    // "Mohammed Ahmed"
    // "Sarah Johnson"
    // "Dr. Ahmed Hassan"
    //
    // Use Cases:
    // - Display in UI
    // - Communication
    // - Personalization
    // - Reports and analytics
    // ------------------------------------------------------
    public string Name { get; set; }

    // ------------------------------------------------------
    // Phone Number
    // ------------------------------------------------------
    // The user's contact phone number.
    // Required field - cannot be null or empty.
    // Used for authentication and communication.
    //
    // Examples:
    // "+1234567890"
    // "+201234567890"
    // "+447911123456"
    //
    // Use Cases:
    // - Login/authentication
    // - SMS notifications
    // - Customer support
    // - Order communication
    // ------------------------------------------------------
    public string Phone { get; set; }

    // ------------------------------------------------------
    // Email Address
    // ------------------------------------------------------
    // The user's email address.
    // Required field - cannot be null or empty.
    // Used for authentication and communication.
    //
    // Examples:
    // "john.doe@example.com"
    // "user@company.com"
    // "support@domain.com"
    //
    // Use Cases:
    // - Login/authentication
    // - Email notifications
    // - Password reset
    // - Marketing communications
    // - Order confirmations
    // ------------------------------------------------------
    public string Email { get; set; }

    // ------------------------------------------------------
    // Password
    // ------------------------------------------------------
    // The user's password for authentication.
    // Required field - should be hashed/encrypted.
    //
    // ⚠️ IMPORTANT: Never store plain-text passwords!
    // This field should contain a hashed password.
    //
    // Security Best Practices:
    // - Use bcrypt, Argon2, or PBKDF2
    // - Add salt to each password
    // - Use strong hashing algorithms
    // - Regularly rotate hash algorithms
    //
    // Examples:
    // "$2a$11$abcdefghijklmnopqrstuvwxyz" (bcrypt hash)
    // "hashed_password_string" (hashed)
    //
    // Use Cases:
    // - Authentication
    // - Password verification
    // - Security validation
    // ------------------------------------------------------
    public string Password { get; set; }

    // ==========================================================
    // Status & Security
    // ==========================================================

    // ------------------------------------------------------
    // Block Status
    // ------------------------------------------------------
    // Indicates whether the user is blocked/suspended.
    // Default: false (user is active)
    //
    // When blocked:
    // - User cannot log in
    // - Cannot place orders
    // - Cannot perform any actions
    //
    // Use Cases:
    // - Suspension for policy violations
    // - Temporary account lock
    // - Permanent deactivation
    // - Fraud prevention
    // - Security concerns
    // ------------------------------------------------------
    public bool IsBlocked { get; set; } = false;

    // ------------------------------------------------------
    // User Role Flag
    // ------------------------------------------------------
    // Indicates the user's role in the system.
    // true = Normal user (customer)
    // false = Administrator
    //
    // Default: true (normal user)
    //
    // Use Cases:
    // - Access control
    // - Permission management
    // - Admin panel access
    // - Feature availability
    // - Reporting
    // ------------------------------------------------------
    public bool IsUser { get; set; } = true;

    // ------------------------------------------------------
    // Device Token
    // ------------------------------------------------------
    // Push notification token for the user's device.
    // Nullable - may be null if not registered.
    //
    // Examples:
    // "ExponentPushToken[xxxxxxxxxxxxxxxxxxxxxx]" (Expo)
    // "fcm-token-xxxxxxxxxxxxxxxx" (Firebase Cloud Messaging)
    // "APNS-token-xxxxxxxxxxxxxxxx" (Apple Push Notification)
    //
    // Use Cases:
    // - Push notifications
    // - Order updates
    // - Marketing notifications
    // - Real-time communication
    // ------------------------------------------------------
    public string? DeviceToken { get; set; } = null;

    // ------------------------------------------------------
    // User Thumbnail
    // ------------------------------------------------------
    // Profile picture/avatar of the user.
    // Nullable - may be null if not set.
    //
    // Examples:
    // "users/john_doe.jpg"
    // "https://cdn.example.com/users/jane_smith.png"
    // "uploads/users/avatar_123.webp"
    //
    // Use Cases:
    // - Profile display
    // - User identification
    // - Personalization
    // - UI elements
    // ------------------------------------------------------
    public string? Thumbnail { get; set; }

    // ==========================================================
    // Navigation Properties
    // ==========================================================

    // ------------------------------------------------------
    // Addresses Relationship
    // ------------------------------------------------------
    // Navigation property for user's saved addresses.
    // One user can have many addresses.
    // Initialized as empty collection.
    //
    // Examples:
    // User.Addresses
    //   ├─ Home Address (Current)
    //   ├─ Office Address
    //   └─ Work Address
    //
    // Usage:
    // var currentAddress = user.Addresses
    //     .FirstOrDefault(a => a.IsCurrent);
    // ------------------------------------------------------
    public virtual ICollection<Address> Addresses { get; set; }
        = new List<Address>();

    // ------------------------------------------------------
    // Categories Relationship
    // ------------------------------------------------------
    // Navigation property for user's categories.
    // One user can have many categories.
    // Initialized as empty collection.
    //
    // Examples:
    // User.Categories
    //   ├─ Electronics
    //   ├─ Clothing
    //   └─ Books
    //
    // Usage:
    // var categories = user.Categories
    //     .Where(c => !c.IsBlocked)
    //     .ToList();
    // ------------------------------------------------------
    public virtual ICollection<Category> Categories { get; set; }
        = new List<Category>();

    // ------------------------------------------------------
    // Orders Relationship
    // ------------------------------------------------------
    // Navigation property for user's orders.
    // One user can have many orders.
    // Initialized as empty collection.
    //
    // Examples:
    // User.Orders
    //   ├─ Order #1001 - $150.00
    //   ├─ Order #1002 - $75.00
    //   └─ Order #1003 - $200.00
    //
    // Usage:
    // var orderCount = user.Orders.Count;
    // var totalSpent = user.Orders
    //     .Where(o => o.Status == (int)EnOrderStatus.Delivered)
    //     .Sum(o => o.TotalPrice);
    // ------------------------------------------------------
    public virtual ICollection<Order> Orders { get; set; }
        = new List<Order>();

    // ------------------------------------------------------
    // Payment Types Relationship
    // ------------------------------------------------------
    // Navigation property for user's payment types.
    // One user can have many payment types.
    // Initialized with 0 capacity (not typical).
    //
    // Note: new List<PaymentType>(0) is unusual.
    // Typically: new List<PaymentType>()
    //
    // This collection allows admins to manage payment types.
    // ------------------------------------------------------
    public virtual ICollection<PaymentType> PaymentTypes { get; set; }
        = new List<PaymentType>(0);

    // ------------------------------------------------------
    // Store Relationship
    // ------------------------------------------------------
    // Navigation property to the user's store.
    // Nullable - may be null if user doesn't own a store.
    //
    // Examples:
    // User.Store.Name
    // User.Store.IsBlocked
    // User.Store.WallpaperImage
    //
    // Usage:
    // if (user.Store != null)
    // {
    //     var storeName = user.Store.Name;
    // }
    // ------------------------------------------------------
    public virtual Store? Store { get; set; } = null;

    // ------------------------------------------------------
    // Delivery Relationship
    // ------------------------------------------------------
    // Navigation property to user's delivery profile.
    // Nullable - may be null if user isn't a delivery person.
    //
    // Examples:
    // User.Delivery.IsAvailable
    // User.Delivery.DeviceToken
    // User.Delivery.Orders
    //
    // Usage:
    // if (user.Delivery != null)
    // {
    //     var isAvailable = user.Delivery.IsAvailable;
    // }
    // ------------------------------------------------------
    public virtual Delivery? Delivery { get; set; } = null;
}