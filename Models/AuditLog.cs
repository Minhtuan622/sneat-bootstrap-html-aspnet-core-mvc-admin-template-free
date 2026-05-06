namespace AspnetCoreMvcFull.Models;

public class AuditLog
{
  public long Id { get; set; }

  public string ActionName { get; set; }

  public string EntityName { get; set; }

  public long? EntityId { get; set; }

  public string? OldValues { get; set; }

  public string? NewValues { get; set; }

  public string? CreatedBy { get; set; }

  public DateTime CreatedAt { get; set; }
}
