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
                query = query.Where(x => x.IsActive == genericFiltersDTO.IsActive);
            }

            if (!string.IsNullOrEmpty(genericFiltersDTO.SearchTerm))
            {
                string searchTerm = genericFiltersDTO.SearchTerm.ToLower();
                query = query.Where(x =>
                    x.Id.ToString().Contains(searchTerm) ||
                    x.Name.ToLower().Contains(searchTerm));
            }

            if (sort.SortBy != null)
            {
                bool isAsc = string.Equals(sort.SortDirection, "asc", StringComparison.OrdinalIgnoreCase);
                bool isDsc = string.Equals(sort.SortDirection, "dsc", StringComparison.OrdinalIgnoreCase);

                query = sort.SortBy.ToLower() switch
                {
                    "id" when isAsc => query.OrderBy(x => x.Id),
                    "id" when isDsc => query.OrderByDescending(x => x.Id),
                    "name" when isAsc => query.OrderBy(x => x.Name),
                    "name" when isDsc => query.OrderByDescending(x => x.Name),
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
                query = query.Where(x => x.IsActive == genericFiltersDTO.IsActive);
            }

            if (!string.IsNullOrEmpty(genericFiltersDTO.SearchTerm))
            {
                string searchTerm = genericFiltersDTO.SearchTerm.ToLower();
                query = query.Where(x =>
                    x.Id.ToString().Contains(searchTerm) ||
                    x.Name.ToLower().Contains(searchTerm));
            }

            return await query.CountAsync(cancellationToken);
        }

        public async Task<Permission?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            IQueryable<Permission> query = _context.Permissions
                .Include(x => x.RolePermissions!)
                    .ThenInclude(x => x.Role)
                .Where(x => x.Id == id);

            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Permission?> GetByNameAsync(string name, CancellationToken cancellationToken)
        {
            IQueryable<Permission> query = _context.Permissions
                .Include(x => x.RolePermissions!)
                    .ThenInclude(x => x.Role)
                .Where(x => x.Name.ToLower() == name.ToLower());

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

        public async Task<bool> AnyDuplicateAsync(int id, string name, CancellationToken cancellationToken)
        {
            return await _context.Permissions.AnyAsync(x => x.Id != id && x.Name.ToLower() == name.ToLower(), cancellationToken);
        }
    }
}
