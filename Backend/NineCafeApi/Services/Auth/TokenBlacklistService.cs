using Microsoft.Extensions.Caching.Memory;

namespace NineCafeApi.Services.Auth
{
    public class TokenBlacklistService : ITokenBlacklistService
    {
        private readonly IMemoryCache _cache;
        public TokenBlacklistService(IMemoryCache cache)
        {
            _cache = cache;
        }
        public void BlacklistToken(string token, TimeSpan expiry)
        {
            _cache.Set(token, true, expiry);
        }
        public bool IsBlacklisted(string token)
        {
            return _cache.TryGetValue(token, out _);
        }
    }
}
