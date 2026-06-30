using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Application.Interfaces;
using Domain.Entities.Masterlist;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PermissionRepository(AppDbContext context) : IPermissionRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<List<Permission>> GetAllAsync(GenericFiltersDTO genericFiltersDTO, Sort sort, CancellationToken cancellationToken)
        {
            IQueryable<Permission> query = _context.Permissions;

            if (genericFiltersDTO.IsActive != null)
            {
                query = query.Where(r => r.IsActive == genericFiltersDTO.IsActive);
            }

            if (!string.IsNullOrEmpty(genericFiltersDTO.SearchTerm))
            {
                query = query.Where(r =>
                    r.Name.Contains(genericFiltersDTO.SearchTerm)
                );
            }

            if (sort.SortBy != null)
            {
                bool isAsc = string.Equals(sort.SortDirection, "asc", StringComparison.OrdinalIgnoreCase);
                bool isDsc = string.Equals(sort.SortDirection, "dsc", StringComparison.OrdinalIgnoreCase);

                query = sort.SortBy.ToLower() switch
                {
                    "id" when isAsc => query.OrderBy(r => r.Id),
                    "id" when isDsc => query.OrderByDescending(r => r.Id),
                    "name" when isAsc => query.OrderBy(r => r.Name),
                    "name" when isDsc => query.OrderByDescending(r => r.Name),
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
            IQueryable<Permission> query = _context.Permissions;

            if (genericFiltersDTO.IsActive != null)
            {
                query = query.Where(r => r.IsActive == genericFiltersDTO.IsActive);
            }

            if (!string.IsNullOrEmpty(genericFiltersDTO.SearchTerm))
            {
                query = query.Where(r =>
                    r.Name.Contains(genericFiltersDTO.SearchTerm)
                );
            }

            return await query.CountAsync(cancellationToken);
        }

        public async Task<Permission?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            IQueryable<Permission> query = _context.Permissions
                .Include(rp => rp.RolePermissions!)
                    .ThenInclude(r => r.Role)
                .Where(r => r.Id == id);

            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Permission?> GetByNameAsync(string name, CancellationToken cancellationToken)
        {
            IQueryable<Permission> query = _context.Permissions
                .Include(rp => rp.RolePermissions!)
                    .ThenInclude(r => r.Role)
                .Where(r => r.Name.ToLower() == name.ToLower());

            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task AddAsync(Permission permission, CancellationToken cancellationToken)
        {
            await _context.Permissions.AddAsync(permission, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Permission permission, CancellationToken cancellationToken)
        {
            _context.Permissions.Update(permission);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> CheckDuplicateAsync(int id, string name, CancellationToken cancellationToken)
        {
            return await _context.Permissions.AnyAsync(r => r.Id != id && r.Name.ToLower() == name.ToLower(), cancellationToken);
        }
    }
}
