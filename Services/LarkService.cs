using AspnetCoreMvcFull.Repositories;
using Polly;
using Polly.Retry;

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

      if (string.IsNullOrWhiteSpace(webhook))
      {
        throw new Exception("Lark webhook missing");
      }

      var payload = new
      {
        msg_type = "text",
        content = new { text = message }
      };

      await _retryPolicy.ExecuteAsync(async () =>
      {
        var response = await httpClient.PostAsJsonAsync(webhook, payload);

        response.EnsureSuccessStatusCode();
      });
    }
  }
}
