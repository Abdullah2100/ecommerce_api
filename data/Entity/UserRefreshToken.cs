using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.domain.entity;

// ==========================================================
// UserRefreshToken Entity Class
// ----------------------------------------------------------
// This class represents refresh tokens used for authentication
// and session management. Each record stores a refresh token
// associated with a user, enabling secure token refresh without
// requiring re-authentication.
//
// Each object created from this class represents one row
// inside the UserRefreshTokens table.
//
// Example:
// Id                   = "f47ac10b-58cc-4372-a567-0e02b2c3d479"
// UserId               = "b5c9f8d3-2a7e-4b1d-9f6c-8e4d3c2a1b5f"
// Refresh              = "a8d7f9e2-5c3b-4a6d-8f1e-7b2c4d9e8f3a"
// ExpireAt             = 2026-09-27 14:30:00.000
// Role                 = "User"
// ==========================================================
public class UserRefreshToken
{
    // ==========================================================
    // Primary Key
    // ==========================================================

    // ------------------------------------------------------
    // Token Identifier
    // ------------------------------------------------------
    // Unique identifier for the refresh token record.
    // Marked with [Key] attribute as the primary key.
    //
    // Uses Guid for global uniqueness and security.
    //
    // Example:
    // "f47ac10b-58cc-4372-a567-0e02b2c3d479"
    // ------------------------------------------------------
    [Key]
    public Guid Id { get; set; }

    // ==========================================================
    // User Association
    // ==========================================================

    // ------------------------------------------------------
    // User Identifier
    // ------------------------------------------------------
    // Unique identifier (GUID) of the user associated with
    // this refresh token.
    //
    // Required foreign key - cannot be null.
    // References the User entity.
    //
    // Example:
    // "b5c9f8d3-2a7e-4b1d-9f6c-8e4d3c2a1b5f" (user ID)
    //
    // Use Cases:
    // - Token validation
    // - User session management
    // - Security auditing
    // - Revoking tokens
    // ------------------------------------------------------
    public Guid UserId { get; set; }

    // ==========================================================
    // Token Information
    // ==========================================================

    // ------------------------------------------------------
    // Refresh Token Value
    // ------------------------------------------------------
    // The actual refresh token value.
    // Stored as a Guid for security and uniqueness.
    //
    // Required field - cannot be null.
    // Should be cryptographically secure random value.
    //
    // Example:
    // "a8d7f9e2-5c3b-4a6d-8f1e-7b2c4d9e8f3a"
    //
    // Security Best Practices:
    // - Use cryptographically secure random generation
    // - Token should be unique per session
    // - Token should be rotated on refresh
    // - Token should be stored securely (hashed in production)
    // ------------------------------------------------------
    public Guid Refresh { get; set; }

    // ------------------------------------------------------
    // Token Expiry
    // ------------------------------------------------------
    // The date and time when this refresh token expires.
    // After this time, the token is invalid and cannot be used.
    //
    // Required field - cannot be null.
    // Stored in UTC format for consistency.
    //
    // Examples:
    // 2026-09-27 14:30:00.000 (7 days from creation)
    // 2026-12-31 23:59:59.999 (end of year)
    // 2027-01-01 00:00:00.000 (new year)
    //
    // Typical Duration:
    // - Access Token: 15-30 minutes
    // - Refresh Token: 7-30 days
    // - Remember Me: 30-90 days
    //
    // Use Cases:
    // - Automatic token expiration
    // - Session management
    // - Security compliance
    // - User experience (reduced re-authentication)
    // ------------------------------------------------------
    public DateTime ExpireAt { get; set; }

    // ==========================================================
    // User Role Information
    // ==========================================================

    // ------------------------------------------------------
    // User Role
    // ------------------------------------------------------
    // The role of the user when the token was issued.
    // Used for authorization and permission validation.
    //
    // Nullable - may be null if role not specified.
    //
    // Examples:
    // "User"
    // "Admin"
    // "SuperAdmin"
    // "StoreOwner"
    // "DeliveryPerson"
    // null (no specific role)
    //
    // Use Cases:
    // - Authorization verification
    // - Permission checking
    // - Role-based access control (RBAC)
    // - Security auditing
    // ------------------------------------------------------
    public string? Role { get; set; }
}