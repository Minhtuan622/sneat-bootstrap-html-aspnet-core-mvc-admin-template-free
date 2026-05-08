using AspnetCoreMvcFull.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace AspnetCoreMvcFull.Repositories
{
  public class ReportLogRepository(IConfiguration config)
  {
    private readonly string _connectionString = config.GetConnectionString("DefaultConnection")!;

    public async Task<IEnumerable<ReportLog>> GetAll()
    {
      await using var connection = new SqlConnection(_connectionString);

      return await connection.QueryAsync<ReportLog>(
        """
        SELECT TOP 200 *
        FROM report_logs
        ORDER BY created_at DESC
        """
      );
    }

    public async Task Create(ReportLog model)
    {
      await using var connection = new SqlConnection(_connectionString);

      await connection.ExecuteAsync(
        """
        INSERT INTO report_logs
        (
            live_config_id,
            message,
            is_success,
            error_message,
            duration_ms
        )
        VALUES
        (
            @LiveConfigId,
            @Message,
            @IsSuccess,
            @ErrorMessage,
            @DurationMs
        )
        """,
        model
      );
    }
  }
}
