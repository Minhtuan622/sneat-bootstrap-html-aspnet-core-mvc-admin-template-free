using AspnetCoreMvcFull.Repositories;
using Polly;
using Polly.Retry;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;

namespace AspnetCoreMvcFull.Services
{
  public class LarkService(
    HttpClient httpClient,
    IConfiguration config,
    SystemSettingsRepository settingsRepository)
  {
    private readonly AsyncRetryPolicy _retryPolicy =
      Policy
        .Handle<Exception>()
        .WaitAndRetryAsync(
          3,
          retryAttempt => TimeSpan.FromSeconds(retryAttempt),
          (exception, _, retryCount, _) =>
          {
            Console.WriteLine($"Retry {retryCount}: {exception.Message}");
          }
        );

    public async Task Send(string message)
    {
      var webhook = await settingsRepository.GetValue("lark_webhook") ?? config["Lark:Webhook"];
      // Optional: only used when Lark bot has "Signature verification" enabled.
      var secret = await settingsRepository.GetValue("lark_secret");

      if (string.IsNullOrWhiteSpace(webhook))
      {
        throw new Exception("Lark webhook missing");
      }

      webhook = webhook.Trim();

      var payload = new Dictionary<string, object>
      {
        ["msg_type"] = "text",
        ["content"] = new { text = message }
      };

      if (!string.IsNullOrWhiteSpace(secret))
      {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        payload["timestamp"] = timestamp;
        payload["sign"] = GenerateSign(timestamp, secret.Trim());
      }

      await _retryPolicy.ExecuteAsync(async () =>
      {
        var response = await httpClient.PostAsJsonAsync(webhook, payload);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<LarkResponse>();
        if (result is not null && result.Code != 0)
        {
          throw new Exception($"Lark API error {result.Code}: {result.Msg}");
        }
      });
    }

    private static string GenerateSign(string timestamp, string secret)
    {
      var stringToSign = $"{timestamp}\n{secret}";
      var keyBytes = Encoding.UTF8.GetBytes(secret);
      var valueBytes = Encoding.UTF8.GetBytes(stringToSign);
      using var hmac = new HMACSHA256(keyBytes);
      return Convert.ToBase64String(hmac.ComputeHash(valueBytes));
    }

    private sealed class LarkResponse
    {
      public int Code { get; set; }
      public string? Msg { get; set; }
    }
  }
}
