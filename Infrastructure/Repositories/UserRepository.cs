using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UserRepository(AppDbContext context) : IUserRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<List<User>> GetAllAsync(GenericFiltersDTO genericFiltersDTO, Sort userSort, CancellationToken cancellationToken)
        {
            IQueryable<User> query = _context.Users
                .Include(ur => ur.UserRoles)
                    .ThenInclude(r => r.Role!)
                        .ThenInclude(rp => rp.RolePermissions)
                            .ThenInclude(p => p.Permission);

            if (genericFiltersDTO.IsActive != null)
            {
                query = query.Where(u => u.IsActive == genericFiltersDTO.IsActive);
            }

            if (!string.IsNullOrEmpty(genericFiltersDTO.SearchTerm))
            {
                query = query.Where(u =>
                    u.Username.Contains(genericFiltersDTO.SearchTerm) ||
                    u.FirstName.Contains(genericFiltersDTO.SearchTerm) ||
                    (u.MiddleName ?? "").Contains(genericFiltersDTO.SearchTerm) ||
                    u.LastName.Contains(genericFiltersDTO.SearchTerm) ||
                    (u.Suffix ?? "").Contains(genericFiltersDTO.SearchTerm) ||
                    u.IDPrefix.Contains(genericFiltersDTO.SearchTerm) ||
                    u.IDNumber.Contains(genericFiltersDTO.SearchTerm)
                );
            }

            if (userSort.SortBy != null)
            {
                bool isAsc = string.Equals(userSort.SortDirection, "asc", StringComparison.OrdinalIgnoreCase);
                bool isDsc = string.Equals(userSort.SortDirection, "dsc", StringComparison.OrdinalIgnoreCase);

                query = userSort.SortBy.ToLower() switch
                {
                    "id" when isAsc => query.OrderBy(u => u.Id),
                    "id" when isDsc => query.OrderByDescending(u => u.Id),
                    "username" when isAsc => query.OrderBy(u => u.Username),
                    "username" when isDsc => query.OrderByDescending(u => u.Username),
                    "firstname" when isAsc => query.OrderBy(u => u.FirstName),
                    "firstname" when isDsc => query.OrderByDescending(u => u.FirstName),
                    "lastname" when isAsc => query.OrderBy(u => u.LastName),
                    "lastname" when isDsc => query.OrderByDescending(u => u.LastName),
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
            IQueryable<User> query = _context.Users;

            if (genericFiltersDTO.IsActive != null)
            {
                query = query.Where(u => u.IsActive == genericFiltersDTO.IsActive);
            }

            if (!string.IsNullOrEmpty(genericFiltersDTO.SearchTerm))
            {
                query = query.Where(u =>
                    u.Username.Contains(genericFiltersDTO.SearchTerm) ||
                    u.FirstName.Contains(genericFiltersDTO.SearchTerm) ||
                    (u.MiddleName ?? "").Contains(genericFiltersDTO.SearchTerm) ||
                    u.LastName.Contains(genericFiltersDTO.SearchTerm) ||
                    (u.Suffix ?? "").Contains(genericFiltersDTO.SearchTerm) ||
                    u.IDPrefix.Contains(genericFiltersDTO.SearchTerm) ||
                    u.IDNumber.Contains(genericFiltersDTO.SearchTerm)
                );
            }
            return await query.CountAsync(cancellationToken);
        }

        public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken)
        {
            IQueryable<User> query = _context.Users
                .Include(ur => ur.UserRoles!)
                    .ThenInclude(r => r.Role!)
                        .ThenInclude(rp => rp.RolePermissions)
                            .ThenInclude(p => p.Permission)
                .Where(u => u.Username.ToLower() == username.ToLower());

            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            IQueryable<User> query = _context.Users
                .Include(ur => ur.UserRoles!)
                    .ThenInclude(r => r.Role!)
                        .ThenInclude(rp => rp.RolePermissions)
                            .ThenInclude(p => p.Permission)
                .Where(u => u.Id == id);

            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<User?> GetByFullIdNoAsync(string employeePrefix, string employeeId, CancellationToken cancellationToken)
        {
            IQueryable<User> query = _context.Users
                .Include(ur => ur.UserRoles!)
                    .ThenInclude(r => r.Role!)
                        .ThenInclude(rp => rp.RolePermissions)
                            .ThenInclude(p => p.Permission)
                .Where(u => u.IDPrefix == employeePrefix && u.IDNumber == employeeId);

            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task AddAsync(User user, CancellationToken cancellationToken)
        {
            await _context.Users.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(User user, CancellationToken cancellationToken)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> CheckDuplicateAsync(int id, string username, CancellationToken cancellationToken)
        {
            return await _context.Users.AnyAsync(u => u.Id != id && u.Username.ToLower() == username.ToLower(), cancellationToken);
        }
    }
}