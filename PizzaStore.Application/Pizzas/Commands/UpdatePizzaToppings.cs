using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PizzaStore.Application.Common.Exceptions;
using PizzaStore.Application.Common.Interfaces;
using PizzaStore.Application.DTOs.Pizzas;
using PizzaStore.Application.DTOs.Toppings;

namespace PizzaStore.Application.Pizzas.Commands
{
    public record UpdatePizzaToppingsCommand
    {
        public Guid PizzaId { get; init; } = default;

        public IReadOnlyCollection<Guid> ToppingIds { get; init; } = Array.Empty<Guid>();

        public UpdatePizzaToppingsCommand(Guid pizzaId, IReadOnlyCollection<Guid>? toppingIds = null)
        {
            PizzaId = pizzaId;
            ToppingIds = toppingIds ?? Array.Empty<Guid>();
        }
    }

    public class UpdatePizzaToppingsValidator : AbstractValidator<UpdatePizzaToppingsCommand>
    {
        public UpdatePizzaToppingsValidator()
        {
            RuleFor(x => x.PizzaId).NotEmpty().WithMessage("PizzaId is required.");

            RuleFor(x => x.ToppingIds)
                .NotNull().WithMessage("ToppingIds must not be null.")
                .Must(ids => ids == null || ids.Distinct().Count() == ids.Count)
                .WithMessage("Duplicate topping ids are not allowed.");
        }
    }

    public class UpdatePizzaToppingsHandler
    {
        private readonly IApplicationDbContext _context;

        public UpdatePizzaToppingsHandler(IApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<PizzaResponse> Handle(UpdatePizzaToppingsCommand command, CancellationToken cancellationToken = default)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            var pizza = await _context.Pizzas
                .Include(p => p.PizzaToppings)
                .FirstOrDefaultAsync(p => p.Id == command.PizzaId, cancellationToken);

            if (pizza == null)
                throw new NotFoundException($"Pizza with id '{command.PizzaId}' was not found.");

            var requestedIds = command.ToppingIds ?? Array.Empty<Guid>();
            var toppingResponses = new List<ToppingResponse>();
            // Validate toppings exist
            if (requestedIds.Count > 0)
            {
                var toppings = await _context.Toppings
                    .Where(t => requestedIds.Contains(t.Id))
                    .ToListAsync(cancellationToken);

                if (toppings.Count != requestedIds.Count)
                {
                    var missing = requestedIds.Except(toppings.Select(t => t.Id)).ToArray();
                    throw new NotFoundException($"One or more toppings were not found: {string.Join(',', missing)}");
                }
                var existingIds = pizza.PizzaToppings.Select(pt => pt.ToppingId).ToHashSet();

                var toRemove = existingIds.Except(requestedIds).ToList();
                var toAdd = requestedIds.Except(existingIds).ToList();

                // Remove toppings via domain method
                foreach (var id in toRemove)
                {
                    pizza.RemoveTopping(id);
                }

                // Add toppings via domain method
                foreach (var id in toAdd)
                {
                    var topping = toppings.First(t => t.Id == id);
                    pizza.AddTopping(topping);
                }

                toppingResponses = toppings.Select(t => new ToppingResponse(
                    t.Id,
                    t.Name,
                    t.CreatedAt,
                    t.UpdatedAt)).ToList();

            }
            else
            {
                // Remove all existing toppings
                var existingIds = pizza.PizzaToppings.Select(pt => pt.ToppingId).ToList();
                foreach (var id in existingIds)
                {
                    pizza.RemoveTopping(id);
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            return new PizzaResponse(
                pizza.Id, 
                pizza.Name, 
                pizza.Description, 
                pizza.Price, 
                pizza.CreatedAt, 
                pizza.UpdatedAt, 
                toppingResponses);
        }
    }
}
