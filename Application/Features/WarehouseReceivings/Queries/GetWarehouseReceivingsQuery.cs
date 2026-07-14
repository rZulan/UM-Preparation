using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Application.DTO.WarehouseReceiving;
using Application.Interfaces;
using Application.Results;
using MediatR;

namespace Application.Features.WarehouseReceivings.Queries;

/// <summary>Query to retrieve a filtered, sorted, and paginated list of warehouse entries.</summary>
/// <param name="GenericFiltersDTO">Search and pagination filters.</param>
/// <param name="Sort">Sort direction and field.</param>
public record GetWarehouseReceivingsQuery(GenericFiltersDto GenericFiltersDTO, Sort Sort)
    : IRequest<GetAllResult<List<GetWarehouseReceivingDto>>>;

public class GetWarehouseReceivingsQueryHandler(IWarehouseReceivingRepository warehouseReceivingRepository)
    : IRequestHandler<GetWarehouseReceivingsQuery, GetAllResult<List<GetWarehouseReceivingDto>>>
{
    public async Task<GetAllResult<List<GetWarehouseReceivingDto>>> Handle(GetWarehouseReceivingsQuery request,
        CancellationToken cancellationToken)
    {
        var warehouseReceivings =
            await warehouseReceivingRepository.GetAllAsync(request.GenericFiltersDTO, request.Sort, cancellationToken);

        var result = warehouseReceivings.Select(x => new GetWarehouseReceivingDto
        {
            Id = x.Id,
            WarehouseId = x.WarehouseId,
            Warehouse = x.Warehouse.Name,
            Quantity = x.Quantity,
            ProductId = x.ProductId,
            ProductCode = x.Product.ItemCode,
            ProductDescription = x.Product.Description,
            IsInteger = x.Product.Uom.IsInteger
        }).ToList();

        PaginationInfo? paginationInfo = null;

        if (request.GenericFiltersDTO.UsePagination)
            paginationInfo = new PaginationInfo
            {
                CurrentPage = request.GenericFiltersDTO.PageNumber,
                PageSize = request.GenericFiltersDTO.PageSize,
                TotalCount =
                    await warehouseReceivingRepository.GetCountAsync(request.GenericFiltersDTO, cancellationToken)
            };

        var sortInfo = new SortInfo
        {
            SortColumns =
                ["id", "warehouseid", "warehouse", "quantity", "productid", "productcode", "productdescription"],
            CurrentSort = request.Sort != null
                ? new CurrentSort
                {
                    Column = request.Sort.SortBy,
                    Direction = request.Sort.SortDirection
                }
                : null
        };

        return GetAllResult<List<GetWarehouseReceivingDto>>.Success(result, paginationInfo: paginationInfo,
            sortInfo: sortInfo);
    }
}