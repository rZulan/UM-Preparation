using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Application.DTO.WarehouseReceiving;
using Application.Features.WarehouseReceivings.Commands;
using Application.Features.WarehouseReceivings.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UM_Preparation.Extensions;

namespace UM_Preparation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WarehouseController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetWarehouseReceivings([FromQuery] GenericFiltersDTO genericFiltersDTO, [FromQuery] Sort sort)
        {
            var result = await _mediator.Send(new GetWarehouseReceivingsQuery(genericFiltersDTO, sort));

            return StatusCode(result.StatusCode!.Value.GetHashCode(), result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetWarehouseReceivingById(int id)
        {
            var result = await _mediator.Send(new GetWarehouseReceivingByIdQuery(id));

            return StatusCode(result.StatusCode!.Value.GetHashCode(), result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddWarehouseReceiving([FromBody] AddWarehouseReceivingDTO addWarehouseDTO)
        {
            var userId = this.GetCurrentUserId();

            var result = await _mediator.Send(new AddWarehouseReceivingCommand(userId, addWarehouseDTO));

            return StatusCode(result.StatusCode!.Value.GetHashCode(), result);
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateWarehouse(int id, [FromBody] UpdateWarehouseReceivingDTO updateWarehouseDTO)
        {
            var userId = this.GetCurrentUserId();

            var result = await _mediator.Send(new UpdateWarehouseReceivingCommand(userId, id, updateWarehouseDTO));

            return StatusCode(result.StatusCode!.Value.GetHashCode(), result);
        }

        [HttpPatch("{id}/archive")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ArchiveWarehouseReceiving(int id)
        {
            var userId = this.GetCurrentUserId();

            var result = await _mediator.Send(new ToggleWarehouseReceivingActiveCommand(userId, id, false));

            return StatusCode(result.StatusCode!.Value.GetHashCode(), result);
        }

        [HttpPatch("{id}/restore")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RestoreWarehouseReceiving(int id)
        {
            var userId = this.GetCurrentUserId();

            var result = await _mediator.Send(new ToggleWarehouseReceivingActiveCommand(userId, id, true));

            return StatusCode(result.StatusCode!.Value.GetHashCode(), result);
        }
    }
}
