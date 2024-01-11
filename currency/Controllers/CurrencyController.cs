using System;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;


namespace currency.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CurrencyController : ControllerBase
    {

        private readonly ILogger<CurrencyController> _logger;

        public CurrencyController(ILogger<CurrencyController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{baseCurrency}/{targetCurrency}")]
        public async Task<string> Get(string baseCurrency, string targetCurrency, [FromQuery] string value)
        {
            if (string.IsNullOrEmpty(baseCurrency) || string.IsNullOrEmpty(targetCurrency) || string.IsNullOrEmpty(value))
            {
                // Gerekli parametrelerden biri veya birkaçı eksikse, hata mesajı döndür
                return "Hatalı istek. baseCurrency, targetCurrency ve value parametreleri boş olamaz.";
            }

            if (!decimal.TryParse(value, out _))
            {
                // value parametresi bir sayıya dönüştürülemezse, hata mesajı döndür
                return "Hatalı istek. value parametresi sayı olmalıdır.";
            }

            using (HttpClient client = new HttpClient())
            {
                string apiUrl = "https://cdn.jsdelivr.net/gh/fawazahmed0/currency-api@1/latest/currencies/" + baseCurrency + "/" + targetCurrency + ".json";

                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    try
                    {
                        JsonDocument jsonDocument = JsonDocument.Parse(content);

                        if (jsonDocument.RootElement.TryGetProperty(targetCurrency, out JsonElement element))
                        {
                            decimal rate = element.GetDecimal();
                            string result = (Convert.ToDecimal(value) * rate).ToString();
                            return result;
                        }
                        else
                        {
                            // targetCurrency özelliği bulunamazsa, uygun bir hata mesajı döndür
                            return "Hatalı istek. targetCurrency özelliği bulunamadı.";
                        }
                    }
                    catch (JsonException)
                    {
                        // JSON parse hatası durumunda, uygun bir hata mesajı döndür
                        return "Hatalı istek. JSON parse hatası.";
                    }
                }
                else
                {
                    // HTTP isteği başarısızsa, uygun bir hata mesajı döndür
                    return $"Hata kodu: {response.StatusCode}";
                }
            }
        }

    }
}