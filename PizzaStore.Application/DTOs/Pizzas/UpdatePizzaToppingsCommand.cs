using System;
using System.Collections.Generic;

namespace PizzaStore.Application.DTOs.Pizzas
{
    public sealed record UpdatePizzaToppingsCommand(List<Guid> ToppingIds);
}
