using System.Net;
using Application.DTO.Inventory;
using Application.Interfaces;
using Application.Results;
using MediatR;

namespace Application.Features.Inventory.Queries;

public record GetInventoryReceivingByProductQuery(int WarehouseId, int ProductId)
    : IRequest<Result<GetInventoryReceivingDto>>;

public class GetInventoryReceivingByProductQueryHandler(IInventoryRepository inventoryRepository)
    : IRequestHandler<GetInventoryReceivingByProductQuery, Result<GetInventoryReceivingDto>>
{
    public async Task<Result<GetInventoryReceivingDto>> Handle(
        GetInventoryReceivingByProductQuery request,
        CancellationToken cancellationToken)
    {
        var inventory = await inventoryRepository.GetReceivingsByWarehouseAndProductIdAsync(
            request.WarehouseId,
            request.ProductId,
            cancellationToken);

        if (inventory == null)
            Result<GetInventoryReceivingDto>.Failure("Warehouse or product not found", HttpStatusCode.NotFound);

        return Result<GetInventoryReceivingDto>.Success(inventory);
    }
}