using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PizzaStore.Application.Common.Exceptions;
using PizzaStore.Application.Common.Interfaces;
using PizzaStore.Application.DTOs.Toppings;

namespace PizzaStore.Application.Toppings.Queries
{
    public record GetToppingQuery(Guid Id = default);

    public class GetToppingValidator : AbstractValidator<GetToppingQuery>
    {
        public GetToppingValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");
        }
    }

    public class GetToppingHandler
    {
        private readonly IApplicationDbContext _context;

        public GetToppingHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ToppingResponse> Handle(GetToppingQuery query, CancellationToken cancellationToken = default)
        {
            var result = await _context.Toppings
                .AsNoTracking()
                .Where(t => t.Id == query.Id)
                .Select(t => new ToppingResponse(t.Id, t.Name, t.CreatedAt, t.UpdatedAt))
                .FirstOrDefaultAsync(cancellationToken);

            if (result == null)
            {
                throw new NotFoundException($"Topping with id '{query.Id}' was not found.");
            }

            return result;
        }
    }
}
