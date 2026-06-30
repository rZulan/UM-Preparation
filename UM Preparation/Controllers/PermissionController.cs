using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Application.DTO.Permission;
using Application.Features.Permissions.Commands;
using Application.Features.Permissions.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UM_Preparation.Extensions;

namespace UM_Preparation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PermissionController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetPermissions([FromQuery] GenericFiltersDTO genericFiltersDTO, [FromQuery] Sort sort)
        {
            var result = await _mediator.Send(new GetPermissionsQuery(genericFiltersDTO, sort));

            return StatusCode(result.StatusCode!.Value.GetHashCode(), result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPermissionById(int id)
        {
            var result = await _mediator.Send(new GetPermissionByIdQuery(id));

            return StatusCode(result.StatusCode!.Value.GetHashCode(), result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddPermission([FromBody] AddPermissionDTO addPermissionDTO)
        {
            var userId = this.GetCurrentUserId();

            var result = await _mediator.Send(new AddPermissionCommand(userId, addPermissionDTO));

            return StatusCode(result.StatusCode!.Value.GetHashCode(), result);
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdatePermission(int id, [FromBody] UpdatePermissionDTO updatePermissionDTO)
        {
            var userId = this.GetCurrentUserId();

            var result = await _mediator.Send(new UpdatePermissionCommand(userId, id, updatePermissionDTO));

            return StatusCode(result.StatusCode!.Value.GetHashCode(), result);
        }

        [HttpPatch("{id}/archive")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ArchivePond(int id)
        {
            var userId = this.GetCurrentUserId();

            var result = await _mediator.Send(new TogglePermissionActiveCommand(userId, id, false));

            return StatusCode(result.StatusCode!.Value.GetHashCode(), result);
        }

        [HttpPatch("{id}/restore")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RestorePond(int id)
        {
            var userId = this.GetCurrentUserId();

            var result = await _mediator.Send(new TogglePermissionActiveCommand(userId, id, true));

            return StatusCode(result.StatusCode!.Value.GetHashCode(), result);
        }
    }
}
