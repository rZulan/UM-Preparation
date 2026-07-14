using Application.DTO.Inventory;
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
        private readonly IMediator _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetInventory([FromQuery] [BindRequired] int warehouseId)
        {
            Result<GetInventoryDto> result = await _mediator.Send(new GetInventoryQuery(warehouseId));

            return StatusCode((int)result.StatusCode!.Value, result);
        }

        [HttpGet]
        [Route("receiving")]
        public async Task<IActionResult> GetInventoryReceiving([FromQuery] [BindRequired] int warehouseId)
        {
            Result<GetInventoryReceivingDto> result =
                await _mediator.Send(new GetInventoryReceivingsQuery(warehouseId));

            return StatusCode((int)result.StatusCode!.Value, result);
        }

        [HttpGet]
        [Route("receiving/{productId:int}")]
        public async Task<IActionResult> GetInventoryReceiving(
            [FromQuery] [BindRequired] int warehouseId,
            int productId)
        {
            Result<GetInventoryReceivingDto> result =
                await _mediator.Send(new GetInventoryReceivingByProductQuery(warehouseId, productId));

            return StatusCode((int)result.StatusCode!.Value, result);
        }
    }
}