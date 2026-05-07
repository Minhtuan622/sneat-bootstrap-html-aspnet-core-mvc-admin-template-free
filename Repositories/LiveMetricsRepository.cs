using AspnetCoreMvcFull.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AspnetCoreMvcFull.Repositories
{
  public class LiveMetricsRepository(IConfiguration config)
  {
    private readonly string? _connectionString = config.GetConnectionString("DefaultConnection");

    public async Task<IEnumerable<LiveMetric>> GetMetrics(AspnetCoreMvcFull.Models.DashboardFilterModel? filter = null)
    {
      await using var connection = new SqlConnection(_connectionString);

      var rows = await connection.QueryAsync<LiveMetric>(
        "sp_calculate_live_metrics",
        commandType: CommandType.StoredProcedure
      );

      var query = rows.AsQueryable();

      if (!string.IsNullOrWhiteSpace(filter?.ProjectName))
        query = query.Where(x => x.ProjectName.Contains(filter.ProjectName, StringComparison.OrdinalIgnoreCase));

      if (!string.IsNullOrWhiteSpace(filter?.PostId))
        query = query.Where(x => x.PostId.Contains(filter.PostId, StringComparison.OrdinalIgnoreCase));

      if (!string.IsNullOrWhiteSpace(filter?.Status))
      {
        query = filter.Status.ToLower() switch
        {
          "profit" => query.Where(x => x.EstimatedProfit >= 0),
          "loss" => query.Where(x => x.EstimatedProfit < 0),
          _ => query
        };
      }

      if (filter?.FromDate != null)
        query = query.Where(x => x.LastOrderAt >= filter.FromDate.Value.Date);

      if (filter?.ToDate != null)
        query = query.Where(x => x.LastOrderAt <= filter.ToDate.Value.Date.AddDays(1).AddTicks(-1));

      return query.ToList();
    }
  }
}
