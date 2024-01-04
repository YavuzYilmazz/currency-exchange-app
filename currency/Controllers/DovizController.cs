using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

public class DovizController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> DovizCevir()
    {
        // Form verilerini al
        string fromCurrency = Request.Form["fromCurrency"].ToString().ToUpper();
        decimal amount = Convert.ToDecimal(Request.Form["amount"]);
        string toCurrency = Request.Form["toCurrency"].ToString().ToUpper();

        // API Key'i buraya ekleyin
        string apiKey = "YOUR_API_KEY";

        // API ile döviz kurlarını al
        string apiUrl = $"https://open.er-api.com/v6/latest/{fromCurrency}?apikey={apiKey}";
        var exchangeRates = await GetExchangeRates(apiUrl);

        if (exchangeRates != null && exchangeRates.Rates.ContainsKey(toCurrency))
        {
            decimal convertedAmount = amount * exchangeRates.Rates[toCurrency];

            ViewBag.Result = $"{amount} {fromCurrency} = {convertedAmount} {toCurrency}";
        }
        else
        {
            ViewBag.Result = "Döviz kuru alınamadı veya hatalı döviz cinsi girildi.";
        }

        return View("Index");
    }

    private async Task<ExchangeRates> GetExchangeRates(string apiUrl)
    {
        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = await client.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ExchangeRates>(json);
            }
            else
            {
                return null;
            }
        }
    }
}

public class ExchangeRates
{
    public string @base { get; set; }
    public string disclaimer { get; set; }
    public string license { get; set; }
    public long timestamp { get; set; }
    public string? @base { get; set; }
    public Dictionary<string, decimal> rates { get; set; }
}
