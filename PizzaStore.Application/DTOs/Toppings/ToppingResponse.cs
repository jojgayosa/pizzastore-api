using System;

namespace PizzaStore.Application.DTOs.Toppings
{
    public sealed record ToppingResponse(Guid Id, string Name, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt);
}
