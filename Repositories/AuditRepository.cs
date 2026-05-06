using System.Text.Json;
using Dapper;
using Microsoft.Data.SqlClient;

namespace AspnetCoreMvcFull.Repositories;

public class AuditRepository(IConfiguration config)
{
  private SqlConnection GetConnection()
  {
    return new SqlConnection(config.GetConnectionString("DefaultConnection"));
  }

  public async Task Create(
    string actionName,
    string entityName,
    long? entityId,
    object? oldValues,
    object? newValues,
    string? createdBy = "system")
  {
    await using var conn = GetConnection();

    await conn.ExecuteAsync(
      """
      INSERT INTO audit_logs
      (
          action_name,
          entity_name,
          entity_id,
          old_values,
          new_values,
          created_by
      )
      VALUES
      (
          @actionName,
          @entityName,
          @entityId,
          @oldValues,
          @newValues,
          @createdBy
      )
      """,
      new
      {
        actionName,
        entityName,
        entityId,
        oldValues =
          oldValues == null
            ? null
            : JsonSerializer.Serialize(oldValues),

        newValues =
          newValues == null
            ? null
            : JsonSerializer.Serialize(newValues),

        createdBy
      }
    );
  }
}
