using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.domain.entity;

// ==========================================================
// GeneralSetting Entity Class
// ----------------------------------------------------------
// This class represents system-wide configuration settings
// or application parameters. Each record stores a key-value
// pair for various configuration options used throughout
// the application.
//
// Inherits from GeneralShredInfo which provides:
// - Id (Guid primary key)
// - CreatedAt (creation timestamp)
// - UpdatedAt (modification timestamp) - Full audit trail
//
// Each object created from this class represents one row
// inside the GeneralSettings table.
//
// Example:
// Id                   = "f47ac10b-58cc-4372-a567-0e02b2c3d479"
// Name                 = "VAT_Rate"
// Value                = 15.00
// CreatedAt            = 2026-08-27 14:30:00.000
// UpdatedAt            = null
// ==========================================================
public class GeneralSetting : GeneralShredInfo
{
    // ==========================================================
    // Setting Information
    // ==========================================================

    // ------------------------------------------------------
    // Setting Name (Key)
    // ------------------------------------------------------
    // The unique identifier or key for the setting.
    // This is the name used to reference the setting in code.
    //
    // Required field - cannot be null or empty.
    // Should be unique across all settings.
    //
    // Naming Conventions:
    // - Use UPPER_SNAKE_CASE for constants
    // - Use PascalCase for categories
    // - Include category prefix for organization
    // - Be descriptive and self-documenting
    //
    // Examples:
    // "VAT_Rate" - Value Added Tax rate
    // "Currency_Default" - Default currency code
    // "Max_Login_Attempts" - Security setting
    // "Session_Timeout_Minutes" - Session duration
    // "Enable_2FA" - Two-factor authentication flag
    // "Maintenance_Mode" - System maintenance flag
    // "Email_Verification_Required" - Registration setting
    // "Max_Order_Items" - Business rule limit
    // "Shipping_Threshold" - Free shipping threshold
    // "Inventory_Alert_Level" - Stock notification level
    // ------------------------------------------------------
    public string Name { get; set; }

    // ------------------------------------------------------
    // Setting Value
    // ------------------------------------------------------
    // The actual value of the setting.
    // Stored as decimal to support numeric, boolean,
    // and percentage values.
    //
    // Required field - must have a value.
    // Use decimal for precision in financial calculations.
    //
    // Interpretation of Value:
    // - Numeric: The actual number (e.g., 15.00)
    // - Percentage: Represented as decimal (e.g., 0.15 for 15%)
    // - Boolean: 1 for true, 0 for false
    // - Count/Limit: The actual number (e.g., 100)
    // - Rate: The actual rate value (e.g., 1.75 for exchange rate)
    //
    // Examples:
    // 15.00   = 15% VAT rate
    // 0.85    = 85% of original value
    // 1.75    = Exchange rate 1.75
    // 30.00   = 30 minutes timeout
    // 1.00    = Enabled/True
    // 0.00    = Disabled/False
    // 1000.00 = Threshold amount
    //
    // Use Cases:
    // - Configuration values
    // - Business rules
    // - System parameters
    // - Feature flags
    // - Rate multipliers
    // - Threshold values
    // ------------------------------------------------------
    public decimal Value { get; set; }

    // ==========================================================
    // Computed Properties (Optional Enhancements)
    // ==========================================================

    /*
    // Boolean helper property
    [NotMapped]
    public bool BoolValue => Value == 1;

    // Integer helper property
    [NotMapped]
    public int IntValue => (int)Value;

    // Percentage helper property
    [NotMapped]
    public decimal PercentageValue => Value / 100;
    */
}