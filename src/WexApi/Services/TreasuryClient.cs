using WexApi.DTO;

namespace WexApi.Services;

public class TreasuryClient : ITreasuryClient
{
    private const string BaseApiUrl = "https://api.fiscaldata.treasury.gov/services/api/fiscal_service/v1/accounting/od/rates_of_exchange";

    private readonly HttpClient _http;

    public TreasuryClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<TreasuryRateRecord?> GetExchangeRate(GetExchangeRateRequest req)
    {
        // build api filter
        DateOnly sixMonth = DateOnly.FromDateTime(req.RecordDate.AddMonths(-6));
        string qParams = "?sort=-record_date&format=json&fields=record_date,country,currency,exchange_rate,country_currency_desc&page[size]=1";
        string filters = $"&filter=currency:eq:{req.Currency},record_date:gte:{sixMonth:yyyy-MM-dd},record_date:lte:{req.RecordDate:yyyy-MM-dd}";

        string fullUrl = BaseApiUrl + qParams + filters;

        TreasuryApiResponse? resp = await _http.GetFromJsonAsync<TreasuryApiResponse>(fullUrl);

        if (resp == null || resp.Data.Count == 0)
        {
            return null;
        }

        return resp.Data[0];
    }
}
