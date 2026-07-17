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
        [HttpGet]
        public async Task<IActionResult> GetMiscellaneousReceipts([FromQuery] GenericFiltersDto genericFiltersDto,
            [FromQuery] Sort sort, CancellationToken cancellationToken)
        {
            GetAllResult<List<GetMiscellaneousReceiptDto>> result =
                await mediator.Send(new GetMiscellaneousReceiptsQuery(genericFiltersDto, sort), cancellationToken);
            return StatusCode((int)result.StatusCode!.Value, result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetMiscellaneousReceiptById(int id, CancellationToken cancellationToken)
        {
            Result<GetMiscellaneousReceiptDto> result =
                await mediator.Send(new GetMiscellaneousReceiptByIdQuery(id), cancellationToken);

            return StatusCode((int)result.StatusCode!.Value, result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddMiscellaneousReceipt(
            [FromBody] AddMiscellaneousReceiptDto addMiscellaneousReceiptDto,
            CancellationToken cancellationToken)
        {
            int? userId = this.GetCurrentUserId();

            Result<object> result =
                await mediator.Send(new AddMiscellaneousReceiptCommand(userId, addMiscellaneousReceiptDto),
                    cancellationToken);
            return StatusCode((int)result.StatusCode!.Value, result);
        }
    }
}
