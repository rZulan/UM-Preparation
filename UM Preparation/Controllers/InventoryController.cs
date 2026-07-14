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
        public async Task<IActionResult> GetInventory([FromQuery, BindRequired] int warehouseId)
        {
            Result<GetInventoryDto> result = await _mediator.Send(new GetInventoryQuery(warehouseId));

            return StatusCode((int)result.StatusCode!.Value, result);
        }
    }
}
