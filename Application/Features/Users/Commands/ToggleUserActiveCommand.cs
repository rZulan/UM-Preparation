using Application.Interfaces;
using Application.Results;
using MediatR;

namespace Application.Features.Users.Commands
{
    /// <summary>Command to activate or deactivate a user account.</summary>
    /// <param name="Id">The ID of the user to toggle.</param>
    /// <param name="Toggle">The desired active state.</param>
    public record ToggleUserActiveCommand(int Id, bool Toggle) : IRequest<Result<object>>;
    public class ToggleUserActiveCommandHandler(IUserRepository userRepository) : IRequestHandler<ToggleUserActiveCommand, Result<object>>
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<Result<object>> Handle(ToggleUserActiveCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _userRepository.GetByIdAsync(request.Id, cancellationToken);

            if (existingUser == null)
            {
                return Result<object>.Failure("User not found", System.Net.HttpStatusCode.NotFound);
            }

            if (request.Toggle && existingUser.IsActive)
            {
                return Result<object>.Failure("User is already active");
            }

            if (!request.Toggle && !existingUser.IsActive)
            {
                return Result<object>.Failure("User is already archived");
            }

            existingUser.IsActive = request.Toggle;

            await _userRepository.UpdateAsync(existingUser, cancellationToken);

            var status = existingUser.IsActive ? "restored" : "archived";

            return Result<object>.Success(null, $"User {status} successfully");
        }
    }
}
