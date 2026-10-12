using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PizzaStore.Application.Common.Exceptions;
using PizzaStore.Application.Common.Interfaces;
using PizzaStore.Application.DTOs.Pizzas;
using PizzaStore.Application.DTOs.Toppings;
using PizzaStore.Domain.Entities;

namespace PizzaStore.Application.Pizzas.Commands
{
    public record CreatePizzaCommand
    {
        public string Name { get; init; } = string.Empty;

        public string? Description { get; init; }

        public decimal Price { get; init; } = 0m;

        public IReadOnlyCollection<Guid> ToppingIds { get; init; } = Array.Empty<Guid>();
    }

    public class CreatePizzaValidator : AbstractValidator<CreatePizzaCommand>
    {
        private const int MaxNameLength = 200;
        private const int MaxDescriptionLength = 1000;

        public CreatePizzaValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(MaxNameLength).WithMessage($"Name must not exceed {MaxNameLength} characters.");

            RuleFor(x => x.Description)
                .MaximumLength(MaxDescriptionLength).WithMessage($"Description must not exceed {MaxDescriptionLength} characters.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0.");

            RuleFor(x => x.ToppingIds)
                .NotNull().WithMessage("ToppingIds must not be null.")
                .Must(ids => ids.Distinct().Count() == ids.Count)
                .WithMessage("Duplicate topping ids are not allowed.");
        }
    }

    public class CreatePizzaHandler
    {
        private readonly IApplicationDbContext _context;

        public CreatePizzaHandler(IApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<PizzaResponse> Handle(CreatePizzaCommand command, CancellationToken cancellationToken = default)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            var name = command.Name?.Trim() ?? string.Empty;

            // Check for existing pizza with same name (case-insensitive)
            var nameLower = name.ToLowerInvariant();
            var exists = await _context.Pizzas
                .AnyAsync(p => p.Name.ToLower() == nameLower, cancellationToken);

            if (exists)
            {
                throw new ConflictException($"A pizza with the name '{name}' already exists.");
            }

            var pizza = new Pizza(Guid.NewGuid(), name, command.Price, command.Description);
            var toppingResponses = new List<ToppingResponse>();

            var toppingIds = command.ToppingIds ?? Array.Empty<Guid>();
            if (toppingIds.Count > 0)
            {
                //Add toppings
                var toppings = await _context.Toppings
                    .Where(t => toppingIds.Contains(t.Id))
                    .ToListAsync(cancellationToken);

                if (toppings.Count != toppingIds.Count)
                {
                    var missing = toppingIds.Except(toppings.Select(t => t.Id)).ToArray();
                    throw new NotFoundException($"One or more toppings were not found: {string.Join(',', missing)}");
                }

                toppingResponses = toppings
                    .Select(t => new ToppingResponse(t.Id, t.Name, t.CreatedAt, t.UpdatedAt))
                    .ToList();

                foreach (var topping in toppings)
                {
                    pizza.AddTopping(topping);
                }
            }

            _context.Pizzas.Add(pizza);
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
