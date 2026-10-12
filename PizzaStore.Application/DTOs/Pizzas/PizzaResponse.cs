using System;
using System.Collections.Generic;
using PizzaStore.Application.DTOs.Toppings;

namespace PizzaStore.Application.DTOs.Pizzas
{
    public sealed record PizzaResponse(
        Guid Id,
        string Name,
        string? Description,
        decimal Price,
        DateTimeOffset CreatedAt,
        DateTimeOffset UpdatedAt,
        List<ToppingResponse> Toppings
    );
}
