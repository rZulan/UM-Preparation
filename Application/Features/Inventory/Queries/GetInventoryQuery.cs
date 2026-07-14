using System.Net;
using Application.DTO.Inventory;
using Application.Interfaces;
using Application.Results;
using MediatR;

namespace Application.Features.Inventory.Queries;

public record GetInventoryQuery(int WarehouseId) : IRequest<Result<GetInventoryDto>>;

public class GetInventoryQueryHandler(IInventoryRepository inventoryRepository)
    : IRequestHandler<GetInventoryQuery, Result<GetInventoryDto>>
{
    private readonly IInventoryRepository _inventoryRepository = inventoryRepository;

    public async Task<Result<GetInventoryDto>> Handle(
        GetInventoryQuery request,
        CancellationToken cancellationToken)
    {
        var inventory = await _inventoryRepository.GetByWarehouseIdAsync(request.WarehouseId, cancellationToken);

        if (inventory == null) Result<GetInventoryDto>.Failure("Warehouse not found", HttpStatusCode.NotFound);

        return Result<GetInventoryDto>.Success(inventory);
    }
}