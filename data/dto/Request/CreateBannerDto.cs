using System.ComponentModel.DataAnnotations;

namespace data.dto.Request;

public record CreateBannerDto ( [Required]  byte[] Image,[Required] DateTime EndAt);