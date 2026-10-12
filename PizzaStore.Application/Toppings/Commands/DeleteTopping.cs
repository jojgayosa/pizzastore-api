using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PizzaStore.Application.Common.Exceptions;
using PizzaStore.Application.Common.Interfaces;

namespace PizzaStore.Application.Toppings.Commands
{
    public record DeleteToppingCommand(Guid Id = default);

    public class DeleteToppingValidator : AbstractValidator<DeleteToppingCommand>
    {
        public DeleteToppingValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");
        }
    }

    public class DeleteToppingHandler
    {
        private readonly IApplicationDbContext _context;

        public DeleteToppingHandler(IApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task Handle(DeleteToppingCommand command, CancellationToken cancellationToken = default)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            var topping = await _context.Toppings.FindAsync(command.Id, cancellationToken);

            if (topping == null)
            {
                throw new NotFoundException($"Topping with id '{command.Id}' was not found.");
            }

            // Check if topping is used by any pizza
            var inUse = await _context.PizzaToppings
                .AnyAsync(pt => pt.ToppingId == command.Id, cancellationToken);

            if (inUse)
            {
                throw new ConflictException($"Topping with id '{command.Id}' is assigned to one or more pizzas and cannot be deleted.");
            }

            _context.Toppings.Remove(topping);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
