using WexApi.DTO;
using WexApi.Services;

public class MockTreasuryClient : ITreasuryClient
{
    private TreasuryRateRecord? _response;

    public void SetResponse(TreasuryRateRecord? response)
    {
        _response = response;
    }

    public Task<TreasuryRateRecord?> GetExchangeRate(GetExchangeRateRequest req)
    {
        return Task.FromResult(_response);
    }
}
