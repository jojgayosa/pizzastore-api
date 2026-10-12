using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PizzaStore.Application.Common.Exceptions;
using PizzaStore.Application.Common.Interfaces;
using PizzaStore.Application.DTOs.Toppings;
using PizzaStore.Domain.Entities;

namespace PizzaStore.Application.Toppings.Commands
{
    // Command containing the data required to create a topping
    public record CreateToppingCommand(string Name = "");

    // Validator for the CreateToppingCommand
    public class CreateToppingValidator : AbstractValidator<CreateToppingCommand>
    {
        private const int MaxNameLength = 100; // must match domain configuration

        public CreateToppingValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(MaxNameLength).WithMessage($"Name must not exceed {MaxNameLength} characters.");
        }
    }

    // Handler that performs the creation
    public class CreateToppingHandler
    {
        private readonly IApplicationDbContext _context;

        public CreateToppingHandler(IApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<ToppingResponse> Handle(CreateToppingCommand command, CancellationToken cancellationToken = default)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            // Normalize/trim the name before processing
            var name = command.Name.Trim();


            // Check for existing topping with the same name (case-insensitive)
            var exists = await _context.Toppings
                .AnyAsync(t => t.Name.ToLower() == name.ToLower(), cancellationToken);

            if (exists)
            {
                throw new ConflictException("A topping with the same name already exists.");
            }

            var topping = new Topping(Guid.NewGuid(), name);

            _context.Toppings.Add(topping);

            await _context.SaveChangesAsync(cancellationToken);

            return new ToppingResponse(
                topping.Id, 
                topping.Name, 
                topping.CreatedAt, 
                topping.UpdatedAt
                );
        }
    }
}
