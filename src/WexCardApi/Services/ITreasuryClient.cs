using WexCardApi.DTO;

namespace WexCardApi.Services;

public interface ITreasuryClient
{
    Task<TreasuryRateRecord?> GetExchangeRate(GetExchangeRateRequest req);
}
