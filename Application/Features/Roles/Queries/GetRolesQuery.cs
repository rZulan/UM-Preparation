using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Application.DTO.Permission;
using Application.DTO.Role;
using Application.Interfaces;
using Application.Results;
using MediatR;

namespace Application.Features.Roles.Queries
{
    /// <summary>Query to retrieve a filtered, sorted, and paginated list of roles.</summary>
    /// <param name="GenericFiltersDTO">Search and pagination filters.</param>
    /// <param name="Sort">Sort direction and field.</param>
    public record GetRolesQuery(GenericFiltersDTO GenericFiltersDTO, Sort Sort) : IRequest<GetAllResult<List<GetRoleDTO>>>;
    public class GetRolesQueryHandler(IRoleRepository roleRepository) : IRequestHandler<GetRolesQuery, GetAllResult<List<GetRoleDTO>>>
    {
        private readonly IRoleRepository _roleRepository = roleRepository;

        public async Task<GetAllResult<List<GetRoleDTO>>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
        {
            var roles = await _roleRepository.GetAllAsync(request.GenericFiltersDTO, request.Sort, cancellationToken);

            var result = roles.Select(x => new GetRoleDTO
            {
                Id = x.Id,
                IsActive = x.IsActive,
                Name = x.Name,
                Permissions = x.RolePermissions
                    .Where(rp => rp.Permission != null)
                    .Select(rp => new GetPermissionDTO
                    {
                        Id = rp.Permission!.Id,
                        IsActive = rp.Permission!.IsActive,
                        Name = rp.Permission.Name,
                    }).ToList() ?? []
            }).ToList();

            PaginationInfo? paginationInfo = null;

            if (request.GenericFiltersDTO.UsePagination == true)
            {
                paginationInfo = new PaginationInfo
                {
                    CurrentPage = request.GenericFiltersDTO.PageNumber,
                    PageSize = request.GenericFiltersDTO.PageSize,
                    TotalCount = await _roleRepository.GetCountAsync(request.GenericFiltersDTO, cancellationToken)
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

            return GetAllResult<List<GetRoleDTO>>.Success(result, paginationInfo: paginationInfo, sortInfo: sortInfo);
        }
    }
}
