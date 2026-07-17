using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Application.DTO.MiscellaneousReceipt;
using Application.Interfaces;
using Application.Results;
using MediatR;

namespace Application.Features.MiscellaneousReceipts.Queries;

/// <summary>Query to retrieve a filtered, sorted, and paginated list of miscellaneous receipts.</summary>
/// <param name="GenericFiltersDto">Search and pagination filters.</param>
/// <param name="Sort">Sort direction and field.</param>
public record GetMiscellaneousReceiptsQuery(GenericFiltersDto GenericFiltersDto, Sort Sort)
    : IRequest<GetAllResult<List<GetMiscellaneousReceiptDto>>>;

public class GetMiscellaneousReceiptsQueryHandler(IMiscellaneousReceiptRepository miscellaneousReceiptRepository)
    : IRequestHandler<GetMiscellaneousReceiptsQuery, GetAllResult<List<GetMiscellaneousReceiptDto>>>
{
    public async Task<GetAllResult<List<GetMiscellaneousReceiptDto>>> Handle(GetMiscellaneousReceiptsQuery request,
        CancellationToken cancellationToken)
    {
        var miscellaneousReceipts =
            await miscellaneousReceiptRepository.GetAllAsync(request.GenericFiltersDto, request.Sort,
                cancellationToken);

        var result = miscellaneousReceipts.Select(d => new GetMiscellaneousReceiptDto
        {
            Id = d.Id,
            IsActive = d.IsActive,
            WarehouseId = d.WarehouseId,
            Warehouse = d.Warehouse.Name,
            Reason = d.Reason,
            MiscellaneousReceiptProducts = d.MiscellaneousReceiptProducts
                .OrderBy(product => product.ProductId)
                .Select(product => new GetMiscellaneousReceiptProductDto
                {
                    ProductId = product.ProductId,
                    ItemCode = product.Product.ItemCode,
                    Description = product.Product.Description,
                    Uom = product.Product.Uom.ShortName,
                    Quantity = product.Quantity
                }).ToList()
        }).ToList();

        PaginationInfo? paginationInfo = null;

        if (request.GenericFiltersDto.UsePagination)
            paginationInfo = new PaginationInfo
            {
                CurrentPage = request.GenericFiltersDto.PageNumber,
                PageSize = request.GenericFiltersDto.PageSize,
                TotalCount =
                    await miscellaneousReceiptRepository.GetCountAsync(request.GenericFiltersDto, cancellationToken)
            };

        var sortInfo = new SortInfo
        {
            SortColumns = ["id", "itemcode", "description", "uom", "quantity"],
            CurrentSort = request.Sort != null
                ? new CurrentSort
                {
                    Column = request.Sort.SortBy,
                    Direction = request.Sort.SortDirection
                }
                : null
        };

        return GetAllResult<List<GetMiscellaneousReceiptDto>>.Success(result, paginationInfo: paginationInfo,
            sortInfo: sortInfo);
    }
}