using System;

namespace PizzaStore.Domain.Entities
{
    public class PizzaTopping
    {
        public PizzaTopping(Pizza pizza, Topping topping)
        {
            Pizza = pizza ?? throw new ArgumentNullException(nameof(pizza));
            Topping = topping ?? throw new ArgumentNullException(nameof(topping));
            PizzaId = pizza.Id;
            ToppingId = topping.Id;
        }

        // For serializers or ORM materialization only
        protected PizzaTopping()
        {
        }

        public Guid PizzaId { get; init; }

        public Guid ToppingId { get; init; }

        public Pizza? Pizza { get; init; }

        public Topping? Topping { get; init; }
    }
}
