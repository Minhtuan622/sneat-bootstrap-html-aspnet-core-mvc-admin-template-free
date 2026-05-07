namespace AspnetCoreMvcFull.Models;

public class DashboardFilterModel
{
  public string? ProjectName { get; set; }
  public string? PostId { get; set; }
  public DateTime? FromDate { get; set; }
  public DateTime? ToDate { get; set; }
  public string? Status { get; set; }
}
