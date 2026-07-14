using System.Net;
using Application.DTO.Inventory;
using Application.Interfaces;
using Application.Results;
using MediatR;

namespace Application.Features.Inventory.Queries;

public record GetInventoryReceivingsQuery(int WarehouseId) : IRequest<Result<GetInventoryReceivingDto>>;

public class GetInventoryReceivingsQueryHandler(IInventoryRepository inventoryRepository)
    : IRequestHandler<GetInventoryReceivingsQuery, Result<GetInventoryReceivingDto>>
{
    public async Task<Result<GetInventoryReceivingDto>> Handle(
        GetInventoryReceivingsQuery request,
        CancellationToken cancellationToken)
    {
        var inventory = await inventoryRepository.GetReceivingsByWarehouseIdAsync(
            request.WarehouseId,
            cancellationToken);

        if (inventory == null) Result<GetInventoryReceivingDto>.Failure("Warehouse not found", HttpStatusCode.NotFound);

        return Result<GetInventoryReceivingDto>.Success(inventory);
    }
}