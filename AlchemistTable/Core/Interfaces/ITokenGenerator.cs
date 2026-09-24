using AlchemistTable.Core.Entities;

namespace AlchemistTable.Core.Interfaces
{
    public interface ITokenGenerator
    {
        Task<string> GenerateTokenAsync(string email, CancellationToken cancellationToken = default);
    }
}
