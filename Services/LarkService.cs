using AspnetCoreMvcFull.Repositories;

namespace AspnetCoreMvcFull.Services
{
  public class LarkService(
    HttpClient httpClient,
    IConfiguration config,
    SystemSettingsRepository settingsRepository)
  {
    public async Task Send(string message)
    {
      var webhook = await settingsRepository.GetValue("lark_webhook") ?? config["Lark:Webhook"];

      var payload = new
      {
        msg_type = "text",
        content = new { text = message }
      };

      var response = await httpClient.PostAsJsonAsync(webhook, payload);
      response.EnsureSuccessStatusCode();
    }
  }
}
