using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Application.DTO.User;
using Application.Interfaces;
using Application.Results;
using MediatR;

namespace Application.Features.Users.Queries;

/// <summary>Query to retrieve a filtered, sorted, and paginated list of users.</summary>
/// <param name="GenericFiltersDTO">Search and pagination filters.</param>
/// <param name="Sort">Sort direction and field.</param>
public record GetUsersQuery(GenericFiltersDto GenericFiltersDTO, Sort Sort) : IRequest<GetAllResult<List<GetUserDto>>>;

public class GetUsersQueryHandler(IUserRepository userRepository)
    : IRequestHandler<GetUsersQuery, GetAllResult<List<GetUserDto>>>
{
    public async Task<GetAllResult<List<GetUserDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await userRepository.GetAllAsync(request.GenericFiltersDTO, request.Sort, cancellationToken);

        var result = users.Select(u => new GetUserDto
        {
            Id = u.Id,
            IsActive = u.IsActive,
            Username = u.Username,
            FirstName = u.FirstName,
            MiddleName = u.MiddleName ?? "N/A",
            LastName = u.LastName,
            Suffix = u.Suffix ?? "N/A",
            IDPrefix = u.IDPrefix,
            IDNumber = u.IDNumber,
            Role = u.UserRoles.FirstOrDefault()?.Role?.Name ?? "N/A",
            Warehouse = u.Warehouse?.Name ?? "N/A",
            Permissions =
            [
                .. u.UserRoles
                    .SelectMany(ur => ur.Role!.RolePermissions)
                    .Select(rp => rp.Permission!.Name)
                    .Distinct()
            ]
        }).ToList() ?? [];

        PaginationInfo? paginationInfo = null;

        if (request.GenericFiltersDTO.UsePagination)
            paginationInfo = new PaginationInfo
            {
                CurrentPage = request.GenericFiltersDTO.PageNumber,
                PageSize = request.GenericFiltersDTO.PageSize,
                TotalCount = await userRepository.GetCountAsync(request.GenericFiltersDTO, cancellationToken)
            };

        var sortInfo = new SortInfo
        {
            SortColumns =
            [
                "id", "username", "firstname", "middlename", "lastname", "suffix", "idprefix", "idnumber",
                "warehouseid", "warehouse"
            ],
            CurrentSort = request.Sort != null
                ? new CurrentSort
                {
                    Column = request.Sort.SortBy,
                    Direction = request.Sort.SortDirection
                }
                : null
        };

        return GetAllResult<List<GetUserDto>>.Success(result, paginationInfo: paginationInfo, sortInfo: sortInfo);
    }
}