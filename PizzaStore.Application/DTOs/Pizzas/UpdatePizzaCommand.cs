using System;
using System.Collections.Generic;

namespace PizzaStore.Application.DTOs.Pizzas
{
    public sealed record UpdatePizzaCommand(
        string Name,
        string? Description,
        decimal Price
    );
}
