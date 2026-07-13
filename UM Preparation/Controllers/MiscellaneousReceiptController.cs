using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Application.DTO.MiscellaneousReceipt;
using Application.Features.MiscellaneousReceipts.Commands;
using Application.Features.MiscellaneousReceipts.Queries;
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
    public class MiscellaneousReceiptController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetMiscellaneousReceipts([FromQuery] GenericFiltersDto genericFiltersDto, [FromQuery] Sort sort)
        {
            GetAllResult<List<GetMiscellaneousReceiptDto>> result = await _mediator.Send(new GetMiscellaneousReceiptsQuery(genericFiltersDto, sort));
            return StatusCode((int)result.StatusCode!.Value, result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetMiscellaneousReceiptById(int id)
        {
            Result<GetMiscellaneousReceiptDto> result = await _mediator.Send(new GetMiscellaneousReceiptByIdQuery(id));

            return StatusCode((int)result.StatusCode!.Value, result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddMiscellaneousReceipt([FromBody] AddMiscellaneousReceiptDto addMiscellaneousReceiptDto)
        {
            int? userId = this.GetCurrentUserId();

            Result<object> result = await _mediator.Send(new AddMiscellaneousReceiptCommand(userId, addMiscellaneousReceiptDto));
            return StatusCode((int)result.StatusCode!.Value, result);
        }
    }
}
