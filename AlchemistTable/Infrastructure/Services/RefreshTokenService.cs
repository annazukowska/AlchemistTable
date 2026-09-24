using System.Linq;
using AlchemistTable.Core.Entities;
using AlchemistTable.Core.Interfaces;
using AlchemistTable.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AlchemistTable.Infrastructure.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly AppDbContext _appDbContext;
        public RefreshTokenService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
        }
        public async Task<bool> AddAsync(RefreshToken token, CancellationToken cancellationToken = default)
        {
            var existingToken = await _appDbContext.RefreshTokens.FirstOrDefaultAsync(t => t.Token == token.Token);
            if (existingToken != null) 
            {
                throw new ArgumentException("A token with this value already exists.");
            }
            await _appDbContext.RefreshTokens.AddAsync(token, cancellationToken);
            await _appDbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> DeleteAsync(string token, CancellationToken cancellationToken = default)
        {
            var existingToken = await _appDbContext.RefreshTokens.FirstOrDefaultAsync(t => t.Token == token, cancellationToken);
            if (existingToken != null)
            {
                _appDbContext.RefreshTokens.Remove(existingToken);
                await _appDbContext.SaveChangesAsync(cancellationToken);
                return true;
            }
            return false;
        }

        public async Task<bool> RefreshTokenAsync(string oldToken, RefreshToken newToken, CancellationToken cancellationToken = default) 
        {
            var existingToken = await _appDbContext.RefreshTokens.FirstOrDefaultAsync(t => t.Token == oldToken, cancellationToken);
            if (existingToken != null)
            {
                _appDbContext.RefreshTokens.Remove(existingToken);
            }
            if (newToken == null || string.IsNullOrEmpty(newToken.Token))
            {
                throw new ArgumentException("New token cannot be null or empty.");
            }
            var checkNewToken = await _appDbContext.RefreshTokens.FirstOrDefaultAsync(t => t.Token == newToken.Token, cancellationToken);
            if (checkNewToken != null)
            {
                throw new ArgumentException("A token with this value already exists.");
            }
            await _appDbContext.RefreshTokens.AddAsync(newToken, cancellationToken);
            await _appDbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<IReadOnlyList<RefreshToken>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _appDbContext.RefreshTokens.ToListAsync(cancellationToken);
        }

        public async Task<RefreshToken?> GetTokenAsync(string token, CancellationToken cancellationToken = default)
        {
            return await _appDbContext.RefreshTokens
                .FirstOrDefaultAsync(t => t.Token.Equals(token), cancellationToken);
        }

        public async Task<RefreshToken?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)  
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("ID cannot be empty.", nameof(id));
            }
            return await _appDbContext.RefreshTokens.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<bool> UpdateAsync(RefreshToken entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            var existingToken = await _appDbContext.RefreshTokens.FindAsync(new object[] { entity.Id }, cancellationToken);
            if (existingToken == null)
            {
                return false;
            }
            existingToken.Token = entity.Token;
            existingToken.ExpiresAt = entity.ExpiresAt;
            existingToken.AlchemistId = entity.AlchemistId;
            _appDbContext.RefreshTokens.Update(existingToken);
            await _appDbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var token = await _appDbContext.RefreshTokens.FindAsync(new object[] { id }, cancellationToken);
            if (token != null)
            {
                _appDbContext.RefreshTokens.Remove(token);
                await _appDbContext.SaveChangesAsync(cancellationToken);
                return true;
            }
            return false;
        }
    }
}
