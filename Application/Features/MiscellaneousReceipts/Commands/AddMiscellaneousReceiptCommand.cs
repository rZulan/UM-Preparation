using System.Net;
using Application.DTO.MiscellaneousReceipt;
using Application.Interfaces;
using Application.Results;
using Domain.Entities;
using Domain.Entities.Junction;
using MediatR;

namespace Application.Features.MiscellaneousReceipts.Commands;

/// <summary>Command to create a new miscellaneous receipt.</summary>
/// <param name="UserId">The ID of the authenticated user performing the action.</param>
/// <param name="AddMiscellaneousReceiptDto">The miscellaneous receipt data to be created.</param>
public record AddMiscellaneousReceiptCommand(int? UserId, AddMiscellaneousReceiptDto AddMiscellaneousReceiptDto)
    : IRequest<Result<object>>;

public class AddMiscellaneousReceiptCommandHandler(
    IMiscellaneousReceiptRepository miscellaneousReceiptRepository,
    IUserRepository userRepository,
    IWarehouseRepository warehouseRepository,
    IProductRepository productRepository)
    : IRequestHandler<AddMiscellaneousReceiptCommand, Result<object>>
{
    public async Task<Result<object>> Handle(AddMiscellaneousReceiptCommand request,
        CancellationToken cancellationToken)
    {
        if (request.UserId == null) return Result<object>.Failure("User is not signed in", HttpStatusCode.Unauthorized);

        var existingUser = await userRepository.GetByIdAsync(request.UserId.Value, cancellationToken);

        if (existingUser == null) return Result<object>.Failure("User not found", HttpStatusCode.NotFound);

        var existingWarehouse =
            await warehouseRepository.GetByIdAsync(request.AddMiscellaneousReceiptDto.WarehouseId, cancellationToken);

        if (existingWarehouse == null) return Result<object>.Failure("Warehouse not found", HttpStatusCode.NotFound);

        var receiptProducts = request.AddMiscellaneousReceiptDto.AddMiscellaneousReceiptProducts;

        if (receiptProducts.Count == 0)
            return Result<object>.Failure("At least one product is required");

        if (receiptProducts.Any(product => product.Quantity <= 0))
            return Result<object>.Failure("Product quantity must be greater than zero");

        if (receiptProducts.Select(product => product.ProductId).Distinct().Count() != receiptProducts.Count)
            return Result<object>.Failure("Duplicate product found");

        var productIds = receiptProducts.Select(product => product.ProductId).ToList();
        var existingProducts = await productRepository.AllExistsAsync(productIds, cancellationToken);

        if (!existingProducts) return Result<object>.Failure("One or more products do not exist");

        var createdAt = DateTime.UtcNow;
        var miscellaneousReceipt = new MiscellaneousReceipt
        {
            WarehouseId = existingWarehouse.Id,
            Reason = request.AddMiscellaneousReceiptDto.Reason,
            CreatedAt = createdAt,
            CreatedById = existingUser.Id
        };

        foreach (var product in receiptProducts)
        {
            miscellaneousReceipt.MiscellaneousReceiptProducts.Add(new MiscellaneousReceiptProducts
            {
                MiscellaneousReceiptId = 0,
                ProductId = product.ProductId,
                Quantity = product.Quantity,
                MiscellaneousReceipt = miscellaneousReceipt
            });

            miscellaneousReceipt.WarehouseReceivings.Add(new WarehouseReceiving
            {
                Quantity = product.Quantity,
                WarehouseId = existingWarehouse.Id,
                ProductId = product.ProductId,
                MiscellaneousReceipt = miscellaneousReceipt,
                CreatedAt = createdAt,
                CreatedById = existingUser.Id
            });
        }

        await miscellaneousReceiptRepository.AddAsync(miscellaneousReceipt, cancellationToken);

        return Result<object>.Success(miscellaneousReceipt.Id, "Miscellaneous Receipt created successfully",
            HttpStatusCode.Created);
    }
}
