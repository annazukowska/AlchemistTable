using AlchemistTable.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace AlchemistTable.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Potion> Potions { get; set; }
        public DbSet<Alchemist> Alchemists { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        // Uncomment the following line if you have a Recipe model
        //public DbSet<Recipe> Recipes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Alchemist>()
                .HasIndex(a => a.Email)
                .IsUnique();
            modelBuilder.Entity<RefreshToken>()
                .HasIndex(rt => rt.Token)
                .IsUnique();
        }

    }
}
