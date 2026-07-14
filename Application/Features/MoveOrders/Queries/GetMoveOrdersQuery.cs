using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Application.DTO.MoveOrder;
using Application.Interfaces;
using Application.Results;
using MediatR;

namespace Application.Features.MoveOrders.Queries;

public record GetMoveOrdersQuery(GenericFiltersDto GenericFiltersDTO, Sort Sort)
    : IRequest<GetAllResult<List<GetMoveOrderDto>>>;

public class GetMoveOrdersQueryHandler(IMoveOrderRepository moveOrderRepository)
    : IRequestHandler<GetMoveOrdersQuery, GetAllResult<List<GetMoveOrderDto>>>
{
    private readonly IMoveOrderRepository _moveOrderRepository = moveOrderRepository;

    public async Task<GetAllResult<List<GetMoveOrderDto>>> Handle(GetMoveOrdersQuery request,
        CancellationToken cancellationToken)
    {
        var moveOrders =
            await _moveOrderRepository.GetAllAsync(request.GenericFiltersDTO, request.Sort, cancellationToken);

        var result = moveOrders.Select(moveOrder => new GetMoveOrderDto
        {
            Id = moveOrder.Id,
            IsActive = moveOrder.IsActive,
            IsTransacted = moveOrder.IsTransacted,
            CreatedAt = moveOrder.CreatedAt,
            WarehouseId = moveOrder.WarehouseId,
            Warehouse = moveOrder.Warehouse.Name,
            MoveOrderProducts = moveOrder.MoveOrderProducts.Select(product => new GetMoveOrderProductDTO
            {
                ProductId = product.ProductId,
                ItemCode = product.Product.ItemCode,
                Description = product.Product.Description,
                TotalQuantity = product.TotalQuantity
            }).ToList()
        }).ToList();

        PaginationInfo? paginationInfo = null;

        if (request.GenericFiltersDTO.UsePagination)
            paginationInfo = new PaginationInfo
            {
                CurrentPage = request.GenericFiltersDTO.PageNumber,
                PageSize = request.GenericFiltersDTO.PageSize,
                TotalCount = await _moveOrderRepository.GetCountAsync(request.GenericFiltersDTO, cancellationToken)
            };

        var sortInfo = new SortInfo
        {
            SortColumns = ["id"],
            CurrentSort = request.Sort != null
                ? new CurrentSort
                {
                    Column = request.Sort.SortBy,
                    Direction = request.Sort.SortDirection
                }
                : null
        };

        return GetAllResult<List<GetMoveOrderDto>>.Success(result, paginationInfo: paginationInfo, sortInfo: sortInfo);
    }
}