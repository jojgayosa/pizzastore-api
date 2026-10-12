using Microsoft.EntityFrameworkCore;
using PizzaStore.Domain.Entities;
using PizzaStore.Application.Common.Interfaces;
using System.Reflection;

namespace PizzaStore.Infrastructure.Data
{
    public class PizzaStoreDbContext : DbContext, IApplicationDbContext
    {
        public PizzaStoreDbContext(DbContextOptions<PizzaStoreDbContext> options)
            : base(options)
        {
        }

        public DbSet<Pizza> Pizzas => Set<Pizza>();

        public DbSet<Topping> Toppings => Set<Topping>();
         
        public DbSet<PizzaTopping> PizzaToppings => Set<PizzaTopping>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all IEntityTypeConfiguration implementations from this assembly
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
