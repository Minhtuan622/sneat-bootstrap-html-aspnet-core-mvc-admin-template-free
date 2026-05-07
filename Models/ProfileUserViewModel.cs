using System.ComponentModel.DataAnnotations;

namespace AspnetCoreMvcFull.Models;

public class ProfileUserViewModel
{
  public string Username { get; set; } = string.Empty;

  [Required(ErrorMessage = "Họ tên là bắt buộc")]
  [StringLength(200)]
  public string FullName { get; set; } = string.Empty;

  public string RoleName { get; set; } = string.Empty;
  public string? AvatarPath { get; set; }
}
