using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Application.DTO.MoveOrder;
using Application.Features.MoveOrders.Commands;
using Application.Features.MoveOrders.Queries;
using Application.Results;
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
        public async Task<IActionResult> GetMoveOrders([FromQuery] GenericFiltersDto genericFiltersDto,
            [FromQuery] Sort sort)
        {
            GetAllResult<List<GetMoveOrderDto>> result =
                await _mediator.Send(new GetMoveOrdersQuery(genericFiltersDto, sort));

            return StatusCode((int)result.StatusCode!.Value, result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetMoveOrderById(int id)
        {
            Result<GetMoveOrderDto> result = await _mediator.Send(new GetMoveOrderByIdQuery(id));

            return StatusCode((int)result.StatusCode!.Value, result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddMoveOrder([FromBody] AddMoveOrderDto addMoveOrderDto)
        {
            int? userId = this.GetCurrentUserId();

            Result<object> result = await _mediator.Send(new AddMoveOrderCommand(userId, addMoveOrderDto));

            return StatusCode((int)result.StatusCode!.Value, result);
        }
    }
}