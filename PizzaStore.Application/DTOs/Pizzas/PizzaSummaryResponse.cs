using System;

namespace PizzaStore.Application.DTOs.Pizzas
{
    public sealed record PizzaSummaryResponse(Guid Id, string Name, decimal Price);
}
