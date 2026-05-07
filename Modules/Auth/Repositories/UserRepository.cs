using AspnetCoreMvcFull.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace AspnetCoreMvcFull.Repositories;

public class UserRepository(IConfiguration config)
{
  private SqlConnection GetConnection()
  {
    return new SqlConnection(config.GetConnectionString("DefaultConnection"));
  }

  public async Task<User?> GetByUsername(string username)
  {
    await using var conn = GetConnection();

    return await conn.QueryFirstOrDefaultAsync<User>(
      "SELECT * FROM users WHERE username = @username",
      new { username }
    );
  }

  public async Task<IEnumerable<User>> GetAll()
  {
    await using var conn = GetConnection();

    return await conn.QueryAsync<User>("SELECT * FROM users ORDER BY created_at DESC");
  }

  public async Task<User?> GetById(long id)
  {
    await using var conn = GetConnection();

    return await conn.QueryFirstOrDefaultAsync<User>("SELECT * FROM users WHERE id = @id", new { id });
  }

  public async Task<bool> ExistsUsername(string username, long? excludeId = null)
  {
    await using var conn = GetConnection();

    var count = await conn.ExecuteScalarAsync<int>(
      "SELECT COUNT(1) FROM users WHERE username = @username AND (@excludeId IS NULL OR id <> @excludeId)",
      new { username, excludeId }
    );

    return count > 0;
  }

  public async Task<long> Create(UserFormModel model, string passwordHash)
  {
    await using var conn = GetConnection();

    return await conn.ExecuteScalarAsync<long>(@"
INSERT INTO users(username, password_hash, full_name, role_name, is_active, created_at)
OUTPUT INSERTED.id
VALUES(@Username, @PasswordHash, @FullName, @RoleName, @IsActive, GETUTCDATE())",
      new
      {
        model.Username,
        PasswordHash = passwordHash,
        model.FullName,
        model.RoleName,
        model.IsActive
      });
  }

  public async Task Update(UserFormModel model)
  {
    await using var conn = GetConnection();
    await conn.ExecuteAsync(@"
UPDATE users
SET full_name = @FullName,
    role_name = @RoleName,
    is_active = @IsActive
WHERE id = @Id", model);
  }

  public async Task ResetPassword(long id, string passwordHash)
  {
    await using var conn = GetConnection();
    await conn.ExecuteAsync("UPDATE users SET password_hash = @passwordHash WHERE id = @id", new { id, passwordHash });
  }


  public async Task UpdateProfile(string username, string fullName, string? avatarPath)
  {
    await using var conn = GetConnection();
    await conn.ExecuteAsync(@"UPDATE users SET full_name = @fullName, avatar_path = @avatarPath WHERE username = @username",
      new { username, fullName, avatarPath });
  }

  public async Task Delete(long id)
  {
    await using var conn = GetConnection();
    await conn.ExecuteAsync("DELETE FROM users WHERE id = @id", new { id });
  }
}
