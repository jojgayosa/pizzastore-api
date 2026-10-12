using System;
using System.Collections.Generic;

namespace PizzaStore.Domain.Entities
{
    public class Pizza
    {
        private readonly List<PizzaTopping> _pizzaToppings = new();

        public Pizza(Guid id, string name, decimal price, string? description = null)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Price = price;
            Description = description;
            var now = DateTimeOffset.UtcNow;
            CreatedAt = now;
            UpdatedAt = now;
        }

        // For serializers or ORM materialization only
        protected Pizza()
        {
        }

        public Guid Id { get; init; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public DateTimeOffset CreatedAt { get; private set; }

        public DateTimeOffset UpdatedAt { get; private set; }

        public IReadOnlyCollection<PizzaTopping> PizzaToppings => _pizzaToppings.AsReadOnly();

        public void AddTopping(Topping topping)
        {
            if (topping == null) throw new ArgumentNullException(nameof(topping));

            if (_pizzaToppings.Exists(pt => pt.ToppingId == topping.Id))
                return;

            _pizzaToppings.Add(new PizzaTopping(this, topping));
            UpdatedAt = DateTimeOffset.UtcNow;
        }

        public void RemoveTopping(Guid toppingId)
        {
            _pizzaToppings.RemoveAll(pt => pt.ToppingId == toppingId);
            UpdatedAt = DateTimeOffset.UtcNow;
        }

        public void SetUpdateDate()
        {
            UpdatedAt = DateTimeOffset.UtcNow;
        }
    }
}
