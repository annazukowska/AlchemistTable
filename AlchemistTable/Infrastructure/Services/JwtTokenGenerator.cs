using AlchemistTable.Core.Entities;
using AlchemistTable.Core.Interfaces;
using AlchemistTable.Infrastructure.Helpers;

namespace AlchemistTable.Infrastructure.Services
{
    public class JwtTokenGenerator : ITokenGenerator
    {
        private readonly IConfiguration _configuration;

        public JwtTokenGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<string> GenerateTokenAsync(string email, CancellationToken cancellationToken = default)
        {
            var key = _configuration["Jwt:Key"]!;
            var token = JwtHelper.GenerateToken(email, key);
            return Task.FromResult(token);
        }
    }
}
