using Application.DTO.User;
using Application.Interfaces;
using Application.Results;
using Domain.Entities.Junction;
using MediatR;
using System.Net;

namespace Application.Features.Users.Commands
{
    /// <summary>Command to update an existing user's profile and role assignments.</summary>
    /// <param name="Id">The ID of the user to update.</param>
    /// <param name="UpdateDTO">The updated user data.</param>
    public record UpdateUserCommand(int Id, UpdateUserDTO UpdateDTO) : IRequest<Result<object>>;
    public class UpdateUserCommandHandler(IUserRepository userRepository, IPasswordHasherService passwordHasher, IRoleRepository roleRepository) : IRequestHandler<UpdateUserCommand, Result<object>>
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IPasswordHasherService _paswordHasher = passwordHasher;
        private readonly IRoleRepository _roleRepository = roleRepository;

        public async Task<Result<object>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _userRepository.GetByIdAsync(request.Id, cancellationToken);

            if (existingUser == null)
            {
                return Result<object>.Failure("User not found", HttpStatusCode.NotFound);
            }

            if (!string.IsNullOrEmpty(request.UpdateDTO.Username))
            {
                var existingUsername = await _userRepository.CheckDuplicateAsync(request.Id, request.UpdateDTO.Username, cancellationToken);

                if (existingUsername)
                {
                    return Result<object>.Failure("Username already exists", HttpStatusCode.BadRequest);
                }

                existingUser.Username = request.UpdateDTO.Username;
            }

            if (!string.IsNullOrEmpty(request.UpdateDTO.UpdatePassword))
            {
                existingUser.PasswordHash = _paswordHasher.Hash(request.UpdateDTO.UpdatePassword);
            }

            if (!string.IsNullOrEmpty(request.UpdateDTO.FirstName))
            {
                existingUser.FirstName = request.UpdateDTO.FirstName;
            }

            if (!string.IsNullOrEmpty(request.UpdateDTO.MiddleName))
            {
                existingUser.MiddleName = request.UpdateDTO.MiddleName;
            }

            if (!string.IsNullOrEmpty(request.UpdateDTO.LastName))
            {
                existingUser.LastName = request.UpdateDTO.LastName;
            }

            if (!string.IsNullOrEmpty(request.UpdateDTO.Suffix))
            {
                existingUser.Suffix = request.UpdateDTO.Suffix;
            }

            if (!string.IsNullOrEmpty(request.UpdateDTO.IDPrefix))
            {
                existingUser.IDPrefix = request.UpdateDTO.IDPrefix;
            }

            if (!string.IsNullOrEmpty(request.UpdateDTO.IDNumber))
            {
                existingUser.IDNumber = request.UpdateDTO.IDNumber;
            }

            if (request.UpdateDTO.RoleId.HasValue)
            {
                var existingRole = await _roleRepository.GetByIdAsync(request.UpdateDTO.RoleId.Value, cancellationToken);

                if (existingRole == null)
                {
                    return Result<object>.Failure("Role not found", HttpStatusCode.NotFound);
                }

                existingUser.UserRoles.Clear();

                existingUser.UserRoles.Add(new UserRoles
                {
                    RoleId = existingRole.Id,
                    UserId = existingUser.Id
                });
            }

            await _userRepository.UpdateAsync(existingUser, cancellationToken);

            return Result<object>.Success(null, "User updated successfully", HttpStatusCode.OK);
        }
    }
}
