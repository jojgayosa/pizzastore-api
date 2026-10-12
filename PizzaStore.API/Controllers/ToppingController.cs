using Microsoft.AspNetCore.Mvc;
using PizzaStore.Application.Toppings.Commands;
using PizzaStore.Application.Toppings.Queries;

namespace PizzaStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ToppingController : ControllerBase
    {
        private readonly CreateToppingHandler _createHandler;
        private readonly UpdateToppingHandler _updateHandler;
        private readonly DeleteToppingHandler _deleteHandler;
        private readonly GetToppingsHandler _getAllHandler;
        private readonly GetToppingHandler _getHandler;

        public ToppingController(
            CreateToppingHandler createHandler,
            UpdateToppingHandler updateHandler,
            DeleteToppingHandler deleteHandler,
            GetToppingsHandler getAllHandler,
            GetToppingHandler getHandler)
        {
            _createHandler = createHandler;
            _updateHandler = updateHandler;
            _deleteHandler = deleteHandler;
            _getAllHandler = getAllHandler;
            _getHandler = getHandler;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateToppingCommand command, CancellationToken cancellationToken)
        {
            var result = await _createHandler.Handle(command, cancellationToken);
            return CreatedAtAction(nameof(GetBudId), new { id = result.Id }, result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var results = await _getAllHandler.Handle(new GetToppingsQuery(), cancellationToken);
            return Ok(results);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetBudId(Guid id, CancellationToken cancellationToken)
        {
            var result = await _getHandler.Handle(new GetToppingQuery(id), cancellationToken);
            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateToppingCommand command, CancellationToken cancellationToken)
        {
            // Ensure route id is used
            var cmd = command with { Id = id };
            var result = await _updateHandler.Handle(cmd, cancellationToken);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _deleteHandler.Handle(new DeleteToppingCommand(id), cancellationToken);
            return NoContent();
        }
    }
}
