using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Application.DTO.Role;
using Application.Features.Roles.Commands;
using Application.Features.Roles.Queries;
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
    public class RoleController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetRoles([FromQuery] GenericFiltersDto genericFiltersDto, [FromQuery] Sort sort)
        {
            GetAllResult<List<GetRoleDto>> result = await _mediator.Send(new GetRolesQuery(genericFiltersDto, sort));

            return StatusCode((int)result.StatusCode!.Value, result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetRoleById(int id)
        {
            Result<GetRoleDto> result = await _mediator.Send(new GetRoleByIdQuery(id));

            return StatusCode((int)result.StatusCode!.Value, result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddRole([FromBody] AddRoleDto addRoleDto)
        {
            int? userId = this.GetCurrentUserId();

            Result<object> result = await _mediator.Send(new AddRoleCommand(userId, addRoleDto));

            return StatusCode((int)result.StatusCode!.Value, result);
        }

        [HttpPatch("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] UpdateRoleDto updateRoleDto)
        {
            int? userId = this.GetCurrentUserId();

            Result<object> result = await _mediator.Send(new UpdateRoleCommand(userId, id, updateRoleDto));

            return StatusCode((int)result.StatusCode!.Value, result);
        }

        [HttpPatch("{id:int}/archive")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ArchiveRole(int id)
        {
            int? userId = this.GetCurrentUserId();

            Result<object> result = await _mediator.Send(new ToggleRoleActiveCommand(userId, id, false));

            return StatusCode((int)result.StatusCode!.Value, result);
        }

        [HttpPatch("{id:int}/restore")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RestoreRole(int id)
        {
            int? userId = this.GetCurrentUserId();

            Result<object> result = await _mediator.Send(new ToggleRoleActiveCommand(userId, id, true));

            return StatusCode((int)result.StatusCode!.Value, result);
        }
    }
}

