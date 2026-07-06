using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Application.Interfaces;
using Domain.Entities.Masterlist;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class RoleRepository(AppDbContext context) : IRoleRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<List<Role>> GetAllAsync(GenericFiltersDTO genericFiltersDTO, Sort sort, CancellationToken cancellationToken)
        {
            IQueryable<Role> query = _context.Roles
                .Include(x => x.RolePermissions)
                    .ThenInclude(x => x.Permission);

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
            IQueryable<Role> query = _context.Roles;

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

        public async Task<Role?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            IQueryable<Role> query = _context.Roles
                .Include(x => x.RolePermissions!)
                    .ThenInclude(x => x.Permission)
                .Where(x => x.Id == id);

            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<Role>> GetByIdsAsync(List<int> ids, CancellationToken cancellationToken)
        {
            IQueryable<Role> query = _context.Roles
                .Include(x => x.RolePermissions!)
                    .ThenInclude(x => x.Permission)
                .Where(x => ids.Contains(x.Id));

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken)
        {
            IQueryable<Role> query = _context.Roles
                .Include(x => x.RolePermissions!)
                    .ThenInclude(x => x.Permission)
                .Where(x => x.Name.ToLower() == name.ToLower());

            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task AddAsync(Role role, CancellationToken cancellationToken)
        {
            await _context.Roles.AddAsync(role, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Role role, CancellationToken cancellationToken)
        {
            _context.Roles.Update(role);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> AnyDuplicateAsync(int id, string name, CancellationToken cancellationToken)
        {
            return await _context.Roles.AnyAsync(x => x.Id != id && x.Name.ToLower() == name.ToLower(), cancellationToken);
        }
    }
}
