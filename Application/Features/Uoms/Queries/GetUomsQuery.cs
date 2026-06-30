using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Application.DTO.Uom;
using Application.Interfaces;
using Application.Results;
using MediatR;

namespace Application.Features.Uoms.Queries
{
    /// <summary>Query to retrieve a filtered, sorted, and paginated list of units of measure.</summary>
    /// <param name="GenericFiltersDTO">Search and pagination filters.</param>
    /// <param name="Sort">Sort direction and field.</param>
    public record GetUomsQuery(GenericFiltersDTO GenericFiltersDTO, Sort Sort) : IRequest<GetAllResult<List<GetUomDTO>>>;
    public class GetUomsQueryHandler(IUomRepository uomRepository) : IRequestHandler<GetUomsQuery, GetAllResult<List<GetUomDTO>>>
    {
        private readonly IUomRepository _uomRepository = uomRepository;

        public async Task<GetAllResult<List<GetUomDTO>>> Handle(GetUomsQuery request, CancellationToken cancellationToken)
        {
            var uoms = await _uomRepository.GetAllAsync(request.GenericFiltersDTO, request.Sort, cancellationToken);

            var result = uoms.Select(x => new GetUomDTO
            {
                Id = x.Id,
                IsActive = x.IsActive,
                Name = x.Name,
                ShortName = x.ShortName,
                IsInteger = x.IsInteger,
            }).ToList();

            PaginationInfo? paginationInfo = null;

            if (request.GenericFiltersDTO.UsePagination == true)
            {
                paginationInfo = new PaginationInfo
                {
                    CurrentPage = request.GenericFiltersDTO.PageNumber,
                    PageSize = request.GenericFiltersDTO.PageSize,
                    TotalCount = await _uomRepository.GetCountAsync(request.GenericFiltersDTO, cancellationToken)
                };
            }

            var sortInfo = new SortInfo
            {
                SortColumns = ["id", "name"],
                CurrentSort = request.Sort != null ? new CurrentSort
                {
                    Column = request.Sort.SortBy,
                    Direction = request.Sort.SortDirection
                } : null
            };

            return GetAllResult<List<GetUomDTO>>.Success(result, paginationInfo: paginationInfo, sortInfo: sortInfo);
        }
    }
}
