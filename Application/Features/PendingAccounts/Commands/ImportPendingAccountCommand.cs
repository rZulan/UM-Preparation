using Application.DTO.PendingAccount;
using Application.DTO.User;
using Application.Features.Users.Commands;
using Application.Interfaces;
using Application.Results;
using MediatR;
using System.Net;

namespace Application.Features.PendingAccounts.Commands
{
    /// <summary>Command to import a pending account into the system as a registered user.</summary>
    /// <param name="Id">The ID of the pending account to import.</param>
    /// <param name="RoleId">The ID of the role to assign to the newly registered user.</param>
    public record ImportPendingAccountCommand(int Id, ImportPendingAccountDto ImportPendingAccountDTO) : IRequest<Result<object>>;
    public class ImportPendingAccountCommandHandler(IPendingAccountRepository pendingAccountRepository, IRoleRepository roleRepository, IWarehouseRepository warehouseRepository, IMediator mediator) : IRequestHandler<ImportPendingAccountCommand, Result<object>>
    {
        private readonly IPendingAccountRepository _pendingAccountRepository = pendingAccountRepository;
        private readonly IRoleRepository _roleRepository = roleRepository;
        private readonly IWarehouseRepository _warehouseRepository = warehouseRepository;
        private readonly IMediator _mediator = mediator;

        public async Task<Result<object>> Handle(ImportPendingAccountCommand request, CancellationToken cancellationToken)
        {
            var existingPendingAccount = await _pendingAccountRepository.GetByIdAsync(request.Id, cancellationToken);

            if (existingPendingAccount == null)
            {
                return Result<object>.Failure("Pending Account not found", HttpStatusCode.NotFound);
            }

            var existingRole = await _roleRepository.GetByIdAsync(request.ImportPendingAccountDTO.RoleId, cancellationToken);

            if (existingRole == null)
            {
                return Result<object>.Failure("Role not found", HttpStatusCode.NotFound);
            }

            if (request.ImportPendingAccountDTO.WarehouseId.HasValue)
            {
                var existingWarehouse = await _warehouseRepository.GetByIdAsync(request.ImportPendingAccountDTO.WarehouseId.Value, cancellationToken);

                if (existingWarehouse == null)
                {
                    return Result<object>.Failure("Warehouse not found", HttpStatusCode.NotFound);
                }
            }

            var registerDTO = new RegisterUserDto
            {
                Username = existingPendingAccount.Username,
                Password = existingPendingAccount.Password,
                FirstName = existingPendingAccount.FirstName,
                MiddleName = existingPendingAccount.MiddleName,
                LastName = existingPendingAccount.LastName,
                Suffix = existingPendingAccount.Suffix,
                IDPrefix = existingPendingAccount.EmployeePrefix,
                IDNumber = existingPendingAccount.EmployeeId,
                RoleId = existingRole.Id,
                WarehouseId = request.ImportPendingAccountDTO.WarehouseId
            };

            var result = await _mediator.Send(new RegisterUserCommand(registerDTO), cancellationToken);

            if (result.IsFailure)
            {
                return Result<object>.Failure("Failed to create user account from pending account: " + result.Message, HttpStatusCode.Conflict);
            }

            return Result<object>.Success(result.Value, "Pending Account created successfully", HttpStatusCode.Created);
        }
    }
}
