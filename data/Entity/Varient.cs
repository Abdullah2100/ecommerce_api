using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.domain.entity;

// ==========================================================
// Variant Entity Class
// ----------------------------------------------------------
// This class represents variant types/categories that can be
// applied to products. Examples include Size, Color, Material,
// Style, Flavor, etc. Each variant type can have multiple
// variant options (ProductVariant).
//
// Inherits from GeneralSharedInfoWithId which provides:
// - Id (Guid primary key)
// - NO CreatedAt (creation timestamp) - Not tracked
// - NO UpdatedAt (modification timestamp) - Not tracked
//
// Each object created from this class represents one row
// inside the Variants table.
//
// Example:
// Id                   = "f47ac10b-58cc-4372-a567-0e02b2c3d479"
// Name                 = "Size"
// ==========================================================
public class Variant : GeneralSharedInfoWithId
{
    // ==========================================================
    // Core Fields
    // ==========================================================

    // ------------------------------------------------------
    // Variant Name
    // ------------------------------------------------------
    // The display name of the variant type/group.
    // Required field - cannot be null or empty.
    // Default: empty string (should be set on creation)
    //
    // Examples:
    // "Size"
    // "Color"
    // "Material"
    // "Style"
    // "Flavor"
    // "Scent"
    // "Pattern"
    // "Finish"
    // "Edition"
    // "Capacity"
    // "Shape"
    // "Temperature"
    // "Size"
    // "Fabric"
    // "Fit"
    // "Collar Style"
    // "Sleeve Length"
    // "Neckline"
    // "Closure"
    // "Pocket Style"
    // "Heel Height"
    // "Shoe Width"
    // "Ring Size"
    // "Battery Life"
    // "Processor"
    // "RAM"
    // "Storage"
    // "Screen Size"
    // "Resolution"
    //
    // Use Cases:
    // - Grouping product variants
    // - Display in product selection UI
    // - Filtering and navigation
    // - Product configuration
    // - Inventory management
    // - Reports and analytics
    // ------------------------------------------------------
    public string Name { get; set; } = string.Empty;

    // ==========================================================
    // Navigation Properties
    // ==========================================================

    // ------------------------------------------------------
    // Product Variants Relationship
    // ------------------------------------------------------
    // Navigation property for all product variants that belong
    // to this variant type.
    //
    // One Variant can have many ProductVariant instances.
    // Initialized as empty collection to prevent null reference.
    //
    // Example:
    // Variant.ProductVariants
    //   ├─ Size: Small (ProductVariant)
    //   ├─ Size: Medium (ProductVariant)
    //   └─ Size: Large (ProductVariant)
    //
    // Usage:
    // var options = variant.ProductVariants
    //     .Where(pv => pv.ProductId == productId)
    //     .Select(pv => pv.Name)
    //     .ToList();
    // ------------------------------------------------------
    public virtual ICollection<ProductVariant> ProductVariants { get; set; }
        = new List<ProductVariant>();
}