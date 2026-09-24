using AlchemistTable.Core.Entities;

namespace AlchemistTable.Core.Interfaces
{
    public interface IRefreshTokenService : ICrudService<RefreshToken>
    {
        Task<RefreshToken?> GetTokenAsync(string token, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(string token, CancellationToken cancellationToken = default);
        Task<bool> RefreshTokenAsync(string oldToken, RefreshToken newToken, CancellationToken cancellationToken = default);
    }
}
