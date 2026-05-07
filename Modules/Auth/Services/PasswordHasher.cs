using System.Security.Cryptography;
using System.Text;

namespace AspnetCoreMvcFull.Services;

public static class PasswordHasher
{
  public static string Hash(string input)
  {
    var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));

    return Convert.ToHexString(bytes);
  }

  public static bool Verify(string input, string hash)
  {
    return string.Equals(Hash(input), hash, StringComparison.OrdinalIgnoreCase);
  }
}
