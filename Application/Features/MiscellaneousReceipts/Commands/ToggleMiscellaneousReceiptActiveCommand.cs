using Application.Interfaces;
using Application.Results;
using MediatR;
using System.Net;

namespace Application.Features.MiscellaneousReceipts.Commands
{
    /// <summary>Command to activate or deactivate a miscellaneous receipt.</summary>
    /// <param name="UserId">The ID of the authenticated user performing the action.</param>
    /// <param name="Id">The ID of the miscellaneous receipt to toggle.</param>
    /// <param name="IsActive">The desired active state.</param>
    public record ToggleMiscellaneousReceiptActiveCommand(int? UserId, int Id, bool IsActive) : IRequest<Result<object>>;
    public class ToggleMiscellaneousReceiptActiveCommandHandler(IMiscellaneousReceiptRepository miscellaneousReceipt, IUserRepository userRepository) : IRequestHandler<ToggleMiscellaneousReceiptActiveCommand, Result<object>>
    {
        private readonly IMiscellaneousReceiptRepository _miscellaneousReceipt = miscellaneousReceipt;
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<Result<object>> Handle(ToggleMiscellaneousReceiptActiveCommand request, CancellationToken cancellationToken)
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

            var existing = await _miscellaneousReceipt.GetByIdAsync(request.Id, cancellationToken);

            if (existing == null)
            {
                return Result<object>.Failure("Miscellaneous Receipt not found");
            }

            if (request.IsActive && existing.IsActive)
            {
                return Result<object>.Failure("Miscellaneous Receipt is already active");
            }

            if (!request.IsActive && !existing.IsActive)
            {
                return Result<object>.Failure("Miscellaneous Receipt is already archived");
            }

            existing.IsActive = request.IsActive;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedById = existingUser.Id;

            await _miscellaneousReceipt.UpdateAsync(existing, cancellationToken);

            var status = existing.IsActive ? "restored" : "archived";

            return Result<object>.Success(existing.Id, $"Miscellaneous Receipt {status} successfully");
        }
    }
}