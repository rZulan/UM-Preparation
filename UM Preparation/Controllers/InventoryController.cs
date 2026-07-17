using Application.DTO.Inventory;
using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Application.Features.Inventory.Queries;
using Application.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace UM_Preparation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InventoryController(IMediator mediator) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetInventory([FromQuery] [BindRequired] int warehouseId,
            [FromQuery] GenericFiltersDto genericFiltersDto, [FromQuery] Sort sort,
            CancellationToken cancellationToken)
        {
            GetAllResult<GetInventoryDto> result = await mediator.Send(
                new GetInventoryQuery(warehouseId, genericFiltersDto, sort), cancellationToken);

            return StatusCode((int)result.StatusCode!.Value, result);
        }

        [HttpGet]
        [Route("receiving")]
        public async Task<IActionResult> GetInventoryReceiving([FromQuery] [BindRequired] int warehouseId,
            [FromQuery] GenericFiltersDto genericFiltersDto, [FromQuery] Sort sort,
            CancellationToken cancellationToken)
        {
            GetAllResult<GetInventoryReceivingDto> result = await mediator.Send(
                new GetInventoryReceivingsQuery(warehouseId, genericFiltersDto, sort), cancellationToken);
            return StatusCode((int)result.StatusCode!.Value, result);
        }

        [HttpGet]
        [Route("receiving/{productId:int}")]
        public async Task<IActionResult> GetInventoryReceiving(
            [FromQuery] [BindRequired] int warehouseId,
            int productId,
            [FromQuery] GenericFiltersDto genericFiltersDto,
            [FromQuery] Sort sort,
            CancellationToken cancellationToken)
        {
            GetAllResult<GetInventoryReceivingDto> result = await mediator.Send(
                new GetInventoryReceivingByProductQuery(warehouseId, productId, genericFiltersDto, sort),
                cancellationToken);

            return StatusCode((int)result.StatusCode!.Value, result);
        }
    }
}
