using Application.DTO.Dispatch;
using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Application.Features.Dispatches.Commands;
using Application.Features.Dispatches.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UM_Preparation.Extensions;

namespace UM_Preparation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DispatchController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetDispatches([FromQuery] GenericFiltersDTO genericFiltersDTO, [FromQuery] Sort sort)
        {
            var result = await _mediator.Send(new GetDispatchesQuery(genericFiltersDTO, sort));
            return StatusCode(result.StatusCode!.Value.GetHashCode(), result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDispatchById(int id)
        {
            var result = await _mediator.Send(new GetDispatchByIdQuery(id));

            return StatusCode(result.StatusCode!.Value.GetHashCode(), result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddDispatch([FromBody] AddDispatchDTO addDispatchDTO)
        {
            var userId = this.GetCurrentUserId();

            var result = await _mediator.Send(new AddDispatchCommand(userId, addDispatchDTO));
            return StatusCode(result.StatusCode!.Value.GetHashCode(), result);
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateDispatch(int id, [FromBody] UpdateDispatchDTO updateDispatchDTO)
        {
            var userId = this.GetCurrentUserId();

            var result = await _mediator.Send(new UpdateDispatchCommand(userId, id, updateDispatchDTO));

            return StatusCode(result.StatusCode!.Value.GetHashCode(), result);
        }

        [HttpPatch("{id}/archive")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ArchiveDispatch(int id)
        {
            var userId = this.GetCurrentUserId();

            var result = await _mediator.Send(new ToggleDispatchActiveCommand(userId, id, false));

            return StatusCode(result.StatusCode!.Value.GetHashCode(), result);
        }

        [HttpPatch("{id}/restore")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RestoreDispatch(int id)
        {
            var userId = this.GetCurrentUserId();

            var result = await _mediator.Send(new ToggleDispatchActiveCommand(userId, id, true));

            return StatusCode(result.StatusCode!.Value.GetHashCode(), result);
        }
    }
}