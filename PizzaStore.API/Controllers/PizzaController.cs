using Microsoft.AspNetCore.Mvc;
using PizzaStore.Application.Pizzas.Commands;
using PizzaStore.Application.Pizzas.Queries;

namespace PizzaStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PizzaController : ControllerBase
    {
        private readonly CreatePizzaHandler _createHandler;
        private readonly UpdatePizzaHandler _updateHandler;
        private readonly UpdatePizzaToppingsHandler _updateToppingsHandler;
        private readonly DeletePizzaHandler _deleteHandler;
        private readonly GetPizzasHandler _getAllHandler;
        private readonly GetPizzaHandler _getHandler;

        public PizzaController(
            CreatePizzaHandler createHandler,
            UpdatePizzaHandler updateHandler,
            UpdatePizzaToppingsHandler updateToppingsHandler,
            DeletePizzaHandler deleteHandler,
            GetPizzasHandler getAllHandler,
            GetPizzaHandler getHandler)
        {
            _createHandler = createHandler;
            _updateHandler = updateHandler;
            _updateToppingsHandler = updateToppingsHandler;
            _deleteHandler = deleteHandler;
            _getAllHandler = getAllHandler;
            _getHandler = getHandler;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePizzaCommand command, CancellationToken cancellationToken)
        {
            var result = await _createHandler.Handle(command, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var results = await _getAllHandler.Handle(new GetPizzasQuery(), cancellationToken);
            return Ok(results);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _getHandler.Handle(new GetPizzaQuery(id), cancellationToken);
            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePizzaCommand command, CancellationToken cancellationToken)
        {
            var cmd = command with { Id = id };
            var result = await _updateHandler.Handle(cmd, cancellationToken);
            return Ok(result);
        }

        [HttpPut("{id:guid}/toppings")]
        public async Task<IActionResult> UpdateToppings(Guid id, [FromBody] UpdatePizzaToppingsCommand command, CancellationToken cancellationToken)
        {
            var cmd = command with { PizzaId = id };
            var result = await _updateToppingsHandler.Handle(cmd, cancellationToken);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _deleteHandler.Handle(new DeletePizzaCommand(id), cancellationToken);
            return NoContent();
        }
    }
}
