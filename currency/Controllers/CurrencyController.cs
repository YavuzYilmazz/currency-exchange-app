using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using System.Text.Json.Serialization;
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

        [HttpGet(Name = "GetAllCurrencies")]
        public async Task<String> GetAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = "https://cdn.jsdelivr.net/gh/fawazahmed0/currency-api@1/latest/currencies.json";


                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    return content;

                }
                else
                {
                    Console.WriteLine($"error code: {response.StatusCode}");
                }
            }

            return null;
        }


        [HttpGet("{baseCurrency}/{targetCurrency}")]
        public async Task<String> Get(string baseCurrency, string targetCurrency, [FromQuery] string value)
        {

            using (HttpClient client = new HttpClient())
            {
                string apiUrl = "https://cdn.jsdelivr.net/gh/fawazahmed0/currency-api@1/latest/currencies/" + baseCurrency + "/" + targetCurrency + ".json";

                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var currencyData = JsonSerializer.Deserialize<Dictionary<string, object>>(content);
                    string result = (Convert.ToDecimal(value) * Convert.ToDecimal(currencyData[targetCurrency])).ToString();

                    return result;
                }
                else
                {
                    Console.WriteLine($"error code: {response.StatusCode}");
                }
            }

            return null;
        }
    }
}