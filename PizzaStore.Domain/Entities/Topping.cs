using System;
using System.Collections.Generic;

namespace PizzaStore.Domain.Entities
{
    public class Topping
    {
        private readonly List<PizzaTopping> _pizzaToppings = new();

        public Topping(Guid id, string name)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            var now = DateTimeOffset.UtcNow;
            CreatedAt = now;
            UpdatedAt = now;
        }

        // For serializers or ORM materialization only
        protected Topping()
        {
        }

        public Guid Id { get; init; }

        public string Name { get; set; } = null!;

        public DateTimeOffset CreatedAt { get; private set; }

        public DateTimeOffset UpdatedAt { get; private set; }

        public IReadOnlyCollection<PizzaTopping> PizzaToppings => _pizzaToppings.AsReadOnly();

        public void SetUpdateDate()
        {
            UpdatedAt = DateTimeOffset.UtcNow;
        }
    }
}
