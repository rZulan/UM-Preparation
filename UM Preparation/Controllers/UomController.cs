using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Application.DTO.Uom;
using Application.Features.Uoms.Commands;
using Application.Features.Uoms.Queries;
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
    public class UomController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetUoms([FromQuery] GenericFiltersDto genericFiltersDto, [FromQuery] Sort sort)
        {
            GetAllResult<List<GetUomDto>> result = await _mediator.Send(new GetUomsQuery(genericFiltersDto, sort));

            return StatusCode((int)result.StatusCode!.Value, result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUomById(int id)
        {
            Result<GetUomDto> result = await _mediator.Send(new GetUomByIdQuery(id));

            return StatusCode((int)result.StatusCode!.Value, result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddUom([FromBody] AddUomDto addUomDto)
        {
            int? userId = this.GetCurrentUserId();

            Result<object> result = await _mediator.Send(new AddUomCommand(userId, addUomDto));

            return StatusCode((int)result.StatusCode!.Value, result);
        }

        [HttpPatch("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUom(int id, [FromBody] UpdateUomDto updateUomDto)
        {
            int? userId = this.GetCurrentUserId();

            Result<object> result = await _mediator.Send(new UpdateUomCommand(userId, id, updateUomDto));

            return StatusCode((int)result.StatusCode!.Value, result);
        }

        [HttpPatch("{id:int}/archive")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ArchiveUom(int id)
        {
            int? userId = this.GetCurrentUserId();

            Result<object> result = await _mediator.Send(new ToggleUomActiveCommand(userId, id, false));

            return StatusCode((int)result.StatusCode!.Value, result);
        }

        [HttpPatch("{id:int}/restore")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RestoreUom(int id)
        {
            int? userId = this.GetCurrentUserId();

            Result<object> result = await _mediator.Send(new ToggleUomActiveCommand(userId, id, true));

            return StatusCode((int)result.StatusCode!.Value, result);
        }
    }
}
