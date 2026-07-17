using System.Net;
using Application.DTO.Inventory;
using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Application.Interfaces;
using Application.Results;
using MediatR;

namespace Application.Features.Inventory.Queries;

public record GetInventoryReceivingsQuery(int WarehouseId, GenericFiltersDto GenericFiltersDto, Sort Sort)
    : IRequest<GetAllResult<GetInventoryReceivingDto>>;

public class GetInventoryReceivingsQueryHandler(IInventoryRepository inventoryRepository)
    : IRequestHandler<GetInventoryReceivingsQuery, GetAllResult<GetInventoryReceivingDto>>
{
    public async Task<GetAllResult<GetInventoryReceivingDto>> Handle(
        GetInventoryReceivingsQuery request,
        CancellationToken cancellationToken)
    {
        var inventory = await inventoryRepository.GetReceivingsByWarehouseIdAsync(
            request.WarehouseId,
            request.GenericFiltersDto,
            request.Sort,
            cancellationToken);

        if (inventory == null)
            return GetAllResult<GetInventoryReceivingDto>.Failure("Warehouse not found", HttpStatusCode.NotFound);

        PaginationInfo? paginationInfo = null;

        if (request.GenericFiltersDto.UsePagination)
            paginationInfo = new PaginationInfo
            {
                CurrentPage = request.GenericFiltersDto.PageNumber,
                PageSize = request.GenericFiltersDto.PageSize,
                TotalCount = await inventoryRepository.GetProductCountAsync(request.GenericFiltersDto, null,
                    cancellationToken)
            };

        var sortInfo = new SortInfo
        {
            SortColumns = ["productid", "itemcode", "description"],
            CurrentSort = request.Sort != null
                ? new CurrentSort
                {
                    Column = request.Sort.SortBy,
                    Direction = request.Sort.SortDirection
                }
                : null
        };

        return GetAllResult<GetInventoryReceivingDto>.Success(inventory, paginationInfo: paginationInfo,
            sortInfo: sortInfo);
    }
}
