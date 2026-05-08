using System.Text.Json;

namespace AspnetCoreMvcFull.Services;

public class FacebookGraphService(
  HttpClient http,
  IConfiguration config)
{
  public async Task<JsonDocument> GetInsights()
  {
    var token = config["FacebookAds:AccessToken"];
    var adAccountId = config["FacebookAds:AdAccountId"];

    var url =
      $"https://graph.facebook.com/v22.0/act_{adAccountId}/insights" +
      $"?fields=campaign_name,adset_name,ad_name,spend,impressions,clicks,cpc,cpm,reach" +
      $"&date_preset=today" +
      $"&level=ad" +
      $"&access_token={token}";

    var response = await http.GetAsync(url);
    var content = await response.Content.ReadAsStringAsync();

    return JsonDocument.Parse(content);
  }
}
