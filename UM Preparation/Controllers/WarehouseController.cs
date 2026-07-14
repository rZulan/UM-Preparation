using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Application.DTO.Warehouse;
using Application.Features.Warehouses.Commands;
using Application.Features.Warehouses.Queries;
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
    public class WarehouseController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetWarehouses([FromQuery] GenericFiltersDto genericFiltersDto,
            [FromQuery] Sort sort)
        {
            GetAllResult<List<GetWarehouseDto>> result =
                await _mediator.Send(new GetWarehousesQuery(genericFiltersDto, sort));

            return StatusCode((int)result.StatusCode!.Value, result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetWarehouseById(int id)
        {
            Result<GetWarehouseDto> result = await _mediator.Send(new GetWarehouseByIdQuery(id));

            return StatusCode((int)result.StatusCode!.Value, result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddWarehouse([FromBody] AddWarehouseDto addWarehouseDto)
        {
            int? userId = this.GetCurrentUserId();

            Result<object> result = await _mediator.Send(new AddWarehouseCommand(userId, addWarehouseDto));

            return StatusCode((int)result.StatusCode!.Value, result);
        }

        [HttpPatch("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateWarehouse(int id, [FromBody] UpdateWarehouseDto updateWarehouseDto)
        {
            int? userId = this.GetCurrentUserId();

            Result<object> result = await _mediator.Send(new UpdateWarehouseCommand(userId, id, updateWarehouseDto));

            return StatusCode((int)result.StatusCode!.Value, result);
        }

        [HttpPatch("{id:int}/archive")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ArchiveWarehouse(int id)
        {
            int? userId = this.GetCurrentUserId();

            Result<object> result = await _mediator.Send(new ToggleWarehouseActiveCommand(userId, id, false));

            return StatusCode((int)result.StatusCode!.Value, result);
        }

        [HttpPatch("{id:int}/restore")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RestoreWarehouse(int id)
        {
            int? userId = this.GetCurrentUserId();

            Result<object> result = await _mediator.Send(new ToggleWarehouseActiveCommand(userId, id, true));

            return StatusCode((int)result.StatusCode!.Value, result);
        }
    }
}