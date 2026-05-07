using AspnetCoreMvcFull.Models;
using AspnetCoreMvcFull.Repositories;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace AspnetCoreMvcFull.Controllers;

[Authorize(Roles = "Admin")]
public class AuditController(IConfiguration config) : Controller
{
  public async Task<IActionResult> Index()
  {
    await using var conn = new SqlConnection(config.GetConnectionString("DefaultConnection"));
    var logs = await conn.QueryAsync<AuditLog>("SELECT TOP 300 id AS Id, action_name AS ActionName, entity_name AS EntityName, entity_id AS EntityId, old_values AS OldValues, new_values AS NewValues, created_by AS CreatedBy, created_at AS CreatedAt FROM audit_logs ORDER BY id DESC");
    return View(logs);
  }
}
