using System.ComponentModel.DataAnnotations;

namespace api.domain.entity;

public class UserRefreshToken
{
    [Key]
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid Refresh { get; set; }
    public DateTime ExpireAt { get; set; }
}