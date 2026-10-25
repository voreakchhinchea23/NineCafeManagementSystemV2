namespace NineCafeApi.Services.Auth
{
    public interface ITokenBlacklistService
    {
        void BlacklistToken(string token, TimeSpan expiry);
        bool IsBlacklisted(string token);
    }
}
