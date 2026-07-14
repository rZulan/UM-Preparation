using System.Net;
using Application.DTO.WarehouseReceiving;
using Application.Interfaces;
using Application.Results;
using MediatR;

namespace Application.Features.WarehouseReceivings.Commands;

/// <summary>Command to update an existing warehouse entry.</summary>
/// <param name="UserId">The ID of the authenticated user performing the action.</param>
/// <param name="Id">The ID of the warehouse entry to update.</param>
/// <param name="UpdateWarehouseReceivingDTO">The updated warehouse data.</param>
public record UpdateWarehouseReceivingCommand(
    int? UserId,
    int Id,
    UpdateWarehouseReceivingDto UpdateWarehouseReceivingDTO) : IRequest<Result<object>>;

public class UpdateWarehouseReceivingCommandHandler(
    IWarehouseReceivingRepository warehouseReceivingRepository,
    IUserRepository userRepository,
    IProductRepository productRepository) : IRequestHandler<UpdateWarehouseReceivingCommand, Result<object>>
{
    public async Task<Result<object>> Handle(UpdateWarehouseReceivingCommand request,
        CancellationToken cancellationToken)
    {
        if (request.UserId == null) return Result<object>.Failure("User is not signed in", HttpStatusCode.Unauthorized);

        var existingUser = await userRepository.GetByIdAsync(request.UserId.Value, cancellationToken);

        if (existingUser == null) return Result<object>.Failure("User not found", HttpStatusCode.NotFound);

        var existingWarehouseReceiving =
            await warehouseReceivingRepository.GetByIdAsync(request.Id, cancellationToken);

        if (existingWarehouseReceiving == null)
            return Result<object>.Failure("Warehouse Receiving not found", HttpStatusCode.NotFound);

        if (request.UpdateWarehouseReceivingDTO.ProductId.HasValue)
        {
            var existingProduct =
                await productRepository.GetByIdAsync(request.UpdateWarehouseReceivingDTO.ProductId.Value,
                    cancellationToken);

            if (existingProduct == null) return Result<object>.Failure("Product not found", HttpStatusCode.NotFound);

            existingWarehouseReceiving.ProductId = request.UpdateWarehouseReceivingDTO.ProductId.Value;
        }

        if (request.UpdateWarehouseReceivingDTO.Quantity.HasValue)
            existingWarehouseReceiving.Quantity = request.UpdateWarehouseReceivingDTO.Quantity.Value;

        if (request.UpdateWarehouseReceivingDTO.MiscellaneousReceiptId.HasValue)
            existingWarehouseReceiving.MiscellaneousReceiptId =
                request.UpdateWarehouseReceivingDTO.MiscellaneousReceiptId.Value;

        existingWarehouseReceiving.UpdatedAt = DateTime.UtcNow;
        existingWarehouseReceiving.UpdatedById = request.UserId.Value;

        await warehouseReceivingRepository.UpdateAsync(existingWarehouseReceiving, cancellationToken);

        return Result<object>.Success(null);
    }
}