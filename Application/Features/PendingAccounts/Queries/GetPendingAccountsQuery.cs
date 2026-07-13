using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Application.DTO.PendingAccount;
using Application.Interfaces;
using Application.Results;
using MediatR;

namespace Application.Features.PendingAccounts.Queries
{
    /// <summary>Query to retrieve a filtered, sorted, and paginated list of pending accounts.</summary>
    /// <param name="GenericFiltersDTO">Search and pagination filters.</param>
    /// <param name="Sort">Sort direction and field.</param>
    public record GetPendingAccountsQuery(GenericFiltersDto GenericFiltersDTO, Sort Sort) : IRequest<GetAllResult<List<GetPendingAccountDto>>>;
    public class GetPendingAccountsQueryHandler(IPendingAccountRepository pendingAccountRepository) : IRequestHandler<GetPendingAccountsQuery, GetAllResult<List<GetPendingAccountDto>>>
    {
        private readonly IPendingAccountRepository _pendingAccountRepository = pendingAccountRepository;

        public async Task<GetAllResult<List<GetPendingAccountDto>>> Handle(GetPendingAccountsQuery request, CancellationToken cancellationToken)
        {
            var pendingAccounts = await _pendingAccountRepository.GetAllAsync(request.GenericFiltersDTO, request.Sort, cancellationToken);

            var result = pendingAccounts.Select(x => new GetPendingAccountDto
            {
                Id = x.Id,
                IsActive = x.IsActive,
                EmployeePrefix = x.EmployeePrefix,
                EmployeeId = x.EmployeeId,
                Username = x.Username,
                FirstName = x.FirstName,
                MiddleName = x.MiddleName ?? "N/A",
                LastName = x.LastName,
                Suffix = x.Suffix ?? "N/A"
            }).ToList();

            PaginationInfo? paginationInfo = null;

            if (request.GenericFiltersDTO.UsePagination == true)
            {
                paginationInfo = new PaginationInfo
                {
                    CurrentPage = request.GenericFiltersDTO.PageNumber,
                    PageSize = request.GenericFiltersDTO.PageSize,
                    TotalCount = await _pendingAccountRepository.GetCountAsync(request.GenericFiltersDTO, cancellationToken)
                };
            }

            var sortInfo = new SortInfo
            {
                SortColumns = ["id", "employeeprefix", "employeeid", "username", "firstname", "middlename", "lastname", "suffix"],
                CurrentSort = request.Sort != null ? new CurrentSort
                {
                    Column = request.Sort.SortBy,
                    Direction = request.Sort.SortDirection
                } : null
            };

            return GetAllResult<List<GetPendingAccountDto>>.Success(result, paginationInfo: paginationInfo, sortInfo: sortInfo);
        }
    }
}
