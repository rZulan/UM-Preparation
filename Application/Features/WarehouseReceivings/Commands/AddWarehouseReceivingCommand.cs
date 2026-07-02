using Application.DTO.WarehouseReceiving;
using Application.Interfaces;
using Application.Results;
using Domain.Entities;
using MediatR;
using System.Net;

namespace Application.Features.WarehouseReceivings.Commands
{
    /// <summary>Command to create a new warehouse entry.</summary>
    /// <param name="UserId">The ID of the authenticated user performing the action.</param>
    /// <param name="AddWarehouseReceivingDTO">The warehouse data to be created.</param>
    public record AddWarehouseReceivingCommand(int? UserId, AddWarehouseReceivingDTO AddWarehouseReceivingDTO) : IRequest<Result<object>>;
    public class AddWarehouseReceivingCommandHandler(IWarehouseReceivingRepository warehouseReceivingRepository, IUserRepository userRepository, IProductRepository productRepository, IMiscellaneousReceiptRepository miscellaneousReceiptRepository) : IRequestHandler<AddWarehouseReceivingCommand, Result<object>>
    {
        private readonly IWarehouseReceivingRepository _warehouseReceivingRepository = warehouseReceivingRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IProductRepository _productRepository = productRepository;
        private readonly IMiscellaneousReceiptRepository _miscellaneousReceiptRepository = miscellaneousReceiptRepository;

        public async Task<Result<object>> Handle(AddWarehouseReceivingCommand request, CancellationToken cancellationToken)
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

            var existingProduct = await _productRepository.GetByIdAsync(request.AddWarehouseReceivingDTO.ProductId, cancellationToken);

            if (existingProduct == null)
            {
                return Result<object>.Failure("Product not found", HttpStatusCode.NotFound);
            }

            if (request.AddWarehouseReceivingDTO.MiscellaneousReceiptId != null)
            {
                var existingMiscellaneousReceipt = await _miscellaneousReceiptRepository.GetByIdAsync(request.AddWarehouseReceivingDTO.MiscellaneousReceiptId.Value, cancellationToken);

                if (existingMiscellaneousReceipt == null)
                {
                    return Result<object>.Failure("Miscellaneous Receipt not found", HttpStatusCode.NotFound);
                }
            }

            var warehouseReceiving = new WarehouseReceiving
            {
                WarehouseId = request.AddWarehouseReceivingDTO.WarehouseId,
                Quantity = request.AddWarehouseReceivingDTO.Quantity,
                ProductId = request.AddWarehouseReceivingDTO.ProductId,
                MiscellaneousReceiptId = request.AddWarehouseReceivingDTO.MiscellaneousReceiptId,
                CreatedAt = DateTime.UtcNow,
                CreatedById = request.UserId.Value
            };

            await _warehouseReceivingRepository.AddAsync(warehouseReceiving, cancellationToken);

            return Result<object>.Success(null);
        }
    }
}
