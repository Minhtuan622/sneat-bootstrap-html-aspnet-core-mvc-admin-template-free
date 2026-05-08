using AspnetCoreMvcFull.Models;

namespace AspnetCoreMvcFull.Services
{
  public class ReportBuilderService
  {
    public string Build(LiveMetric m)
    {
      var actualRevenue =
        m.TotalRevenue
        * (m.CloseRate / 100)
        * (m.DeliveryRate / 100);

      var importCost =
        actualRevenue
        * (m.ImportCostRate / 100);

      var shippingCost =
        actualRevenue
        * (m.ShippingCostRate / 100);

      var profit =
        actualRevenue
        - m.TotalSpend
        - importCost
        - shippingCost
        - m.CatseCost;

      var roas =
        m.TotalSpend == 0
          ? 0
          : actualRevenue / m.TotalSpend;

      var cpo =
        m.TotalOrders == 0
          ? 0
          : m.TotalSpend / m.TotalOrders;

      return
        $"""
           {m.ProjectName}
           ----------------
           Post Id: {m.PostId}
           Lợi nhuận: {profit:N0}
           Tổng chi tiêu: {m.TotalSpend:N0}
           Doanh số: {m.TotalRevenue:N0}
           Doanh thu thực: {actualRevenue:N0}
           Tiền catse: {m.CatseCost:N0}
           CPM: {m.Cpm:N0}
           CPC: {m.Cpc:N0}
           Clicks: {m.Clicks:N0}
           Reach: {m.Reach:N0}
           ROAS: {roas:N2}
           CPO: {cpo:N0}
         """;
    }
  }
}
