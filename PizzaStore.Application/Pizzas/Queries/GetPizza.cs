using Microsoft.EntityFrameworkCore;
using PizzaStore.Application.Common.Exceptions;
using PizzaStore.Application.Common.Interfaces;
using PizzaStore.Application.DTOs.Pizzas;
using PizzaStore.Application.DTOs.Toppings;

namespace PizzaStore.Application.Pizzas.Queries
{
    public record GetPizzaQuery(Guid Id = default);

    public class GetPizzaHandler
    {
        private readonly IApplicationDbContext _context;

        public GetPizzaHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PizzaResponse> Handle(
            GetPizzaQuery query,
            CancellationToken cancellationToken = default)
        {
            var pizza = await _context.Pizzas
                .AsNoTracking()
                .Where(p => p.Id == query.Id)
                .Select(p => new PizzaResponse(
                    p.Id,
                    p.Name,
                    p.Description,
                    p.Price,
                    p.CreatedAt,
                    p.UpdatedAt,
                    p.PizzaToppings
                        .OrderBy(pt => pt.Topping!.Name)
                        .Select(pt => new ToppingResponse(
                            pt.ToppingId,
                            pt.Topping!.Name,
                            pt.Topping!.CreatedAt,
                            pt.Topping!.UpdatedAt))
                        .ToList()
                ))
                .FirstOrDefaultAsync(cancellationToken);

            if (pizza == null)
            {
                throw new NotFoundException(
                    $"Pizza with id '{query.Id}' was not found.");
            }

            return pizza;
        }
    }
}