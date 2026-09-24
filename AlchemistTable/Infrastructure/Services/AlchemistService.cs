using AlchemistTable.Core.Entities;
using AlchemistTable.Core.Interfaces;
using AlchemistTable.Infrastructure.Data;
using Azure.Core;
using Microsoft.EntityFrameworkCore;

namespace AlchemistTable.Infrastructure.Services
{
    public class AlchemistService : IAlchemistService
    {
        private readonly AppDbContext _appDbContext;

        public AlchemistService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
        }

        public async Task<bool> AddAsync(Alchemist alchemist, CancellationToken cancellationToken = default)
        {
            var existing = await _appDbContext.Alchemists.FirstOrDefaultAsync(a => a.Email == alchemist.Email, cancellationToken);
            if (existing!=null)
            {
                throw new ArgumentException("An alchemist with this email already exists.");
            }
            await _appDbContext.Alchemists.AddAsync(alchemist, cancellationToken);
            await _appDbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var alchemist = await _appDbContext.Alchemists.FindAsync(new object[] { id }, cancellationToken);
            if (alchemist != null)
            {
                _appDbContext.Alchemists.Remove(alchemist);
                await _appDbContext.SaveChangesAsync(cancellationToken);
                return true;
            }
            return false;
        }

        public async Task<Alchemist?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _appDbContext.Alchemists.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<Alchemist?> GetAlchemistByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _appDbContext.Alchemists
                .FirstOrDefaultAsync(i => i.Email.Equals(email), cancellationToken);
        }

        public async Task<IReadOnlyList<Alchemist>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _appDbContext.Alchemists.ToListAsync(cancellationToken);
        }

        public async Task<bool> UpdateAsync(Alchemist alchemist, CancellationToken cancellationToken = default)
        {
            var existing = await _appDbContext.Alchemists.FindAsync(new object[] { alchemist.Id }, cancellationToken);
            if (existing != null)
            {
                existing.Name = alchemist.Name;
                existing.Email = alchemist.Email;
                existing.Password = alchemist.Password;
                existing.Inventory = alchemist.Inventory;
                await _appDbContext.SaveChangesAsync(cancellationToken);
                return true;
            } 
            else
            {
                throw new KeyNotFoundException($"Alchemist with ID {alchemist.Id} not found.");
            } 
        }

        public async Task<bool> UpdateAlchemistInventoryAsync(Guid id, List<Ingredient> inventory, CancellationToken cancellationToken = default)
        {             
            var alchemist = await _appDbContext.Alchemists.FindAsync(new object[] { id }, cancellationToken);
            if (alchemist != null)
            {
                alchemist.Inventory = inventory;
                await _appDbContext.SaveChangesAsync(cancellationToken);
                return true;
            }
            else
            {
                throw new KeyNotFoundException($"Alchemist with ID {id} not found.");
            }
        }
        public async Task<bool> UpdateAlchemistPasswordAsync(Guid id, string newPassword, CancellationToken cancellationToken = default)         
        {
            var alchemist = await _appDbContext.Alchemists.FindAsync(new object[] { id }, cancellationToken);
            if (alchemist != null)
            {
                alchemist.Password = newPassword;
                await _appDbContext.SaveChangesAsync(cancellationToken);
                return true;
            }
            else
            {
                throw new KeyNotFoundException($"Alchemist with ID {id} not found.");
            }
        }

        public async Task<Alchemist?> LoginAlchemist(string email, string password, CancellationToken cancellationToken = default)
        {
            var alchemist = await _appDbContext.Alchemists
                .FirstOrDefaultAsync(i => i.Email.Equals(email) && i.Password.Equals(password), cancellationToken);
            return alchemist;
        }
    }
}
