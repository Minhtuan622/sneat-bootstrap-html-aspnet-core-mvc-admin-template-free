namespace AspnetCoreMvcFull.Models;

public class ErrorLog
{
  public long Id { get; set; }
  public string Message { get; set; } = string.Empty;
  public string? StackTrace { get; set; }
  public string Source { get; set; } = string.Empty;
  public DateTime CreatedAt { get; set; }
}
