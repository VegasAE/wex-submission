using WexApi.DTO;

namespace WexApi.Services;

public interface ITreasuryClient
{
    Task<TreasuryRateRecord?> GetExchangeRate(GetExchangeRateRequest req);
}
