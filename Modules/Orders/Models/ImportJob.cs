namespace AspnetCoreMvcFull.Models;

public class ImportJob
{
  public long Id { get; set; }
  public string FilePath { get; set; } = string.Empty;
  public string OriginalFileName { get; set; } = string.Empty;
  public string Status { get; set; } = "Queued";
  public int TotalRows { get; set; }
  public int ProcessedRows { get; set; }
  public int SuccessCount { get; set; }
  public int FailedCount { get; set; }
  public string? ErrorMessage { get; set; }
  public string CreatedBy { get; set; } = "system";
  public DateTime CreatedAt { get; set; }
  public DateTime? StartedAt { get; set; }
  public DateTime? FinishedAt { get; set; }
}
