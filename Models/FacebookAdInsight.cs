namespace AspnetCoreMvcFull.Models;

public class FacebookAdInsight
{
  public long Id { get; set; }

  public string AdId { get; set; } = "";

  public string CampaignName { get; set; } = "";

  public string AdsetName { get; set; } = "";

  public string AdName { get; set; } = "";

  public decimal Spend { get; set; }

  public long Impressions { get; set; }

  public long Clicks { get; set; }

  public decimal Cpc { get; set; }

  public decimal Cpm { get; set; }

  public long Reach { get; set; }

  public DateTime SyncedAt { get; set; }
}
