namespace currency;


public class ExchangeRates
{
    public string @base { get; set; }
    public string disclaimer { get; set; }
    public string license { get; set; }
    public long timestamp { get; set; }
    public string? Base { get; set; }
    public Dictionary<string, decimal> rates { get; set; }
}
