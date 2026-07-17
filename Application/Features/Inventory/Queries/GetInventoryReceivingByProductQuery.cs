using System.Net;
using Application.DTO.Inventory;
using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Application.Interfaces;
using Application.Results;
using MediatR;

namespace Application.Features.Inventory.Queries;

public record GetInventoryReceivingByProductQuery(int WarehouseId, int ProductId,
    GenericFiltersDto GenericFiltersDto, Sort Sort) : IRequest<GetAllResult<GetInventoryReceivingDto>>;

public class GetInventoryReceivingByProductQueryHandler(IInventoryRepository inventoryRepository)
    : IRequestHandler<GetInventoryReceivingByProductQuery, GetAllResult<GetInventoryReceivingDto>>
{
    public async Task<GetAllResult<GetInventoryReceivingDto>> Handle(
        GetInventoryReceivingByProductQuery request,
        CancellationToken cancellationToken)
    {
        var inventory = await inventoryRepository.GetReceivingsByWarehouseAndProductIdAsync(
            request.WarehouseId,
            request.ProductId,
            request.GenericFiltersDto,
            request.Sort,
            cancellationToken);

        if (inventory == null)
            return GetAllResult<GetInventoryReceivingDto>.Failure("Warehouse or product not found",
                HttpStatusCode.NotFound);

        PaginationInfo? paginationInfo = null;

        if (request.GenericFiltersDto.UsePagination)
            paginationInfo = new PaginationInfo
            {
                CurrentPage = request.GenericFiltersDto.PageNumber,
                PageSize = request.GenericFiltersDto.PageSize,
                TotalCount = await inventoryRepository.GetProductCountAsync(request.GenericFiltersDto,
                    request.ProductId, cancellationToken)
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
