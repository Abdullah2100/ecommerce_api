using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.domain.entity;

// ==========================================================
// ReseatPasswordOtp Entity Class
// ----------------------------------------------------------
// This class represents One-Time Password (OTP) records for
// password reset functionality. Each record stores an OTP
// associated with a user's email for secure password reset.
//
// Inherits from GeneralSharedInfoWithCreatedAt which provides:
// - Id (Guid primary key)
// - CreatedAt (creation timestamp)
// - NO UpdatedAt field (OTPs are immutable once created)
//
// Each object created from this class represents one row
// inside the ReseatPasswordOtps table.
//
// Example:
// Id                   = "f47ac10b-58cc-4372-a567-0e02b2c3d479"
// Email                = "user@example.com"
// Otp                  = "123456"
// IsValidated          = false
// CreatedAt            = 2026-08-27 14:30:00.000
// ==========================================================
public class ReseatPasswordOtp : GeneralSharedInfoWithCreatedAt
{
    // ==========================================================
    // Core Fields
    // ==========================================================

    // ------------------------------------------------------
    // User Email
    // ------------------------------------------------------
    // The email address of the user requesting password reset.
    // Required field - cannot be null or empty.
    //
    // Examples:
    // "john.doe@example.com"
    // "user@company.com"
    // "support@domain.com"
    //
    // Use Cases:
    // - Identify the user requesting password reset
    // - Send OTP to this email
    // - Validate user identity
    // - Prevent unauthorized access
    // ------------------------------------------------------
    public string Email { get; set; }

    // ------------------------------------------------------
    // One-Time Password (OTP)
    // ------------------------------------------------------
    // The actual OTP code sent to the user's email.
    // Required field - cannot be null or empty.
    //
    // Typically a 4-6 digit numeric code.
    // Should be hashed in production for security.
    //
    // Examples:
    // "123456"
    // "789012"
    // "4567"
    // "ABCDEF" (if alphanumeric)
    //
    // Use Cases:
    // - Verify user identity
    // - Security validation
    // - Password reset authorization
    //
    // Security Best Practices:
    // - OTP should expire after a short time (5-15 minutes)
    // - OTP should be one-time use only
    // - Consider hashing OTP before storing
    // ------------------------------------------------------
    public string Otp { get; set; }

    // ------------------------------------------------------
    // Validation Flag
    // ------------------------------------------------------
    // Indicates whether this OTP has been validated/used.
    // Once validated, it cannot be reused.
    //
    // Default: false (not yet validated)
    // Once true, the OTP is considered consumed.
    //
    // Examples:
    // false = OTP is valid and can be used
    // true  = OTP has been used and is invalid
    //
    // Use Cases:
    // - Prevent OTP reuse
    // - Security validation
    // - Audit and tracking
    // - OTP lifecycle management
    // ------------------------------------------------------
    public bool IsValidated { get; set; } = false;
}