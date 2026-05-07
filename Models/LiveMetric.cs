namespace AspnetCoreMvcFull.Models
{
  public class LiveMetric
  {
    public long Id { get; set; }

    public required string ProjectName { get; set; }

    public required string PostId { get; set; }

    public int TotalOrders { get; set; }

    public decimal TotalRevenue { get; set; }

    public decimal TotalSpend { get; set; }

    public decimal CloseRate { get; set; }

    public decimal DeliveryRate { get; set; }

    public decimal ImportCostRate { get; set; }

    public decimal ShippingCostRate { get; set; }

    public decimal CatseCost { get; set; }

    public DateTime? LastOrderAt { get; set; }

    public decimal EstimatedProfit
    {
      get
      {
        var actualRevenue = TotalRevenue * (CloseRate / 100) * (DeliveryRate / 100);
        var importCost = actualRevenue * (ImportCostRate / 100);
        var shippingCost = actualRevenue * (ShippingCostRate / 100);
        return actualRevenue - TotalSpend - importCost - shippingCost - CatseCost;
      }
    }
  }
}
