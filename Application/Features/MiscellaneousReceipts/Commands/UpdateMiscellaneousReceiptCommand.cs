using Application.DTO.MiscellaneousReceipt;
using Application.Interfaces;
using Application.Results;
using MediatR;
using System.Net;

namespace Application.Features.MiscellaneousReceipts.Commands
{
    /// <summary>Command to update an existing miscellaneous receipt.</summary>
    /// <param name="UserId">The ID of the authenticated user performing the action.</param>
    /// <param name="Id">The ID of the miscellaneous receipt to update.</param>
    /// <param name="UpdateMiscellaneousReceiptDTO">The updated miscellaneous receipt data.</param>
    public record UpdateMiscellaneousReceiptCommand(int? UserId, int Id, UpdateMiscellaneousReceiptDTO UpdateMiscellaneousReceiptDTO) : IRequest<Result<object>>;
    public class UpdateMiscellaneousReceiptCommandHandler(IMiscellaneousReceiptRepository miscellaneousReceiptRepository, IUserRepository userRepository) : IRequestHandler<UpdateMiscellaneousReceiptCommand, Result<object>>
    {
        private readonly IMiscellaneousReceiptRepository _miscellaneousReceiptRepository = miscellaneousReceiptRepository;
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<Result<object>> Handle(UpdateMiscellaneousReceiptCommand request, CancellationToken cancellationToken)
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

            var existing = await _miscellaneousReceiptRepository.GetByIdAsync(request.Id, cancellationToken);

            if (existing == null)
            {
                return Result<object>.Failure("Miscellaneous Receipt not found", System.Net.HttpStatusCode.NotFound);
            }

            if (request.UpdateMiscellaneousReceiptDTO.ProductId.HasValue)
            {
                existing.ProductId = request.UpdateMiscellaneousReceiptDTO.ProductId.Value;
            }

            if (request.UpdateMiscellaneousReceiptDTO.Quantity.HasValue)
            {
                existing.Quantity = request.UpdateMiscellaneousReceiptDTO.Quantity.Value;
            }

            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedById = existingUser.Id;

            await _miscellaneousReceiptRepository.UpdateAsync(existing, cancellationToken);

            return Result<object>.Success(existing.Id, "Miscellaneous Receipt updated successfully");
        }
    }
}