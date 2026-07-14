using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Application.DTO.WarehouseReceiving;
using Application.Features.WarehouseReceivings.Commands;
using Application.Features.WarehouseReceivings.Queries;
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
    public class WarehouseReceivingController(IMediator mediator) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetWarehouseReceivings([FromQuery] GenericFiltersDto genericFiltersDto,
            [FromQuery] Sort sort)
        {
            GetAllResult<List<GetWarehouseReceivingDto>> result =
                await mediator.Send(new GetWarehouseReceivingsQuery(genericFiltersDto, sort));

            return StatusCode((int)result.StatusCode!.Value, result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetWarehouseReceivingById(int id)
        {
            Result<GetWarehouseReceivingDto> result = await mediator.Send(new GetWarehouseReceivingByIdQuery(id));

            return StatusCode((int)result.StatusCode!.Value, result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddWarehouseReceiving([FromBody] AddWarehouseReceivingDto addWarehouseDto)
        {
            int? userId = this.GetCurrentUserId();

            Result<object> result = await mediator.Send(new AddWarehouseReceivingCommand(userId, addWarehouseDto));

            return StatusCode((int)result.StatusCode!.Value, result);
        }

        [HttpPatch("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateWarehouse(int id,
            [FromBody] UpdateWarehouseReceivingDto updateWarehouseDto)
        {
            int? userId = this.GetCurrentUserId();

            Result<object> result =
                await mediator.Send(new UpdateWarehouseReceivingCommand(userId, id, updateWarehouseDto));

            return StatusCode((int)result.StatusCode!.Value, result);
        }

        [HttpPatch("{id:int}/archive")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ArchiveWarehouseReceiving(int id)
        {
            int? userId = this.GetCurrentUserId();

            Result<object> result = await mediator.Send(new ToggleWarehouseReceivingActiveCommand(userId, id, false));

            return StatusCode((int)result.StatusCode!.Value, result);
        }

        [HttpPatch("{id:int}/restore")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RestoreWarehouseReceiving(int id)
        {
            int? userId = this.GetCurrentUserId();

            Result<object> result = await mediator.Send(new ToggleWarehouseReceivingActiveCommand(userId, id, true));

            return StatusCode((int)result.StatusCode!.Value, result);
        }
    }
}