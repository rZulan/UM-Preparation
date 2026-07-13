using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Application.DTO.MoveOrder;
using Application.Features.MoveOrders.Commands;
using Application.Features.MoveOrders.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UM_Preparation.Extensions;

namespace UM_Preparation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MoveOrderController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetMoveOrders([FromQuery] GenericFiltersDTO genericFiltersDTO, [FromQuery] Sort sort)
        {
            var result = await _mediator.Send(new GetMoveOrdersQuery(genericFiltersDTO, sort));

            return StatusCode(result.StatusCode!.Value.GetHashCode(), result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMoveOrderById(int id)
        {
            var result = await _mediator.Send(new GetMoveOrderByIdQuery(id));

            return StatusCode(result.StatusCode!.Value.GetHashCode(), result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddMoveOrder([FromBody] AddMoveOrderDTO addMoveOrderDTO)
        {
            var userId = this.GetCurrentUserId();

            var result = await _mediator.Send(new AddMoveOrderCommand(userId, addMoveOrderDTO));

            return StatusCode(result.StatusCode!.Value.GetHashCode(), result);
        }
    }
}
