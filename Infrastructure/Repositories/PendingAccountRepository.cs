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
                query = query.Where(r => r.IsActive == genericFiltersDTO.IsActive);
            }

            if (!string.IsNullOrEmpty(genericFiltersDTO.SearchTerm))
            {
                string searchTerm = genericFiltersDTO.SearchTerm.ToLower();
                query = query.Where(r =>
                    r.Id.ToString().Contains(searchTerm) ||
                    r.EmployeePrefix.ToLower().Contains(searchTerm) ||
                    r.EmployeeId.ToLower().Contains(searchTerm) ||
                    r.Username.ToLower().Contains(searchTerm) ||
                    r.FirstName.ToLower().Contains(searchTerm) ||
                    r.MiddleName.ToLower().Contains(searchTerm) ||
                    r.LastName.ToLower().Contains(searchTerm) ||
                    r.Suffix.ToLower().Contains(searchTerm));
            }

            if (sort.SortBy != null)
            {
                bool isAsc = string.Equals(sort.SortDirection, "asc", StringComparison.OrdinalIgnoreCase);
                bool isDsc = string.Equals(sort.SortDirection, "dsc", StringComparison.OrdinalIgnoreCase);

                query = sort.SortBy.ToLower() switch
                {
                    "id" when isAsc => query.OrderBy(r => r.Id),
                    "id" when isDsc => query.OrderByDescending(r => r.Id),
                    "employeeprefix" when isAsc => query.OrderBy(r => r.EmployeePrefix),
                    "employeeprefix" when isDsc => query.OrderByDescending(r => r.EmployeePrefix),
                    "employeeid" when isAsc => query.OrderBy(r => r.EmployeeId),
                    "employeeid" when isDsc => query.OrderByDescending(r => r.EmployeeId),
                    "username" when isAsc => query.OrderBy(r => r.Username),
                    "username" when isDsc => query.OrderByDescending(r => r.Username),
                    "firstname" when isAsc => query.OrderBy(r => r.FirstName),
                    "firstname" when isDsc => query.OrderByDescending(r => r.FirstName),
                    "middlename" when isAsc => query.OrderBy(r => r.MiddleName),
                    "middlename" when isDsc => query.OrderByDescending(r => r.MiddleName),
                    "lastname" when isAsc => query.OrderBy(r => r.LastName),
                    "lastname" when isDsc => query.OrderByDescending(r => r.LastName),
                    "suffix" when isAsc => query.OrderBy(r => r.Suffix),
                    "suffix" when isDsc => query.OrderByDescending(r => r.Suffix),
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
                query = query.Where(r => r.IsActive == genericFiltersDTO.IsActive);
            }

            if (!string.IsNullOrEmpty(genericFiltersDTO.SearchTerm))
            {
                string searchTerm = genericFiltersDTO.SearchTerm.ToLower();
                query = query.Where(r =>
                    r.Id.ToString().Contains(searchTerm) ||
                    r.EmployeePrefix.ToLower().Contains(searchTerm) ||
                    r.EmployeeId.ToLower().Contains(searchTerm) ||
                    r.Username.ToLower().Contains(searchTerm) ||
                    r.FirstName.ToLower().Contains(searchTerm) ||
                    (r.MiddleName ?? "").ToLower().Contains(searchTerm) ||
                    r.LastName.ToLower().Contains(searchTerm) ||
                    (r.Suffix ?? "").ToLower().Contains(searchTerm));
            }

            return await query.CountAsync(cancellationToken);
        }

        public async Task<PendingAccount?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            IQueryable<PendingAccount> query = _context.PendingAccounts
                .Where(r => r.Id == id);

            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<PendingAccount?> GetByFullIdNoAsync(string employeePrefix, string employeeId, CancellationToken cancellationToken)
        {
            IQueryable<PendingAccount> query = _context.PendingAccounts
                .Where(r => r.EmployeePrefix == employeePrefix && r.EmployeeId == employeeId);

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
