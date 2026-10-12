using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PizzaStore.Application.Common.Exceptions;
using PizzaStore.Application.Common.Interfaces;

namespace PizzaStore.Application.Pizzas.Commands
{
    public record DeletePizzaCommand(Guid Id = default);

    public class DeletePizzaValidator : AbstractValidator<DeletePizzaCommand>
    {
        public DeletePizzaValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");
        }
    }

    public class DeletePizzaHandler
    {
        private readonly IApplicationDbContext _context;

        public DeletePizzaHandler(IApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task Handle(DeletePizzaCommand command, CancellationToken cancellationToken = default)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            var pizza = await _context.Pizzas.FindAsync(command.Id, cancellationToken);

            if (pizza == null)
                throw new NotFoundException($"Pizza with id '{command.Id}' was not found.");

            // Remove any PizzaTopping relationships explicitly to respect Restrict delete behavior
            var relations = await _context.PizzaToppings
                .Where(pt => pt.PizzaId == command.Id)
                .ToListAsync(cancellationToken);

            if (relations.Count > 0)
            {
                _context.PizzaToppings.RemoveRange(relations);
            }

            _context.Pizzas.Remove(pizza);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
