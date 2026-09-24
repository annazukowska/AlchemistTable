using AlchemistTable.Core.Interfaces;
using AlchemistTable.Infrastructure.Data;
using AlchemistTable.Infrastructure.Extentions;
using AlchemistTable.WebApi.Endpoints;
using AlchemistTable.WebApi.Middleware;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAlchemistServices();
builder.Services.AddAuthenticationWithJwt();


var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => Results.Ok("Hello, Potion API!"));

app.MapGet("/info", () => Results.Ok(new
{
    App = "AlchemistTable API",
    Version = "1.0.0",
    Author = "AnnLorem",
    Timestamp = DateTime.UtcNow
}));

app.MapEndpoints();

app.Run();



