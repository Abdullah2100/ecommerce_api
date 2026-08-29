using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
 
namespace api.domain.entity;

 

// ==========================================================
// GeneralSharedInfoWithId Abstract Base Class
// ----------------------------------------------------------
// This is the most foundational base class that provides
// a unique identifier for all entities. It serves as the
// root of the inheritance hierarchy for shared entities.
//
// Purpose:
// - Provides a standard primary key for all entities
// - Ensures consistent ID generation across the system
// - Enables polymorphism for shared entities
//
// Key Features:
// - Guid primary key (globally unique)
// - [Key] attribute for EF Core identification
// - Simple and lightweight
//
// Inheritance Chain:
// GeneralSharedInfoWithId
//     ↑
// GeneralSharedInfoWithCreatedAt
//     ↑
// GeneralSharedInfo
//
// Usage:
// This class is typically inherited by entities that
// only need an ID and no other common fields.
// ==========================================================
public abstract class GeneralSharedInfoWithId
{
    // ------------------------------------------------------
    // Primary Key
    // ------------------------------------------------------
    // Unique identifier for each entity record.
    // Marked with [Key] attribute to designate as primary key
    // in Entity Framework Core.
    //
    // Uses Guid (Globally Unique Identifier) instead of
    // auto-incrementing integers for:
    // - Distributed system support
    // - No database dependency for generation
    // - Global uniqueness across environments
    // - No collision risks in multi-tenant systems
    //
    // Example:
    // "f47ac10b-58cc-4372-a567-0e02b2c3d479"
    // "a8d7f9e2-5c3b-4a6d-8f1e-7b2c4d9e8f3a"
    //
    // Format: 8-4-4-4-12 hexadecimal digits
    // ------------------------------------------------------
    [Key]
    public Guid Id { get; set; }
}

// ==========================================================
// GeneralSharedInfoWithCreatedAt Abstract Base Class
// ----------------------------------------------------------
// Extends GeneralSharedInfoWithId by adding creation
// timestamp tracking. Provides audit trail for when
// records were created.
//
// Purpose:
// - Tracks when entities were created
// - Enables chronological sorting and filtering
// - Supports data analysis and reporting
// - Maintains audit trail
//
// Key Features:
// - Inherits Id from GeneralSharedInfoWithId
// - CreatedAt timestamp with [Column] attribute
// - Default value: DateTime.Now
// - Database column type: "Timestamp"
//
// Inheritance Chain:
// GeneralSharedInfoWithId
//     ↑
// GeneralSharedInfoWithCreatedAt (this class)
//     ↑
// GeneralSharedInfo
//
// Usage:
// This class is typically inherited by entities that
// need creation tracking but NOT update tracking.
// Examples: Audit logs, immutable records, event logs.
// ==========================================================
public abstract class GeneralSharedInfoWithCreatedAt : GeneralSharedInfoWithId
{
    // ------------------------------------------------------
    // Created At Timestamp
    // ------------------------------------------------------
    // Records the exact date and time when the entity was
    // first inserted into the database.
    //
    // Column Type: "Timestamp" in database
    // This may be mapped to DATETIME, DATETIME2, or
    // TIMESTAMP depending on the database provider.
    //
    // Default Value: DateTime.Now (local server time)
    // Recommendation: Use DateTime.UtcNow for consistency
    // across time zones.
    //
    // Importance:
    // - Audit trail for compliance
    // - Chronological analysis
    // - Data lifecycle management
    // - Troubleshooting and debugging
    //
    // Examples:
    // 2026-08-27 14:30:45.123
    // 2026-04-20 09:15:00.000
    //
    // Note: Consider using DateTime.UtcNow instead of
    // DateTime.Now to avoid time zone issues.
    // ------------------------------------------------------
    [Column(TypeName = "Timestamp")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

// ==========================================================
// GeneralShredInfo Abstract Base Class
// ----------------------------------------------------------
// Extends GeneralSharedInfoWithCreatedAt by adding update
// timestamp tracking. Provides complete audit trail for
// both creation and modification of records.
//
// Purpose:
// - Tracks when entities are created AND modified
// - Enables full audit trail
// - Supports change tracking and history
// - Facilitates data synchronization
//
// Key Features:
// - Inherits Id from GeneralSharedInfoWithId
// - Inherits CreatedAt from GeneralSharedInfoWithCreatedAt
// - UpdatedAt timestamp with [Column] attribute
// - Nullable to distinguish "never updated"
// - Database column type: "Timestamp"
//
// Inheritance Chain:
// GeneralSharedInfoWithId
//     ↑
// GeneralSharedInfoWithCreatedAt
//     ↑
// GeneralSharedInfo (this class)
//
// Usage:
// This class is typically inherited by entities that
// need BOTH creation AND update tracking.
// Examples: Mutable entities, configurable records,
// master data, transactional records that can be modified.
// ==========================================================
public abstract class GeneralShredInfo : GeneralSharedInfoWithCreatedAt
{
    // ------------------------------------------------------
    // Updated At Timestamp
    // ------------------------------------------------------
    // Records the date and time when the entity was last
    // modified. Nullable to distinguish between:
    // - null: Never updated (same as CreatedAt)
    // - DateTime: Last update timestamp
    //
    // Column Type: "Timestamp" in database
    // This may be mapped to DATETIME, DATETIME2, or
    // TIMESTAMP depending on the database provider.
    //
    // Nullable (?) meaning:
    // - Newly created records have null
    // - Updated records get a timestamp
    // - Can query for never-updated records
    //
    // Importance:
    // - Complete audit trail
    // - Change tracking
    // - Conflict detection
    // - Data synchronization
    //
    // Examples:
    // 2026-08-27 16:45:12.456 (last modified today)
    // null (never modified since creation)
    //
    // Usage:
    // entity.UpdatedAt = DateTime.UtcNow; // On update
    //
    // Note: Consider using DateTime.UtcNow instead of
    // DateTime.Now to avoid time zone issues.
    // ------------------------------------------------------
    [Column(TypeName = "Timestamp")]
    public DateTime? UpdatedAt { get; set; } = null;
}