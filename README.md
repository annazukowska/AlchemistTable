# AlchemistTable API

A REST API built with ASP.NET Core modelling a fantasy alchemy 
system - alchemists, ingredients, and potions with a brewing mechanic.

## Features

- **JWT Authentication** with refresh token rotation
- **Potion brewing** - combine ingredients to create potions
- **Ingredient management** - search by name with DB-level filtering
- **Alchemist accounts** - register, login, profile management
- **Exception handling middleware** - maps exceptions to correct 
  HTTP status codes
- **EF Core** with SQL Server, migrations, and CSV data seeding

## Tech Stack

- ASP.NET Core Minimal APIs (.NET 8)
- Entity Framework Core + SQL Server
- JWT Bearer authentication
- MiniValidator for request validation
- User Secrets for local configuration

## How to Run

1. Clone the repository
2. Add connection string and JWT key to User Secrets:
```bash
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "your-connection-string"
   dotnet user-secrets set "Jwt:Key" "your-jwt-key"
```
3. Run migrations:
```bash
   dotnet ef database update
```
4. Run the project:
```bash
   dotnet run
```

## Endpoints

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| POST | /alchemists | Register | ❌ |
| POST | /login | Login, returns JWT | ❌ |
| POST | /refresh-token | Refresh JWT | ✅ |
| POST | /logout | Logout | ✅ |
| GET | /me | Current user profile | ✅ |
| GET | /ingredients | List / search by name | ✅ |
| GET | /ingredients/{id} | Get ingredient | ✅ |
| PUT | /ingredients/{id} | Update ingredient | ✅ |
| GET | /potions | List potions | ✅ |
| POST | /potions/brew | Brew a potion | ✅ |
| DELETE | /potions/{id} | Delete potion | ✅ |
