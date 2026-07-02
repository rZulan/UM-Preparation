using Application.DTO.WarehouseReceiving;
using Application.Interfaces;
using Application.Results;
using MediatR;
using System.Net;

namespace Application.Features.WarehouseReceivings.Commands
{
    /// <summary>Command to update an existing warehouse entry.</summary>
    /// <param name="UserId">The ID of the authenticated user performing the action.</param>
    /// <param name="Id">The ID of the warehouse entry to update.</param>
    /// <param name="UpdateWarehouseReceivingDTO">The updated warehouse data.</param>
    public record UpdateWarehouseReceivingCommand(int? UserId, int Id, UpdateWarehouseReceivingDTO UpdateWarehouseReceivingDTO) : IRequest<Result<object>>;
    public class UpdateWarehouseReceivingCommandHandler(IWarehouseReceivingRepository warehouseReceivingRepository, IUserRepository userRepository, IProductRepository productRepository) : IRequestHandler<UpdateWarehouseReceivingCommand, Result<object>>
    {
        private readonly IWarehouseReceivingRepository _warehouseReceivingRepository = warehouseReceivingRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IProductRepository _productRepository = productRepository;

        public async Task<Result<object>> Handle(UpdateWarehouseReceivingCommand request, CancellationToken cancellationToken)
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

            var existingWarehouse = await _warehouseReceivingRepository.GetByIdAsync(request.Id, cancellationToken);

            if (existingWarehouse == null)
            {
                return Result<object>.Failure("Warehouse not found", HttpStatusCode.NotFound);
            }

            if (request.UpdateWarehouseReceivingDTO.ProductId.HasValue)
            {
                var existingProduct = await _productRepository.GetByIdAsync(request.UpdateWarehouseReceivingDTO.ProductId.Value, cancellationToken);

                if (existingProduct == null)
                {
                    return Result<object>.Failure("Product not found", HttpStatusCode.NotFound);
                }

                existingWarehouse.ProductId = request.UpdateWarehouseReceivingDTO.ProductId.Value;
            }

            if (request.UpdateWarehouseReceivingDTO.Quantity.HasValue)
            {
                existingWarehouse.Quantity = request.UpdateWarehouseReceivingDTO.Quantity.Value;
            }

            if (request.UpdateWarehouseReceivingDTO.MiscellaneousReceiptId.HasValue)
            {
                existingWarehouse.MiscellaneousReceiptId = request.UpdateWarehouseReceivingDTO.MiscellaneousReceiptId.Value;
            }

            existingWarehouse.UpdatedAt = DateTime.UtcNow;
            existingWarehouse.UpdatedById = request.UserId.Value;

            await _warehouseReceivingRepository.UpdateAsync(existingWarehouse, cancellationToken);

            return Result<object>.Success(null);
        }
    }
}
