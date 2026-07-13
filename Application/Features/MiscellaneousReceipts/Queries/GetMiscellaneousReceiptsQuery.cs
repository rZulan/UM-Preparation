using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Application.DTO.MiscellaneousReceipt;
using Application.Interfaces;
using Application.Results;
using MediatR;

namespace Application.Features.MiscellaneousReceipts.Queries
{
    /// <summary>Query to retrieve a filtered, sorted, and paginated list of miscellaneous receipts.</summary>
    /// <param name="GenericFiltersDTO">Search and pagination filters.</param>
    /// <param name="Sort">Sort direction and field.</param>
    public record GetMiscellaneousReceiptsQuery(GenericFiltersDto GenericFiltersDTO, Sort Sort) : IRequest<GetAllResult<List<GetMiscellaneousReceiptDto>>>;
    public class GetMiscellaneousReceiptsQueryHandler(IMiscellaneousReceiptRepository miscellanousReceiptRepository) : IRequestHandler<GetMiscellaneousReceiptsQuery, GetAllResult<List<GetMiscellaneousReceiptDto>>>
    {
        private readonly IMiscellaneousReceiptRepository _miscellaneousReceiptRepository = miscellanousReceiptRepository;

        public async Task<GetAllResult<List<GetMiscellaneousReceiptDto>>> Handle(GetMiscellaneousReceiptsQuery request, CancellationToken cancellationToken)
        {
            var miscellaneousReceipts = await _miscellaneousReceiptRepository.GetAllAsync(request.GenericFiltersDTO, request.Sort, cancellationToken);

            var result = miscellaneousReceipts.Select(d => new GetMiscellaneousReceiptDto
            {
                Id = d.Id,
                IsActive = d.IsActive,
                WarehouseId = d.WarehouseId,
                Warehouse = d.Warehouse.Name,
                ProductId = d.ProductId,
                ItemCode = d.Product.ItemCode,
                Description = d.Product.Description,
                Uom = d.Product.Uom.ShortName,
                Quantity = d.Quantity,
                Reason = d.Reason,
            }).ToList();

            PaginationInfo? paginationInfo = null;

            if (request.GenericFiltersDTO.UsePagination == true)
            {
                paginationInfo = new PaginationInfo
                {
                    CurrentPage = request.GenericFiltersDTO.PageNumber,
                    PageSize = request.GenericFiltersDTO.PageSize,
                    TotalCount = await _miscellaneousReceiptRepository.GetCountAsync(request.GenericFiltersDTO, cancellationToken)
                };
            }

            var sortInfo = new SortInfo
            {
                SortColumns = ["id", "itemcode", "description", "uom", "quantity"],
                CurrentSort = request.Sort != null ? new CurrentSort
                {
                    Column = request.Sort.SortBy,
                    Direction = request.Sort.SortDirection
                } : null
            };

            return GetAllResult<List<GetMiscellaneousReceiptDto>>.Success(result, paginationInfo: paginationInfo, sortInfo: sortInfo);
        }
    }
}