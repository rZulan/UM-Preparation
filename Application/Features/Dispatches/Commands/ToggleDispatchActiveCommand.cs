using Application.Interfaces;
using Application.Results;
using MediatR;
using System.Net;

namespace Application.Features.Dispatches.Commands
{
    /// <summary>Command to activate or deactivate a dispatch.</summary>
    /// <param name="UserId">The ID of the authenticated user performing the action.</param>
    /// <param name="Id">The ID of the dispatch to toggle.</param>
    /// <param name="IsActive">The desired active state.</param>
    public record ToggleDispatchActiveCommand(int? UserId, int Id, bool IsActive) : IRequest<Result<object>>;
    public class ToggleDispatchActiveCommandHandler(IDispatchRepository dispatchRepository, IUserRepository userRepository) : IRequestHandler<ToggleDispatchActiveCommand, Result<object>>
    {
        private readonly IDispatchRepository _dispatchRepository = dispatchRepository;
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<Result<object>> Handle(ToggleDispatchActiveCommand request, CancellationToken cancellationToken)
        {
            if (request.UserId == null)
            {
                return Result<object>.Failure("User is not signed in", HttpStatusCode.Unauthorized);
            }

            var existingUser = await _userRepository.GetByIdAsync(request.UserId.Value, cancellationToken);

            if (existingUser == null)
            {
                return Result<object>.Failure("User not found", HttpStatusCode.NotFound);
            }

            var existing = await _dispatchRepository.GetByIdAsync(request.Id, cancellationToken);

            if (existing == null)
            {
                return Result<object>.Failure("Dispatch not found");
            }

            if (request.IsActive && existing.IsActive)
            {
                return Result<object>.Failure("Dispatch is already active");
            }

            if (!request.IsActive && !existing.IsActive)
            {
                return Result<object>.Failure("Dispatch is already archived");
            }

            existing.IsActive = request.IsActive;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedById = existingUser.Id;

            await _dispatchRepository.UpdateAsync(existing, cancellationToken);

            var status = existing.IsActive ? "restored" : "archived";

            return Result<object>.Success(existing.Id, $"Dispatch {status} successfully");
        }
    }
}