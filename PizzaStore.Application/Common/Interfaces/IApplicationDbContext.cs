using Microsoft.EntityFrameworkCore;
using PizzaStore.Domain.Entities;

namespace PizzaStore.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Pizza> Pizzas { get; }

        DbSet<Topping> Toppings { get; }

        DbSet<PizzaTopping> PizzaToppings { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
