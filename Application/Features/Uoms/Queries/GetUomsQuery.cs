using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Application.DTO.Uom;
using Application.Interfaces;
using Application.Results;
using MediatR;

namespace Application.Features.Uoms.Queries;

/// <summary>Query to retrieve a filtered, sorted, and paginated list of units of measure.</summary>
/// <param name="GenericFiltersDTO">Search and pagination filters.</param>
/// <param name="Sort">Sort direction and field.</param>
public record GetUomsQuery(GenericFiltersDto GenericFiltersDTO, Sort Sort) : IRequest<GetAllResult<List<GetUomDto>>>;

public class GetUomsQueryHandler(IUomRepository uomRepository)
    : IRequestHandler<GetUomsQuery, GetAllResult<List<GetUomDto>>>
{
    public async Task<GetAllResult<List<GetUomDto>>> Handle(GetUomsQuery request, CancellationToken cancellationToken)
    {
        var uoms = await uomRepository.GetAllAsync(request.GenericFiltersDTO, request.Sort, cancellationToken);

        var result = uoms.Select(x => new GetUomDto
        {
            Id = x.Id,
            IsActive = x.IsActive,
            Name = x.Name,
            ShortName = x.ShortName,
            IsInteger = x.IsInteger
        }).ToList();

        PaginationInfo? paginationInfo = null;

        if (request.GenericFiltersDTO.UsePagination)
            paginationInfo = new PaginationInfo
            {
                CurrentPage = request.GenericFiltersDTO.PageNumber,
                PageSize = request.GenericFiltersDTO.PageSize,
                TotalCount = await uomRepository.GetCountAsync(request.GenericFiltersDTO, cancellationToken)
            };

        var sortInfo = new SortInfo
        {
            SortColumns = ["id", "name", "shortname", "isinteger"],
            CurrentSort = request.Sort != null
                ? new CurrentSort
                {
                    Column = request.Sort.SortBy,
                    Direction = request.Sort.SortDirection
                }
                : null
        };

        return GetAllResult<List<GetUomDto>>.Success(result, paginationInfo: paginationInfo, sortInfo: sortInfo);
    }
}