using AspnetCoreMvcFull.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace AspnetCoreMvcFull.Repositories;

public class ErrorLogRepository(IConfiguration config)
{
  private SqlConnection GetConnection() => new(config.GetConnectionString("DefaultConnection"));

  public async Task Create(string message, string? stackTrace, string source)
  {
    await using var conn = GetConnection();
    await conn.ExecuteAsync("INSERT INTO error_logs(message, stack_trace, source) VALUES(@message,@stackTrace,@source)", new { message, stackTrace, source });
  }

  public async Task<IEnumerable<ErrorLog>> GetAll(int top = 200)
  {
    await using var conn = GetConnection();
    return await conn.QueryAsync<ErrorLog>("SELECT TOP (@top) id AS Id, message AS Message, stack_trace AS StackTrace, source AS Source, created_at AS CreatedAt FROM error_logs ORDER BY id DESC", new { top });
  }
}
