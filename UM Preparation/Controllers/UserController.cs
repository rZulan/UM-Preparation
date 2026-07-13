using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Application.DTO.User;
using Application.Features.Users.Commands;
using Application.Features.Users.Queries;
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
    public class UserController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] GenericFiltersDto genericFiltersDto, [FromQuery] Sort sort)
        {
            GetAllResult<List<GetUserDto>> result = await _mediator.Send(new GetUsersQuery(genericFiltersDto, sort));

            return StatusCode((int)result.StatusCode!.Value, result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            Result<GetUserDto> result = await _mediator.Send(new GetUserByIdQuery(id));

            return StatusCode((int)result.StatusCode!.Value, result);
        }

        [HttpPatch("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] UpdatePasswordDto updatePasswordDto)
        {
            int? userId = this.GetCurrentUserId();

            Result<object> result = await _mediator.Send(new ChangeUserPasswordCommand(userId, updatePasswordDto));

            return StatusCode((int)result.StatusCode!.Value, result);
        }

        [HttpPatch("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto updateUserDto)
        {
            Result<object> result = await _mediator.Send(new UpdateUserCommand(id, updateUserDto));

            return StatusCode((int)result.StatusCode!.Value, result);
        }

        [HttpPatch("{id:int}/archive")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ArchiveUser(int id)
        {
            Result<object> result = await _mediator.Send(new ToggleUserActiveCommand(id, false));

            return StatusCode((int)result.StatusCode!.Value, result);
        }

        [HttpPatch("{id:int}/restore")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RestoreUser(int id)
        {
            Result<object> result = await _mediator.Send(new ToggleUserActiveCommand(id, true));

            return StatusCode((int)result.StatusCode!.Value, result);
        }
    }
}

