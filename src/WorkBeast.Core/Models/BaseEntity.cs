namespace WorkBeast.Core.Models;

public abstract class BaseEntity
{
    public int Oid { get; set; } = 0;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
    public bool IsSystem { get; set; } = false; // Indicates if the entity is a system entity and cannot be deleted

}