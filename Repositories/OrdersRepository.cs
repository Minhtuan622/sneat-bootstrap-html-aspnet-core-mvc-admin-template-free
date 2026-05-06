using Dapper;
using Microsoft.Data.SqlClient;

namespace AspnetCoreMvcFull.Repositories
{
  public class OrdersRepository(IConfiguration config)
  {
    private SqlConnection GetConnection()
    {
      return new SqlConnection(config.GetConnectionString("DefaultConnection"));
    }

    public async Task Insert(
      string pageId,
      string postId,
      string adId,
      decimal revenue,
      DateTime createdAt)
    {
      await using var conn = GetConnection();

      await conn.ExecuteAsync(
        """
        INSERT INTO orders
        (
          page_id,
          post_id,
          ad_id,
          revenue,
          created_at
        )
        VALUES
        (
          @pageId,
          @postId,
          @adId,
          @revenue,
          @createdAt
        )
        """,
        new
        {
          pageId,
          postId,
          adId,
          revenue,
          createdAt
        }
      );
    }

    public async Task<bool> Exists(
      string postId,
      string adId,
      DateTime createdAt)
    {
      await using var conn = GetConnection();

      var count = await conn.ExecuteScalarAsync<int>(
        """
        SELECT COUNT(*)
        FROM orders
        WHERE post_id = @postId
        AND ad_id = @adId
        AND created_at = @createdAt
        """,
        new
        {
          postId,
          adId,
          createdAt
        }
      );

      return count > 0;
    }
  }
}
