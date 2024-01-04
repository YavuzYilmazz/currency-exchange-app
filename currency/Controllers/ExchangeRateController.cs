using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;


namespace currency.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExchangeRateController : Controller
    {
        [HttpPost("{fromCurrency}/{amount}/{toCurrency}")]
        public async Task<IActionResult> DovizCevir(
            [FromRoute] string fromCurrency,
            [FromRoute] decimal amount,
            [FromRoute] string toCurrency)
        {
            try
            {
                // API Key'i buraya ekleyin
                string apiKey = "f6ccacdf30899180c284929c";

                // API ile döviz kurlarını al
                string apiUrl = $"https://open.er-api.com/v6/latest/{fromCurrency}?apikey={apiKey}";
                var exchangeRates = await GetExchangeRates(apiUrl);

                if (exchangeRates != null && exchangeRates.Rates != null &&
                    exchangeRates.Rates.ContainsKey(toCurrency))
                {
                    decimal convertedAmount = amount * exchangeRates.Rates[toCurrency];
                    ViewData["Result"] = $"{amount} {fromCurrency} = {convertedAmount} {toCurrency}";
                }
                else
                {
                    ViewData["Result"] = "Döviz kuru alınamadı veya hatalı döviz cinsi girildi.";
                }

                // ExchangeRate modelini view'a göndermeden sadece sonucu gönder
                return View("YourViewName"); // Replace "YourViewName" with the actual name of your view
            }
            catch (Exception ex)
            {
                ViewData["Result"] = $"Error: {ex.Message}";
                return View("YourViewName"); // Replace "YourViewName" with the actual name of your view
            }
        }

        private async Task<ExchangeRate> GetExchangeRates(string apiUrl)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<ExchangeRate>(json);
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
