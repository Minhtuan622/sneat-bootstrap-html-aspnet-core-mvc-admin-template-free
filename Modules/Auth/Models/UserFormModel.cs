using System.ComponentModel.DataAnnotations;

namespace AspnetCoreMvcFull.Models;

public class UserFormModel
{
  public long? Id { get; set; }

  [Required(ErrorMessage = "Username là bắt buộc")]
  [StringLength(50)]
  public string Username { get; set; } = string.Empty;

  [StringLength(200)]
  [Required(ErrorMessage = "Họ tên là bắt buộc")]
  public string FullName { get; set; } = string.Empty;

  [Required(ErrorMessage = "Vai trò là bắt buộc")]
  public string RoleName { get; set; } = UserRole.Viewer;

  public bool IsActive { get; set; } = true;

  [StringLength(100)]
  public string? Password { get; set; }
}
