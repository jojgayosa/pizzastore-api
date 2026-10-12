using Microsoft.EntityFrameworkCore;
using PizzaStore.Application.Common.Interfaces;
using PizzaStore.Application.DTOs.Pizzas;
using PizzaStore.Application.DTOs.Toppings;

namespace PizzaStore.Application.Pizzas.Queries
{
    public record GetPizzasQuery();

    public class GetPizzasHandler
    {
        private readonly IApplicationDbContext _context;

        public GetPizzasHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<PizzaResponse>> Handle(GetPizzasQuery query, CancellationToken cancellationToken = default)
        {
            var results = await _context.Pizzas
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .Select(p => new PizzaResponse(
                    p.Id,
                    p.Name,
                    p.Description,
                    p.Price,
                    p.CreatedAt,
                    p.UpdatedAt,
                    p.PizzaToppings
                        .OrderBy(pt => pt.Topping!.Name)
                        .Select(pt => new ToppingResponse(pt.ToppingId, pt.Topping!.Name, pt.Topping!.CreatedAt, pt.Topping!.UpdatedAt))
                        .ToList()
                ))
                .ToListAsync(cancellationToken);

            return results;
        }
    }
}
