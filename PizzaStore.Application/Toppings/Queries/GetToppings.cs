using Microsoft.EntityFrameworkCore;
using PizzaStore.Application.Common.Interfaces;
using PizzaStore.Application.DTOs.Toppings;

namespace PizzaStore.Application.Toppings.Queries
{
    public record GetToppingsQuery();

    public class GetToppingsHandler
    {
        private readonly IApplicationDbContext _context;

        public GetToppingsHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<ToppingResponse>> Handle(GetToppingsQuery query, CancellationToken cancellationToken = default)
        {
            var results = await _context.Toppings
                .AsNoTracking()
                .OrderBy(t => t.Name)
                .Select(t => new ToppingResponse(t.Id, t.Name, t.CreatedAt, t.UpdatedAt))
                .ToListAsync(cancellationToken);

            return results;
        }
    }
}
