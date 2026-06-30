using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Application.DTO.Permission;
using Application.Interfaces;
using Application.Results;
using MediatR;

namespace Application.Features.Permissions.Queries
{
    /// <summary>Query to retrieve a filtered, sorted, and paginated list of permissions.</summary>
    /// <param name="GenericFiltersDTO">Search and pagination filters.</param>
    /// <param name="Sort">Sort direction and field.</param>
    public record GetPermissionsQuery(GenericFiltersDTO GenericFiltersDTO, Sort Sort) : IRequest<GetAllResult<List<GetPermissionDTO>>>;
    public class GetPermissionsQueryHandler(IPermissionRepository permissionRepository) : IRequestHandler<GetPermissionsQuery, GetAllResult<List<GetPermissionDTO>>>
    {
        private readonly IPermissionRepository _permissionRepository = permissionRepository;

        public async Task<GetAllResult<List<GetPermissionDTO>>> Handle(GetPermissionsQuery request, CancellationToken cancellationToken)
        {
            var permissions = await _permissionRepository.GetAllAsync(request.GenericFiltersDTO, request.Sort, cancellationToken);

            var result = permissions.Select(x => new GetPermissionDTO
            {
                Id = x.Id,
                IsActive = x.IsActive,
                Name = x.Name
            }).ToList();

            PaginationInfo? paginationInfo = null;

            if (request.GenericFiltersDTO.UsePagination == true)
            {
                paginationInfo = new PaginationInfo
                {
                    CurrentPage = request.GenericFiltersDTO.PageNumber,
                    PageSize = request.GenericFiltersDTO.PageSize,
                    TotalCount = await _permissionRepository.GetCountAsync(request.GenericFiltersDTO, cancellationToken)
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

            return GetAllResult<List<GetPermissionDTO>>.Success(result, paginationInfo: paginationInfo, sortInfo: sortInfo);
        }
    }
}
