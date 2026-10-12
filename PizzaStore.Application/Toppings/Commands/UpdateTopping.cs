using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PizzaStore.Application.Common.Exceptions;
using PizzaStore.Application.Common.Interfaces;
using PizzaStore.Application.DTOs.Toppings;

namespace PizzaStore.Application.Toppings.Commands
{
    public record UpdateToppingCommand(Guid Id, string Name = "");

    public class UpdateToppingValidator : AbstractValidator<UpdateToppingCommand>
    {
        private const int MaxNameLength = 100;

        public UpdateToppingValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(MaxNameLength).WithMessage($"Name must not exceed {MaxNameLength} characters.");
        }
    }

    public class UpdateToppingHandler
    {
        private readonly IApplicationDbContext _context;

        public UpdateToppingHandler(IApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<ToppingResponse> Handle(UpdateToppingCommand command, CancellationToken cancellationToken = default)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            var name = command.Name?.Trim() ?? string.Empty;

            var toppings = await _context.Toppings
                .Where(t => t.Id == command.Id || t.Name.ToLower() == name.ToLower())
                .ToListAsync(cancellationToken);

            var topping = toppings.FirstOrDefault(t => t.Id == command.Id);

            if (topping == null)
                throw new NotFoundException($"Topping with id '{command.Id}' was not found.");

            // Check for duplicate name by checking if any id is different from the command id
            var duplicate = toppings.Any(t => t.Id != command.Id);

            if (duplicate)
                throw new ConflictException($"A topping with the name '{name}' already exists.");

            // Update properties
            topping.Name = name;
            topping.SetUpdateDate();

            await _context.SaveChangesAsync(cancellationToken);

            return new ToppingResponse(topping.Id, topping.Name, topping.CreatedAt, topping.UpdatedAt);
        }
    }
}
