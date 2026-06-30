using Application.DTO.Dispatch;
using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Application.Interfaces;
using Application.Results;
using MediatR;

namespace Application.Features.Dispatches.Queries
{
    /// <summary>Query to retrieve a filtered, sorted, and paginated list of dispatches.</summary>
    /// <param name="GenericFiltersDTO">Search and pagination filters.</param>
    /// <param name="Sort">Sort direction and field.</param>
    public record GetDispatchesQuery(GenericFiltersDTO GenericFiltersDTO, Sort Sort) : IRequest<GetAllResult<List<GetDispatchDTO>>>;
    public class GetDispatchesQueryHandler(IDispatchRepository dispatchRepository) : IRequestHandler<GetDispatchesQuery, GetAllResult<List<GetDispatchDTO>>>
    {
        private readonly IDispatchRepository _dispatchRepository = dispatchRepository;

        public async Task<GetAllResult<List<GetDispatchDTO>>> Handle(GetDispatchesQuery request, CancellationToken cancellationToken)
        {
            var dispatches = await _dispatchRepository.GetAllAsync(request.GenericFiltersDTO, request.Sort, cancellationToken);

            var result = dispatches.Select(d => new GetDispatchDTO
            {
                Id = d.Id,
                IsActive = d.IsActive,
                BatchNumber = d.BatchNumber,
                ItemCode = d.Product.ItemCode,
                Description = d.Product.Description,
                Uom = d.Product.Uom.ShortName,
                QuantityOut = d.QuantityOut,
                QuantityReturn = d.QuantityReturn,
                HarvestDate = d.HarvestDate
            }).ToList();

            PaginationInfo? paginationInfo = null;

            if (request.GenericFiltersDTO.UsePagination == true)
            {
                paginationInfo = new PaginationInfo
                {
                    CurrentPage = request.GenericFiltersDTO.PageNumber,
                    PageSize = request.GenericFiltersDTO.PageSize,
                    TotalCount = await _dispatchRepository.GetCountAsync(request.GenericFiltersDTO, cancellationToken)
                };
            }

            var sortInfo = new SortInfo
            {
                SortColumns = ["id", "batchNumber"],
                CurrentSort = request.Sort != null ? new CurrentSort
                {
                    Column = request.Sort.SortBy,
                    Direction = request.Sort.SortDirection
                } : null
            };

            return GetAllResult<List<GetDispatchDTO>>.Success(result, paginationInfo: paginationInfo, sortInfo: sortInfo);
        }
    }
}