using AspnetCoreMvcFull.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace AspnetCoreMvcFull.Repositories;

public class ImportJobRepository(IConfiguration config)
{
  private SqlConnection GetConnection() => new(config.GetConnectionString("DefaultConnection"));

  public async Task<long> Create(string filePath, string originalFileName, string createdBy)
  {
    await using var conn = GetConnection();
    return await conn.ExecuteScalarAsync<long>(@"INSERT INTO import_jobs(file_path, original_file_name, status, created_by) OUTPUT INSERTED.id VALUES(@filePath,@originalFileName,'Queued',@createdBy)", new { filePath, originalFileName, createdBy });
  }

  public async Task<ImportJob?> GetQueuedJob()
  {
    await using var conn = GetConnection();
    return await conn.QueryFirstOrDefaultAsync<ImportJob>("SELECT TOP 1 * FROM import_jobs WHERE status = 'Queued' ORDER BY id");
  }

  public async Task Start(long id, int totalRows)
  {
    await using var conn = GetConnection();
    await conn.ExecuteAsync("UPDATE import_jobs SET status='Processing', started_at=GETUTCDATE(), total_rows=@totalRows WHERE id=@id", new { id, totalRows });
  }

  public async Task UpdateProgress(long id, int processedRows, int successCount, int failedCount)
  {
    await using var conn = GetConnection();
    await conn.ExecuteAsync("UPDATE import_jobs SET processed_rows=@processedRows, success_count=@successCount, failed_count=@failedCount WHERE id=@id", new { id, processedRows, successCount, failedCount });
  }

  public async Task Complete(long id, int successCount, int failedCount)
  {
    await using var conn = GetConnection();
    await conn.ExecuteAsync("UPDATE import_jobs SET status='Completed', finished_at=GETUTCDATE(), success_count=@successCount, failed_count=@failedCount, processed_rows=total_rows WHERE id=@id", new { id, successCount, failedCount });
  }

  public async Task Fail(long id, string error)
  {
    await using var conn = GetConnection();
    await conn.ExecuteAsync("UPDATE import_jobs SET status='Failed', finished_at=GETUTCDATE(), error_message=@error WHERE id=@id", new { id, error });
  }

  public async Task<IEnumerable<ImportJob>> GetRecent(int top = 50)
  {
    await using var conn = GetConnection();
    return await conn.QueryAsync<ImportJob>("SELECT TOP (@top) * FROM import_jobs ORDER BY id DESC", new { top });
  }
}
