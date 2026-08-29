using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.domain.entity;

// ==========================================================
// Currency Entity Class
// ----------------------------------------------------------
// This class represents currency configurations used within
// the system. Each record defines a currency with its name,
// value, symbol, and default status.
//
// Inherits from GeneralShredInfo which provides:
// - Id (Guid primary key)
// - CreatedAt (creation timestamp)
// - UpdatedAt (modification timestamp) - Full audit trail
//
// Each object created from this class represents one row
// inside the Currencies table.
//
// Example:
// Id                   = "f47ac10b-58cc-4372-a567-0e02b2c3d479"
// Name                 = "US Dollar"
// Value                = 1
// Symbol               = "$"
// IsDefault            = true
// CreatedAt            = 2026-08-27 14:30:00.000
// UpdatedAt            = null
// ==========================================================
public class Currency : GeneralShredInfo
{
    // ==========================================================
    // Basic Information
    // ==========================================================

    // ------------------------------------------------------
    // Currency Name
    // ------------------------------------------------------
    // The full name of the currency.
    // Required field - cannot be null or empty.
    //
    // Examples:
    // "US Dollar"
    // "Euro"
    // "British Pound"
    // "Japanese Yen"
    // "Egyptian Pound"
    // "Saudi Riyal"
    // "UAE Dirham"
    // "Kuwaiti Dinar"
    // "Indian Rupee"
    // "Chinese Yuan"
    //
    // Use Cases:
    // - Display in UI
    // - Currency selection
    // - Reporting
    // - Localization
    // ------------------------------------------------------
    public string Name { get; set; }

    // ------------------------------------------------------
    // Currency Value (Exchange Rate)
    // ------------------------------------------------------
    // The exchange rate or value of this currency relative to
    // the base/default currency.
    //
    // Type: int (integer)
    // - 1 = Base currency (IsDefault = true)
    // - > 1 = Currency with higher value
    // - < 1 = Currency with lower value (use decimal for precision)
    //
    // Examples:
    // 1.00   = US Dollar (base currency)
    // 0.85   = Euro (1 USD = 0.85 EUR)
    // 0.73   = British Pound (1 USD = 0.73 GBP)
    // 109.50 = Japanese Yen (1 USD = 109.50 JPY)
    // 15.75  = Egyptian Pound (1 USD = 15.75 EGP)
    // 3.75   = Saudi Riyal (1 USD = 3.75 SAR)
    // 3.67   = UAE Dirham (1 USD = 3.67 AED)
    //
    // Note: Using int may cause precision issues.
    // Consider using decimal for better precision.
    // ------------------------------------------------------
    public int Value { get; set; }

    // ------------------------------------------------------
    // Currency Symbol
    // ------------------------------------------------------
    // The symbol used to represent the currency.
    // Stored as VARCHAR(10) in the database.
    //
    // Required field - cannot be null or empty.
    //
    // Examples:
    // "$"    - US Dollar
    // "€"    - Euro
    // "£"    - British Pound
    // "¥"    - Japanese Yen
    // "ج.م"  - Egyptian Pound
    // "ر.س"  - Saudi Riyal
    // "د.إ"  - UAE Dirham
    // "د.ك"  - Kuwaiti Dinar
    // "₹"    - Indian Rupee
    // "¥"    - Chinese Yuan
    //
    // Use Cases:
    // - Currency display
    // - Formatting amounts
    // - Price display
    // - Financial reports
    // ------------------------------------------------------
    [Column(TypeName = "varchar(10)")]
    public string Symbol { get; set; }

    // ==========================================================
    // Default Status
    // ==========================================================

    // ------------------------------------------------------
    // Default Currency Flag
    // ------------------------------------------------------
    // Indicates whether this currency is the default/base
    // currency for the system.
    //
    // Default: false
    // Only one currency should have IsDefault = true.
    //
    // Use Cases:
    // - Base currency for exchange rates
    // - Default display currency
    // - Primary reporting currency
    // - Base for conversions
    //
    // Business Rules:
    // - Only one currency can be default
    // - Default currency has Value = 1
    // - Cannot delete default currency
    // - All other currencies are relative to default
    //
    // Examples:
    // true  = This is the base currency
    // false = This is a secondary currency
    // ------------------------------------------------------
    public bool IsDefault { get; set; } = false;
}