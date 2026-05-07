using AspnetCoreMvcFull.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace AspnetCoreMvcFull.Repositories;

public class SystemSettingsRepository(IConfiguration config)
{
  private SqlConnection GetConnection() => new(config.GetConnectionString("DefaultConnection"));

  public async Task<IEnumerable<SystemSetting>> GetAll()
  {
    await using var conn = GetConnection();
    return await conn.QueryAsync<SystemSetting>("SELECT [key] AS [Key], [value] AS [Value] FROM system_settings ORDER BY [key]");
  }

  public async Task<string?> GetValue(string key)
  {
    await using var conn = GetConnection();
    return await conn.ExecuteScalarAsync<string?>("SELECT [value] FROM system_settings WHERE [key] = @key", new { key });
  }

  public async Task Upsert(string key, string value)
  {
    await using var conn = GetConnection();
    await conn.ExecuteAsync(@"MERGE system_settings AS target
USING (SELECT @key AS [key], @value AS [value]) AS source
ON target.[key] = source.[key]
WHEN MATCHED THEN UPDATE SET [value] = source.[value]
WHEN NOT MATCHED THEN INSERT ([key], [value]) VALUES (source.[key], source.[value]);", new { key, value });
  }
}
