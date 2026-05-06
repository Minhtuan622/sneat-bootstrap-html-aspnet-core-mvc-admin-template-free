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
      " SELECT * FROM users WHERE username = @username",
      new { username }
    );
  }
}
