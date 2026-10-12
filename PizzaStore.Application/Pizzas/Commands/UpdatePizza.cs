using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PizzaStore.Application.Common.Exceptions;
using PizzaStore.Application.Common.Interfaces;
using PizzaStore.Application.DTOs.Pizzas;
using PizzaStore.Application.DTOs.Toppings;

namespace PizzaStore.Application.Pizzas.Commands
{
    public record UpdatePizzaCommand(
        Guid Id = default, 
        string Name = "", 
        string? Description = null, 
        decimal Price = 0m);

    public class UpdatePizzaValidator : AbstractValidator<UpdatePizzaCommand>
    {
        private const int MaxNameLength = 200;
        private const int MaxDescriptionLength = 1000;

        public UpdatePizzaValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(MaxNameLength).WithMessage($"Name must not exceed {MaxNameLength} characters.");

            RuleFor(x => x.Description)
                .MaximumLength(MaxDescriptionLength).WithMessage($"Description must not exceed {MaxDescriptionLength} characters.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0.");
        }
    }

    public class UpdatePizzaHandler
    {
        private readonly IApplicationDbContext _context;

        public UpdatePizzaHandler(IApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<PizzaResponse> Handle(UpdatePizzaCommand command, CancellationToken cancellationToken = default)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            var name = command.Name?.Trim() ?? string.Empty;
            var nameLower = name.ToLowerInvariant();

            var pizzas = await _context.Pizzas
                .Where(p => p.Id == command.Id || p.Name.ToLower() == nameLower)
                .ToListAsync(cancellationToken);

            var pizza = pizzas.FirstOrDefault(p => p.Id == command.Id);

            if (pizza == null)
                throw new NotFoundException($"Pizza with id '{command.Id}' was not found.");

            // Check for duplicate name by checking if any id is different from the command id
            var duplicate = pizzas.Any(p => p.Id != command.Id);

            if (duplicate)
                throw new ConflictException($"A pizza with the name '{name}' already exists.");

            // Update properties
            pizza.Name = name;
            pizza.Description = command.Description;
            pizza.Price = command.Price;
            pizza.SetUpdateDate();

            await _context.SaveChangesAsync(cancellationToken);

            // Load toppings for response
            var toppingIds = await _context.PizzaToppings
                .Where(pt => pt.PizzaId == pizza.Id)
                .Select(pt => pt.ToppingId)
                .ToListAsync(cancellationToken);

            return new PizzaResponse(
                pizza.Id, 
                pizza.Name, 
                pizza.Description, 
                pizza.Price, 
                pizza.CreatedAt, 
                pizza.UpdatedAt,
                []
                );
        }
    }
}
