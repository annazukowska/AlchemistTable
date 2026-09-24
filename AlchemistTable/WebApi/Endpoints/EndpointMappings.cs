using System.Security.Claims;
using AlchemistTable.Application.DTOs;
using AlchemistTable.Application.Handlers;
using AlchemistTable.Core.Entities;
using AlchemistTable.Core.Interfaces;
using AlchemistTable.Infrastructure.Data;
using AlchemistTable.Infrastructure.Helpers;
using Microsoft.AspNetCore.Authorization;
using MiniValidation;

namespace AlchemistTable.WebApi.Endpoints
{
    public static class EndpointMappings
    {
        public static void MapEndpoints(this WebApplication app)
        {
            app.MapMainAppEndpoints();
            app.MapAlchemistEndpoints();
            app.MapPotionEndpoints();
            app.MapIngredientEndpoints();
            app.MapBrewingEndpoints();
        }

        public static void MapMainAppEndpoints(this WebApplication app)
        {
            app.MapPost("/login", async (AlchemistLoginRequest login, IAlchemistService alchemistService, IRefreshTokenService refreshTokenService, ITokenGenerator tokenGenerator, CancellationToken cancellationToken) =>
            {
                if (!MiniValidator.TryValidate(login, out var errors))
                    return Results.BadRequest(errors);

                var alchemist = await alchemistService.LoginAlchemist(login.Email, login.Password, cancellationToken);
                if (alchemist == null) return Results.Unauthorized();

                string token = await tokenGenerator.GenerateTokenAsync(alchemist.Email, cancellationToken);

                var refreshToken = new RefreshToken(alchemist.Id, token);

                var isAdded = await refreshTokenService.AddAsync(refreshToken, cancellationToken);

                if (!isAdded)
                    return Results.Problem("Failed to log in. Please try again later.");

                return Results.Ok(new
                {
                    accessToken = token,
                    refreshToken = refreshToken.Token
                });
            });

            app.MapPost("/refresh-token", async (IAlchemistService alchemistService, IRefreshTokenService refreshTokenService, ITokenGenerator tokenGenerator, RefreshTokenRequest refreshTokenReq, CancellationToken cancellationToken) =>
            {
                if (!MiniValidator.TryValidate(refreshTokenReq, out var errors))
                    return Results.BadRequest(errors);

                var stored = await refreshTokenService.GetTokenAsync(refreshTokenReq.RefreshToken, cancellationToken);

                if (stored == null)
                    return Results.Unauthorized();

                if (stored.IsExpired)
                {
                    var isExpiredDeleted = await refreshTokenService.DeleteAsync(stored.Token);
                    if (!isExpiredDeleted)
                        return Results.Problem("Failed to delete expired refresh token. Please try again later.");

                    return Results.BadRequest("Refresh token is expired. You need to log in again");
                }

                var isDeleted = await refreshTokenService.DeleteAsync(stored.Token, cancellationToken);
                if (!isDeleted)
                    return Results.Problem("Failed to delete old refresh token. Please try again later.");

                var alchemist = await alchemistService.GetByIdAsync(stored.AlchemistId, cancellationToken);
                if (alchemist == null)
                    return Results.NotFound();

                var newAccessToken = await tokenGenerator.GenerateTokenAsync(alchemist.Email, cancellationToken);
                var refreshToken = new RefreshToken(alchemist.Id, newAccessToken);

                var isAdded = await refreshTokenService.AddAsync(refreshToken, cancellationToken);
                
                if (!isAdded)
                    return Results.Problem("Failed to refresh token. Please try again later.");

                return Results.Ok(new
                {
                    accessToken = newAccessToken,
                    refreshToken = refreshToken.Token
                });
            });

            app.MapGet("/me", [Authorize] async (IAlchemistService alchemistService, ClaimsPrincipal user, CancellationToken cancellationToken) =>
            {
                var email = user.Identity?.Name;
                if (email == null) return Results.Unauthorized();
                
                var alchemist = await alchemistService.GetAlchemistByEmailAsync(email, cancellationToken);
                if (alchemist == null) return Results.NotFound();

                return Results.Ok(new AlchemistResponse { Id = alchemist.Id, Name = alchemist.Name, Email = alchemist.Email });
            });

            app.MapGet("/token-info", [Authorize] (HttpContext httpContext) =>
            {
                var expClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == "exp");
                if (expClaim == null)
                    return Results.BadRequest("Expiration claim not found.");

                var expUnix = long.Parse(expClaim.Value);
                var expTime = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;
                var remaining = expTime - DateTime.UtcNow;

                return Results.Ok(new
                {
                    ExpiresAtUtc = expTime,
                    TimeRemaining = remaining.ToString("g")
                });
            });

            app.MapPost("/logout", [Authorize] async (IRefreshTokenService refreshTokenService, RefreshTokenRequest refreshToken, CancellationToken cancellationToken) =>
            {
                if (!MiniValidator.TryValidate(refreshToken, out var errors))
                    return Results.BadRequest(errors);

                var token = await refreshTokenService.GetTokenAsync(refreshToken.RefreshToken, cancellationToken);
                if (token != null)
                {
                    var isDeleted = await refreshTokenService.DeleteAsync(token.Token, cancellationToken);
                    if (!isDeleted)
                        return Results.Problem("Failed to log out. Token removal error.");
                    return Results.Ok("Logged out successfully.");
                }
                return Results.NotFound("Refresh token not found.");
            });
        }

        public static void MapAlchemistEndpoints(this WebApplication app)
        {
            //TODO: Add AlchemistResponse and remove password from the response. Also add a separate endpoint for getting the inventory of the alchemist.
            //app.MapGet("/alchemists", async (IAlchemistService alchemistService, CancellationToken cancellationToken) => await alchemistService.GetAllAsync(cancellationToken));

            app.MapGet("/alchemists/{id}", async (IAlchemistService alchemistService, Guid id, CancellationToken cancellationToken) =>
                {
                    var alchemist = await alchemistService.GetByIdAsync(id, cancellationToken);
                    return alchemist is not null ? Results.Ok(new AlchemistResponse { Id = alchemist.Id, Name = alchemist.Name, Email = alchemist.Email }) : Results.NotFound();
                });

            app.MapPost("/alchemists", [Authorize] async (IAlchemistService alchemistService, AlchemistCreateRequest alchemistCreateRequest, CancellationToken cancellationToken) =>
                {
                    if (!MiniValidator.TryValidate(alchemistCreateRequest, out var errors))
                        return Results.BadRequest(errors);

                var newAlchemist = new Alchemist(alchemistCreateRequest.Name, alchemistCreateRequest.Email, alchemistCreateRequest.Password, alchemistCreateRequest.Inventory);

                var isAdded = await alchemistService.AddAsync(newAlchemist, cancellationToken);
                if (!isAdded)
                    return Results.Problem("Failed to create alchemist. Please try again later.");

                return Results.Created($"/alchemists/{newAlchemist.Id}", new AlchemistResponse { Id = newAlchemist.Id, Name = newAlchemist.Name, Email = newAlchemist.Email });
            });

            app.MapDelete("/alchemists/{id}", [Authorize] async (IAlchemistService alchemistService, Guid id, CancellationToken cancellationToken) =>
                await alchemistService.DeleteAsync(id, cancellationToken) ? Results.Ok() : Results.NotFound("Alchemist doesn't exist"));
        }

        public static void MapPotionEndpoints(this WebApplication app)
        {
            app.MapGet("/potions", async (IPotionService potionService, CancellationToken cancellationToken) => await potionService.GetAllAsync(cancellationToken));

            //Just to manually add for testing. For creating please use the /potions/brew endpoint
            app.MapPost("/potions", [Authorize] async (IPotionService potionService, Potion potion, CancellationToken cancellationToken) =>
            {
                var isAdded = await potionService.AddAsync(potion, cancellationToken);
                if (!isAdded)
                    return Results.Problem("Failed to add potion. Please try again later.");
                return Results.Created($"/potions/{potion.Id}", potion);
            });

            app.MapGet("/potions/{id}", [Authorize] async (IPotionService potionService, Guid id, CancellationToken cancellationToken) =>
            {
                var potion = await potionService.GetByIdAsync(id, cancellationToken);
                return potion is not null ? Results.Ok(potion) : Results.NotFound();
            });

            app.MapDelete("/potions/{id}", [Authorize] async (IPotionService potionService, Guid id, CancellationToken cancellationToken) =>
            {
                var potion = await potionService.GetByIdAsync(id, cancellationToken);
                if (potion == null)
                    return Results.NotFound("Potion not found.");

                var isDeleted = await potionService.DeleteAsync(id, cancellationToken);
                if (!isDeleted)
                    return Results.Problem("Failed to delete potion. Please try again later.");

                return Results.Ok($"Potion {potion.Name} destroyed successfully.");
            });
        }
        public static void MapIngredientEndpoints(this WebApplication app)
        {
            app.MapGet("/ingredients", [Authorize]async (IIngredientService ingredientService, string? name, CancellationToken cancellationToken) =>
            {
                var ingredients = string.IsNullOrWhiteSpace(name)
                    ? await ingredientService.GetAllAsync(cancellationToken)
                    : await ingredientService.GetIngredientsByNameAsync(name, cancellationToken);
                return Results.Ok(ingredients);
            });

            app.MapGet("/ingredients/{id}", [Authorize] async (IIngredientService ingredientService, Guid id, CancellationToken cancellationToken) => 
            {
                var ingredient = await ingredientService.GetByIdAsync(id, cancellationToken);
                return ingredient is not null ? Results.Ok(ingredient) : Results.NotFound("Ingredient not found.");
            });

            //TODO: Refactor this to use an IngredientCreateRequest DTO and add validation to it. Also consider if we want to allow alchemists to create their own custom ingredients or if we want to restrict ingredient creation to admins only.

            app.MapPost("/ingredients", [Authorize] async (IIngredientService ingredientService, Ingredient ingredient, CancellationToken cancellationToken) =>
            {
                //TODO: Add validation to the ingredient properties (e.g. name should not be empty, rarity should be a valid enum value, etc.) and return appropriate error messages if validation fails.

                //TODO: Consider adding a check to see if an ingredient with the same name already exists and return a conflict error if it does.

                var isAdded = await ingredientService.AddAsync(ingredient, cancellationToken);
                if (!isAdded)
                    return Results.Problem("Failed to add ingredient. Please try again later.");
                return Results.Created($"/ingredients/{ingredient.Id}", ingredient);
            });

            app.MapPut("/ingredients/{id}", [Authorize] async (IIngredientService ingredientService, Ingredient ingredient, CancellationToken cancellationToken) =>
            {

                var isUpdated = await ingredientService.UpdateAsync(ingredient, cancellationToken);
                if (!isUpdated)
                    return Results.Problem("Failed to update ingredient. Please try again later.");
                return Results.Created($"/ingredients/{ingredient.Id}", ingredient);
            });

            app.MapDelete("/ingredients/{id}", [Authorize] async (IIngredientService ingredientService, Guid id, CancellationToken cancellationToken) =>
            {
                var ingredient = await ingredientService.GetByIdAsync(id, cancellationToken);
                if (ingredient == null)
                    return Results.NotFound("Ingredient not found.");

                var isDeleted = await ingredientService.DeleteAsync(id, cancellationToken);
                if (!isDeleted)
                    return Results.Problem("Failed to delete ingredient. Please try again later.");

                return Results.Ok($"Ingredient {ingredient.Name} destroyed successfully.");
            });
        }

        public static void MapBrewingEndpoints(this WebApplication app)
        {
            //app.MapPost("/potions/brew", [Authorize] async (CraftPotionRequest request) =>
            //{
            //    if (!MiniValidator.TryValidate(request, out var errors))
            //        return Results.BadRequest(errors);

            //    var ingredients = (await ingredientService.GetAllAsync())
            //        .Where(i => request.IngredientNames.Contains(i.Name))
            //        .ToList();

            //    if (ingredients.Count == 0)
            //        return Results.BadRequest("No valid ingredients provided.");

            //    if (ingredients.Count != request.IngredientNames.Count)
            //        return Results.BadRequest("One or more ingredients not found.");

            //    var brewedPotion = await potionService.BrewPotion(ingredients, request);
            //    if (brewedPotion is null)
            //        return Results.Problem("Failed to brew potion. Please try again later.");
            //    return Results.Created($"/potions/{brewedPotion.Id}", brewedPotion);
            //});

            //TODO: Refactor this to use the BrewPotionHandler and move the brewing logic there. Also add authorization to check if the alchemist has the required ingredients in their inventory before allowing them to brew a potion.

            app.MapPost("/potions/brew", [Authorize] async (
                IAlchemistService alchemistService,
                IIngredientService ingredientService,
                CraftPotionRequest request,
                BrewPotionHandler handler,
                CancellationToken cancellationToken) =>
            {
                var (success, error, potion) = await handler.HandleAsync(request, cancellationToken);

                if (!success)
                    return Results.BadRequest(error);

                return Results.Created($"/potions/{potion!.Id}", potion);
            });
        }
    }
}
