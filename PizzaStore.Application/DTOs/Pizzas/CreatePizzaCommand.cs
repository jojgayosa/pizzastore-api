using System;
using System.Collections.Generic;

namespace PizzaStore.Application.DTOs.Pizzas
{
    public sealed record CreatePizzaCommand(
        string Name,
        string? Description,
        decimal Price,
        List<Guid> ToppingIds
    );
}
