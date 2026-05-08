using AspnetCoreMvcFull.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace AspnetCoreMvcFull.Repositories;

public class FacebookAdInsightRepository(IConfiguration config)
{
  private readonly string _conn = config.GetConnectionString("DefaultConnection")!;

  public async Task Create(FacebookAdInsight model)
  {
    await using var connection = new SqlConnection(_conn);

    const string sql =
      """
      INSERT INTO facebook_ad_insights
      (
          ad_id,
          campaign_name,
          adset_name,
          ad_name,
          spend,
          impressions,
          clicks,
          cpc,
          cpm,
          reach
      )
      VALUES
      (
          @AdId,
          @CampaignName,
          @AdsetName,
          @AdName,
          @Spend,
          @Impressions,
          @Clicks,
          @Cpc,
          @Cpm,
          @Reach
      )
      """;

    await connection.ExecuteAsync(sql, model);
  }

  public async Task DeleteTodayData()
  {
    await using var connection = new SqlConnection(_conn);

    const string sql =
      """
      DELETE FROM facebook_ad_insights
      WHERE CAST(synced_at AS DATE)
          = CAST(GETDATE() AS DATE)
      """;

    await connection.ExecuteAsync(sql);
  }
}
