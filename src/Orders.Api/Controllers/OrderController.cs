using MediatR;
using Microsoft.AspNetCore.Mvc;
using Orders.Application.Commands;
using Orders.Application.Queries;

namespace Orders.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController(IMediator mediator) : ControllerBase
    {
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var query = new GetOrderById(id);

            var result= await mediator.Send(query);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AddOrder order)
        {
            var id=await mediator.Send(order);

            return Ok(id);
        }
    }

}
