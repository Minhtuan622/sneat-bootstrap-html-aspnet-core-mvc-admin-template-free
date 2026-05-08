using AspnetCoreMvcFull.Models;
using AspnetCoreMvcFull.Repositories;
using AspnetCoreMvcFull.Services;

namespace AspnetCoreMvcFull.Workers;

public class FacebookSyncWorker(
  IServiceProvider serviceProvider,
  ILogger<FacebookSyncWorker> logger
) : BackgroundService
{
  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    while (!stoppingToken.IsCancellationRequested)
    {
      try
      {
        using var scope = serviceProvider.CreateScope();

        var fb = scope.ServiceProvider.GetRequiredService<FacebookGraphService>();
        var repo = scope.ServiceProvider.GetRequiredService<FacebookAdInsightRepository>();

        logger.LogInformation("START FACEBOOK SYNC");

        var json = await fb.GetInsights();

        if (json.RootElement.TryGetProperty("data", out var data))
        {
          await repo.DeleteTodayData();
          var facebookAdInsight = data.EnumerateArray().Select(item => new FacebookAdInsight
            {
              CampaignName = item.GetProperty("campaign_name").GetString() ?? "",
              AdsetName = item.GetProperty("adset_name").GetString() ?? "",
              AdName = item.GetProperty("ad_name").GetString() ?? "",
              Spend = decimal.Parse(item.GetProperty("spend").GetString() ?? "0"),
              Impressions = long.Parse(item.GetProperty("impressions").GetString() ?? "0"),
              Clicks = long.Parse(item.GetProperty("clicks").GetString() ?? "0"),
              Cpc = decimal.Parse(item.GetProperty("cpc").GetString() ?? "0"),
              Cpm = decimal.Parse(item.GetProperty("cpm").GetString() ?? "0"),
              Reach = long.Parse(item.GetProperty("reach").GetString() ?? "0")
            }
          );

          foreach (var model in facebookAdInsight)
          {
            await repo.Create(model);
          }

          logger.LogInformation("FACEBOOK SYNC DONE");
        }
      }
      catch (Exception ex)
      {
        logger.LogError(ex, "FACEBOOK SYNC ERROR");
      }

      await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
    }
  }
}
