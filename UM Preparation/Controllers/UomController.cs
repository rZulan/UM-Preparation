using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Application.DTO.Uom;
using Application.Features.Uoms.Commands;
using Application.Features.Uoms.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UM_Preparation.Extensions;

namespace UM_Preparation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UomController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetUoms([FromQuery] GenericFiltersDTO genericFiltersDTO, [FromQuery] Sort sort)
        {
            var result = await _mediator.Send(new GetUomsQuery(genericFiltersDTO, sort));

            return StatusCode(result.StatusCode!.Value.GetHashCode(), result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUomById(int id)
        {
            var result = await _mediator.Send(new GetUomByIdQuery(id));

            return StatusCode(result.StatusCode!.Value.GetHashCode(), result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddUom([FromBody] AddUomDTO addUomDTO)
        {
            var userId = this.GetCurrentUserId();

            var result = await _mediator.Send(new AddUomCommand(userId, addUomDTO));

            return StatusCode(result.StatusCode!.Value.GetHashCode(), result);
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUom(int id, [FromBody] UpdateUomDTO updateUomDTO)
        {
            var userId = this.GetCurrentUserId();

            var result = await _mediator.Send(new UpdateUomCommand(userId, id, updateUomDTO));

            return StatusCode(result.StatusCode!.Value.GetHashCode(), result);
        }

        [HttpPatch("{id}/archive")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ArchiveUom(int id)
        {
            var userId = this.GetCurrentUserId();

            var result = await _mediator.Send(new ToggleUomActiveCommand(userId, id, false));

            return StatusCode(result.StatusCode!.Value.GetHashCode(), result);
        }

        [HttpPatch("{id}/restore")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RestoreUom(int id)
        {
            var userId = this.GetCurrentUserId();

            var result = await _mediator.Send(new ToggleUomActiveCommand(userId, id, true));

            return StatusCode(result.StatusCode!.Value.GetHashCode(), result);
        }
    }
}