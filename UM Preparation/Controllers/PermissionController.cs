using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Application.DTO.Permission;
using Application.Features.Permissions.Commands;
using Application.Features.Permissions.Queries;
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
    public class PermissionController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetPermissions([FromQuery] GenericFiltersDto genericFiltersDto,
            [FromQuery] Sort sort)
        {
            GetAllResult<List<GetPermissionDto>> result =
                await _mediator.Send(new GetPermissionsQuery(genericFiltersDto, sort));

            return StatusCode((int)result.StatusCode!.Value, result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPermissionById(int id)
        {
            Result<GetPermissionDto> result = await _mediator.Send(new GetPermissionByIdQuery(id));

            return StatusCode((int)result.StatusCode!.Value, result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddPermission([FromBody] AddPermissionDto addPermissionDto)
        {
            int? userId = this.GetCurrentUserId();

            Result<object> result = await _mediator.Send(new AddPermissionCommand(userId, addPermissionDto));

            return StatusCode((int)result.StatusCode!.Value, result);
        }

        [HttpPatch("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdatePermission(int id, [FromBody] UpdatePermissionDto updatePermissionDto)
        {
            int? userId = this.GetCurrentUserId();

            Result<object> result = await _mediator.Send(new UpdatePermissionCommand(userId, id, updatePermissionDto));

            return StatusCode((int)result.StatusCode!.Value, result);
        }

        [HttpPatch("{id:int}/archive")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ArchivePond(int id)
        {
            int? userId = this.GetCurrentUserId();

            Result<object> result = await _mediator.Send(new TogglePermissionActiveCommand(userId, id, false));

            return StatusCode((int)result.StatusCode!.Value, result);
        }

        [HttpPatch("{id:int}/restore")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RestorePond(int id)
        {
            int? userId = this.GetCurrentUserId();

            Result<object> result = await _mediator.Send(new TogglePermissionActiveCommand(userId, id, true));

            return StatusCode((int)result.StatusCode!.Value, result);
        }
    }
}