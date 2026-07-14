using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Application.DTO.PendingAccount;
using Application.Features.PendingAccounts.Commands;
using Application.Features.PendingAccounts.Queries;
using Application.Results;
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
        public async Task<IActionResult> CreatePendingAccount([FromBody] AddPendingAccountDto createPendingAccountDto)
        {
            Result<object> result = await _mediator.Send(new AddPendingAccountCommand(createPendingAccountDto));

            return StatusCode((int)result.StatusCode!.Value, result);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPendingAccounts([FromQuery] GenericFiltersDto genericFiltersDto,
            [FromQuery] Sort sort)
        {
            GetAllResult<List<GetPendingAccountDto>> result =
                await _mediator.Send(new GetPendingAccountsQuery(genericFiltersDto, sort));

            return StatusCode((int)result.StatusCode!.Value, result);
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetPendingAccountById(int id)
        {
            Result<GetPendingAccountDto> result = await _mediator.Send(new GetPendingAccountByIdQuery(id));

            return StatusCode((int)result.StatusCode!.Value, result);
        }

        [HttpPost("import")]
        [Authorize]
        public async Task<IActionResult> ImportPendingAccount(int id,
            [FromQuery] ImportPendingAccountDto importPendingAccountDto)
        {
            Result<object> result = await _mediator.Send(new ImportPendingAccountCommand(id, importPendingAccountDto));

            if (result.IsFailure)
            {
                return StatusCode((int)result.StatusCode!.Value, result);
            }

            return CreatedAtAction(nameof(UserController.GetUserById), "User", new { id = result.Value }, result);
        }
    }
}