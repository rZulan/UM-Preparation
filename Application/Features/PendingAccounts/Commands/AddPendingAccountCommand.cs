using Application.DTO.PendingAccount;
using Application.Interfaces;
using Application.Results;
using Domain.Entities;
using MediatR;
using System.Net;

namespace Application.Features.PendingAccounts.Commands
{
    /// <summary>Command to register a new pending account from an employee ID.</summary>
    /// <param name="AddPendingAccountDTO">The pending account data to be created.</param>
    public record AddPendingAccountCommand(AddPendingAccountDTO AddPendingAccountDTO) : IRequest<Result<object>>;
    public class AddPendingAccountCommandHandler(IPendingAccountRepository pendingAccountRepository, IUserRepository userRepository, IPasswordHasherService passwordHasher) : IRequestHandler<AddPendingAccountCommand, Result<object>>
    {
        private readonly IPendingAccountRepository _pendingAccountRepository = pendingAccountRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IPasswordHasherService _paswordHasher = passwordHasher;

        public async Task<Result<object>> Handle(AddPendingAccountCommand request, CancellationToken cancellationToken)
        {
            var existingPendingAccount = await _pendingAccountRepository.GetByFullIdNoAsync(request.AddPendingAccountDTO.id_prefix, request.AddPendingAccountDTO.id_no, cancellationToken);

            if (existingPendingAccount != null)
            {
                existingPendingAccount.Username = request.AddPendingAccountDTO.Username;
                existingPendingAccount.Password = request.AddPendingAccountDTO.Password;
                existingPendingAccount.FirstName = request.AddPendingAccountDTO.first_name;
                existingPendingAccount.MiddleName = request.AddPendingAccountDTO.middle_name;
                existingPendingAccount.LastName = request.AddPendingAccountDTO.last_name;
                existingPendingAccount.Suffix = request.AddPendingAccountDTO.Suffix;

                await _pendingAccountRepository.UpdateAsync(existingPendingAccount, cancellationToken);

                return Result<object>.Success(existingPendingAccount.Id, "Pending Account updated successfully", HttpStatusCode.OK);
            }

            var existingUserAccount = await _userRepository.GetByFullIdNoAsync(request.AddPendingAccountDTO.id_prefix, request.AddPendingAccountDTO.id_no, cancellationToken);

            if (existingUserAccount != null)
            {
                var hashPassword = _paswordHasher.Hash(request.AddPendingAccountDTO.Password);

                existingUserAccount.Username = request.AddPendingAccountDTO.Username;
                existingUserAccount.PasswordHash = hashPassword;
                existingUserAccount.FirstName = request.AddPendingAccountDTO.first_name;
                existingUserAccount.MiddleName = request.AddPendingAccountDTO.middle_name;
                existingUserAccount.LastName = request.AddPendingAccountDTO.last_name;
                existingUserAccount.Suffix = request.AddPendingAccountDTO.Suffix;

                await _userRepository.UpdateAsync(existingUserAccount, cancellationToken);

                return Result<object>.Success(existingUserAccount.Id, "User Account updated successfully", HttpStatusCode.OK);
            }

            var pendingAccount = new PendingAccount
            {
                EmployeePrefix = request.AddPendingAccountDTO.id_prefix,
                EmployeeId = request.AddPendingAccountDTO.id_no,
                Username = request.AddPendingAccountDTO.Username,
                Password = request.AddPendingAccountDTO.Password,
                FirstName = request.AddPendingAccountDTO.first_name,
                MiddleName = request.AddPendingAccountDTO.middle_name,
                LastName = request.AddPendingAccountDTO.last_name,
                Suffix = request.AddPendingAccountDTO.Suffix,
                CreatedAt = DateTime.UtcNow,
            };

            await _pendingAccountRepository.AddAsync(pendingAccount, cancellationToken);

            return Result<object>.Success(pendingAccount.Id, "Pending Account created successfully", HttpStatusCode.Created);
        }
    }
}
