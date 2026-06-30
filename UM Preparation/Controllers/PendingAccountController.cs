using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Application.DTO.PendingAccount;
using Application.Features.PendingAccounts.Commands;
using Application.Features.PendingAccounts.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UM_Preparation.Attributes;

namespace UM_Preparation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PendingAccountController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost]
        [ApiKey]
        public async Task<IActionResult> CreatePendingAccount([FromBody] AddPendingAccountDTO createPendingAccountDTO)
        {
            var result = await _mediator.Send(new AddPendingAccountCommand(createPendingAccountDTO));

            return StatusCode(result.StatusCode!.Value.GetHashCode(), result);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPendingAccounts([FromQuery] GenericFiltersDTO genericFiltersDTO, [FromQuery] Sort sort)
        {
            var result = await _mediator.Send(new GetPendingAccountsQuery(genericFiltersDTO, sort));

            return StatusCode(result.StatusCode!.Value.GetHashCode(), result);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetPendingAccountById(int id)
        {
            var result = await _mediator.Send(new GetPendingAccountByIdQuery(id));

            return StatusCode(result.StatusCode!.Value.GetHashCode(), result);
        }

        [HttpPost("import")]
        [Authorize]
        public async Task<IActionResult> ImportPendingAccount(int id, [FromQuery] int roleId)
        {
            var result = await _mediator.Send(new ImportPendingAccountCommand(id, roleId));

            if (result.IsFailure)
            {
                return StatusCode(result.StatusCode!.Value.GetHashCode(), result);
            }

            return CreatedAtAction(nameof(UserController.GetUserById), "User", new { id = result.Value }, result);
        }
    }
}
