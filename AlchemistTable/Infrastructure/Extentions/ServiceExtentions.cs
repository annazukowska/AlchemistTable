using System.Text;
using AlchemistTable.Application.Handlers;
using AlchemistTable.Core.Interfaces;
using AlchemistTable.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace AlchemistTable.Infrastructure.Extentions
{
    public static class ServiceExtensions
    {
        public static void AddAuthenticationWithJwt(this IServiceCollection services)
        {
            var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
            var jwtKey = configuration["Jwt:Key"]!;

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };
            });

            services.AddAuthorization();
        }

        public static IServiceCollection AddAlchemistServices(this IServiceCollection services)
        {
            services.AddScoped<IIngredientService, IngredientService>();
            services.AddScoped<IPotionService, PotionService>();
            services.AddScoped<IAlchemistService, AlchemistService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddScoped<ITokenGenerator, JwtTokenGenerator>();

            // Handlers should also be scoped
            services.AddScoped<BrewPotionHandler>();

            return services;
        }
    }

}
