using Application.DTO.MiscellaneousReceipt;
using Application.Interfaces;
using Application.Results;
using Domain.Entities;
using MediatR;
using System.Net;

namespace Application.Features.MiscellaneousReceipts.Commands
{
    /// <summary>Command to create a new miscellaneous receipt.</summary>
    /// <param name="UserId">The ID of the authenticated user performing the action.</param>
    /// <param name="AddMiscellaneousReceiptDTO">The miscellaneous receipt data to be created.</param>
    public record AddMiscellaneousReceiptCommand(int? UserId, AddMiscellaneousReceiptDTO AddMiscellaneousReceiptDTO) : IRequest<Result<object>>;
    public class AddMiscellaneousReceiptCommandHandler(IMiscellaneousReceiptRepository miscellaneousReceiptRepository, IUserRepository userRepository) : IRequestHandler<AddMiscellaneousReceiptCommand, Result<object>>
    {
        private readonly IMiscellaneousReceiptRepository _miscellaneousReceiptRepository = miscellaneousReceiptRepository;
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<Result<object>> Handle(AddMiscellaneousReceiptCommand request, CancellationToken cancellationToken)
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

            var miscellaneousReceipt = new MiscellaneousReceipt
            {
                ProductId = request.AddMiscellaneousReceiptDTO.ProductId,
                Quantity = request.AddMiscellaneousReceiptDTO.QuantityOut,
                CreatedAt = DateTime.UtcNow,
                CreatedById = existingUser.Id
            };

            await _miscellaneousReceiptRepository.AddAsync(miscellaneousReceipt, cancellationToken);

            return Result<object>.Success(miscellaneousReceipt.Id, "Miscellaneous Receipt created successfully", HttpStatusCode.Created);
        }
    }
}
