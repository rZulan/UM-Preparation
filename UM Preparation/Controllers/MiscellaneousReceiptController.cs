using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Application.DTO.MiscellaneousReceipt;
using Application.Features.MiscellaneousReceipts.Commands;
using Application.Features.MiscellaneousReceipts.Queries;
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
        public async Task<IActionResult> GetMiscellaneousReceipts([FromQuery] GenericFiltersDTO genericFiltersDTO, [FromQuery] Sort sort)
        {
            var result = await _mediator.Send(new GetMiscellaneousReceiptsQuery(genericFiltersDTO, sort));
            return StatusCode(result.StatusCode!.Value.GetHashCode(), result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMiscellaneousReceiptById(int id)
        {
            var result = await _mediator.Send(new GetMiscellaneousReceiptByIdQuery(id));

            return StatusCode(result.StatusCode!.Value.GetHashCode(), result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddMiscellaneousReceipt([FromBody] AddMiscellaneousReceiptDTO addMiscellaneousReceiptDTO)
        {
            var userId = this.GetCurrentUserId();

            var result = await _mediator.Send(new AddMiscellaneousReceiptCommand(userId, addMiscellaneousReceiptDTO));
            return StatusCode(result.StatusCode!.Value.GetHashCode(), result);
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateMiscellaneousReceipt(int id, [FromBody] UpdateMiscellaneousReceiptDTO updateMiscellaneousReceiptDTO)
        {
            var userId = this.GetCurrentUserId();

            var result = await _mediator.Send(new UpdateMiscellaneousReceiptCommand(userId, id, updateMiscellaneousReceiptDTO));

            return StatusCode(result.StatusCode!.Value.GetHashCode(), result);
        }

        [HttpPatch("{id}/archive")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ArchiveMiscellaneousReceipt(int id)
        {
            var userId = this.GetCurrentUserId();

            var result = await _mediator.Send(new ToggleMiscellaneousReceiptActiveCommand(userId, id, false));

            return StatusCode(result.StatusCode!.Value.GetHashCode(), result);
        }

        [HttpPatch("{id}/restore")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RestoreMiscellaneousReceipt(int id)
        {
            var userId = this.GetCurrentUserId();

            var result = await _mediator.Send(new ToggleMiscellaneousReceiptActiveCommand(userId, id, true));

            return StatusCode(result.StatusCode!.Value.GetHashCode(), result);
        }
    }
}