using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PendingAccountRepository(AppDbContext context) : IPendingAccountRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<List<PendingAccount>> GetAllAsync(GenericFiltersDTO genericFiltersDTO, Sort sort, CancellationToken cancellationToken)
        {
            IQueryable<PendingAccount> query = _context.PendingAccounts;

            if (genericFiltersDTO.IsActive != null)
            {
                query = query.Where(x => x.IsActive == genericFiltersDTO.IsActive);
            }

            if (!string.IsNullOrEmpty(genericFiltersDTO.SearchTerm))
            {
                string searchTerm = genericFiltersDTO.SearchTerm.ToLower();
                query = query.Where(x =>
                    x.Id.ToString().Contains(searchTerm) ||
                    x.EmployeePrefix.ToLower().Contains(searchTerm) ||
                    x.EmployeeId.ToLower().Contains(searchTerm) ||
                    x.Username.ToLower().Contains(searchTerm) ||
                    x.FirstName.ToLower().Contains(searchTerm) ||
                    (x.MiddleName ?? "").ToLower().Contains(searchTerm) ||
                    x.LastName.ToLower().Contains(searchTerm) ||
                    (x.Suffix ?? "").ToLower().Contains(searchTerm));
            }

            if (sort.SortBy != null)
            {
                bool isAsc = string.Equals(sort.SortDirection, "asc", StringComparison.OrdinalIgnoreCase);
                bool isDsc = string.Equals(sort.SortDirection, "dsc", StringComparison.OrdinalIgnoreCase);

                query = sort.SortBy.ToLower() switch
                {
                    "id" when isAsc => query.OrderBy(x => x.Id),
                    "id" when isDsc => query.OrderByDescending(x => x.Id),
                    "employeeprefix" when isAsc => query.OrderBy(x => x.EmployeePrefix),
                    "employeeprefix" when isDsc => query.OrderByDescending(x => x.EmployeePrefix),
                    "employeeid" when isAsc => query.OrderBy(x => x.EmployeeId),
                    "employeeid" when isDsc => query.OrderByDescending(x => x.EmployeeId),
                    "username" when isAsc => query.OrderBy(x => x.Username),
                    "username" when isDsc => query.OrderByDescending(x => x.Username),
                    "firstname" when isAsc => query.OrderBy(x => x.FirstName),
                    "firstname" when isDsc => query.OrderByDescending(x => x.FirstName),
                    "middlename" when isAsc => query.OrderBy(x => x.MiddleName),
                    "middlename" when isDsc => query.OrderByDescending(x => x.MiddleName),
                    "lastname" when isAsc => query.OrderBy(x => x.LastName),
                    "lastname" when isDsc => query.OrderByDescending(x => x.LastName),
                    "suffix" when isAsc => query.OrderBy(x => x.Suffix),
                    "suffix" when isDsc => query.OrderByDescending(x => x.Suffix),
                    _ => query
                };
            }

            if (genericFiltersDTO.UsePagination)
            {
                query = query.Skip((genericFiltersDTO.PageNumber - 1) * genericFiltersDTO.PageSize)
                             .Take(genericFiltersDTO.PageSize);
            }

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<int> GetCountAsync(GenericFiltersDTO genericFiltersDTO, CancellationToken cancellationToken)
        {
            IQueryable<PendingAccount> query = _context.PendingAccounts;

            if (genericFiltersDTO.IsActive != null)
            {
                query = query.Where(x => x.IsActive == genericFiltersDTO.IsActive);
            }

            if (!string.IsNullOrEmpty(genericFiltersDTO.SearchTerm))
            {
                string searchTerm = genericFiltersDTO.SearchTerm.ToLower();
                query = query.Where(x =>
                    x.Id.ToString().Contains(searchTerm) ||
                    x.EmployeePrefix.ToLower().Contains(searchTerm) ||
                    x.EmployeeId.ToLower().Contains(searchTerm) ||
                    x.Username.ToLower().Contains(searchTerm) ||
                    x.FirstName.ToLower().Contains(searchTerm) ||
                    (x.MiddleName ?? "").ToLower().Contains(searchTerm) ||
                    x.LastName.ToLower().Contains(searchTerm) ||
                    (x.Suffix ?? "").ToLower().Contains(searchTerm));
            }

            return await query.CountAsync(cancellationToken);
        }

        public async Task<PendingAccount?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            IQueryable<PendingAccount> query = _context.PendingAccounts
                .Where(x => x.Id == id);

            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<PendingAccount?> GetByFullIdNoAsync(string employeePrefix, string employeeId, CancellationToken cancellationToken)
        {
            IQueryable<PendingAccount> query = _context.PendingAccounts
                .Where(x => x.EmployeePrefix == employeePrefix && x.EmployeeId == employeeId);

            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task AddAsync(PendingAccount permission, CancellationToken cancellationToken)
        {
            await _context.PendingAccounts.AddAsync(permission, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(PendingAccount permission, CancellationToken cancellationToken)
        {
            _context.PendingAccounts.Update(permission);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
