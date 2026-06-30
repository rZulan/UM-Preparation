using Application.DTO.User;
using Application.Interfaces;
using Application.Results;
using MediatR;
using System.Net;

namespace Application.Features.Users.Commands
{
    /// <summary>Command to change the password of an existing user.</summary>
    /// <param name="Id">The ID of the user whose password is being changed.</param>
    /// <param name="UpdatePasswordDTO">The current and new password data.</param>
    public record ChangeUserPasswordCommand(int? Id, UpdatePasswordDTO UpdatePasswordDTO) : IRequest<Result<object>>;
    public class ChangeUserPasswordCommandHandler(IUserRepository userRepository, IPasswordHasherService passwordHasher, IRoleRepository roleRepository) : IRequestHandler<ChangeUserPasswordCommand, Result<object>>
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IPasswordHasherService _paswordHasher = passwordHasher;
        private readonly IRoleRepository _roleRepository = roleRepository;

        public async Task<Result<object>> Handle(ChangeUserPasswordCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _userRepository.GetByIdAsync(request.Id!.Value, cancellationToken);

            if (existingUser == null)
            {
                return Result<object>.Failure("User not found", HttpStatusCode.NotFound);
            }

            if (_paswordHasher.Verify(request.UpdatePasswordDTO.CurrentPassword, existingUser.PasswordHash) == false)
            {
                return Result<object>.Failure("Current password is incorrect", HttpStatusCode.BadRequest);
            }

            existingUser.PasswordHash = _paswordHasher.Hash(request.UpdatePasswordDTO.NewPassword);

            await _userRepository.UpdateAsync(existingUser, cancellationToken);

            return Result<object>.Success(null, "User updated successfully", HttpStatusCode.OK);
        }
    }
}
