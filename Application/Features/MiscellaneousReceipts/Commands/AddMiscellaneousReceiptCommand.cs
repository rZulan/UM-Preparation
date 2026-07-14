using System.Net;
using Application.DTO.MiscellaneousReceipt;
using Application.Interfaces;
using Application.Results;
using Domain.Entities;
using MediatR;

namespace Application.Features.MiscellaneousReceipts.Commands;

/// <summary>Command to create a new miscellaneous receipt.</summary>
/// <param name="UserId">The ID of the authenticated user performing the action.</param>
/// <param name="AddMiscellaneousReceiptDTO">The miscellaneous receipt data to be created.</param>
public record AddMiscellaneousReceiptCommand(int? UserId, AddMiscellaneousReceiptDto AddMiscellaneousReceiptDTO)
    : IRequest<Result<object>>;

public class AddMiscellaneousReceiptCommandHandler(
    IMiscellaneousReceiptRepository miscellaneousReceiptRepository,
    IUserRepository userRepository,
    IWarehouseRepository warehouseRepository,
    IProductRepository productRepository,
    IWarehouseReceivingRepository warehouseReceivingRepository)
    : IRequestHandler<AddMiscellaneousReceiptCommand, Result<object>>
{
    public async Task<Result<object>> Handle(AddMiscellaneousReceiptCommand request,
        CancellationToken cancellationToken)
    {
        if (request.UserId == null) return Result<object>.Failure("User is not signed in", HttpStatusCode.Unauthorized);

        var existingUser = await userRepository.GetByIdAsync(request.UserId.Value, cancellationToken);

        if (existingUser == null) return Result<object>.Failure("User not found", HttpStatusCode.NotFound);

        var existingWarehouse =
            await warehouseRepository.GetByIdAsync(request.AddMiscellaneousReceiptDTO.WarehouseId, cancellationToken);

        if (existingWarehouse == null) return Result<object>.Failure("Warehouse not found", HttpStatusCode.NotFound);

        var existingProduct =
            await productRepository.GetByIdAsync(request.AddMiscellaneousReceiptDTO.ProductId, cancellationToken);

        if (existingProduct == null) return Result<object>.Failure("Product not found", HttpStatusCode.NotFound);

        var miscellaneousReceipt = new MiscellaneousReceipt
        {
            WarehouseId = existingWarehouse.Id,
            ProductId = existingProduct.Id,
            Quantity = request.AddMiscellaneousReceiptDTO.Quantity,
            Reason = request.AddMiscellaneousReceiptDTO.Reason,
            CreatedAt = DateTime.UtcNow,
            CreatedById = existingUser.Id
        };

        await miscellaneousReceiptRepository.AddAsync(miscellaneousReceipt, cancellationToken);

        var warehouseReceiving = new WarehouseReceiving
        {
            Quantity = request.AddMiscellaneousReceiptDTO.Quantity,
            WarehouseId = existingWarehouse.Id,
            ProductId = existingProduct.Id,
            MiscellaneousReceiptId = miscellaneousReceipt.Id,
            CreatedAt = DateTime.UtcNow,
            CreatedById = existingUser.Id
        };

        await warehouseReceivingRepository.AddAsync(warehouseReceiving, cancellationToken);

        return Result<object>.Success(miscellaneousReceipt.Id, "Miscellaneous Receipt created successfully",
            HttpStatusCode.Created);
    }
}