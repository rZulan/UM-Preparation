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
                            .ThenInclude(p => p.Permission)
                .Include(w => w.Warehouse);

            if (genericFiltersDTO.IsActive != null)
            {
                query = query.Where(u => u.IsActive == genericFiltersDTO.IsActive);
            }

            if (!string.IsNullOrEmpty(genericFiltersDTO.SearchTerm))
            {
                string searchTerm = genericFiltersDTO.SearchTerm.ToLower();
                query = query.Where(u =>
                    u.Id.ToString().Contains(searchTerm) ||
                    u.Username.ToLower().Contains(searchTerm) ||
                    u.FirstName.ToLower().Contains(searchTerm) ||
                    (u.MiddleName ?? "").ToLower().Contains(searchTerm) ||
                    u.LastName.ToLower().Contains(searchTerm) ||
                    (u.Suffix ?? "").ToLower().Contains(searchTerm) ||
                    u.IDPrefix.ToLower().Contains(searchTerm) ||
                    u.IDNumber.ToLower().Contains(searchTerm) ||
                    u.Warehouse!.Name.ToLower().Contains(searchTerm)
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
                    "middlename" when isAsc => query.OrderBy(u => u.MiddleName),
                    "middlename" when isDsc => query.OrderByDescending(u => u.MiddleName),
                    "lastname" when isAsc => query.OrderBy(u => u.LastName),
                    "lastname" when isDsc => query.OrderByDescending(u => u.LastName),
                    "suffix" when isAsc => query.OrderBy(u => u.Suffix),
                    "suffix" when isDsc => query.OrderByDescending(u => u.Suffix),
                    "idprefix" when isAsc => query.OrderBy(u => u.IDPrefix),
                    "idprefix" when isDsc => query.OrderByDescending(u => u.IDPrefix),
                    "idnumber" when isAsc => query.OrderBy(u => u.IDNumber),
                    "idnumber" when isDsc => query.OrderByDescending(u => u.IDNumber),
                    "warehouseid" when isAsc => query.OrderBy(u => u.WarehouseId),
                    "warehouseid" when isDsc => query.OrderByDescending(u => u.WarehouseId),
                    "warehouse" when isAsc => query.OrderBy(u => u.Warehouse!.Name),
                    "warehouse" when isDsc => query.OrderByDescending(u => u.Warehouse!.Name),
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
                string searchTerm = genericFiltersDTO.SearchTerm.ToLower();
                query = query.Where(u =>
                    u.Id.ToString().Contains(searchTerm) ||
                    u.Username.ToLower().Contains(searchTerm) ||
                    u.FirstName.ToLower().Contains(searchTerm) ||
                    (u.MiddleName ?? "").ToLower().Contains(searchTerm) ||
                    u.LastName.ToLower().Contains(searchTerm) ||
                    (u.Suffix ?? "").ToLower().Contains(searchTerm) ||
                    u.IDPrefix.ToLower().Contains(searchTerm) ||
                    u.IDNumber.ToLower().Contains(searchTerm) ||
                    u.Warehouse!.Name.ToLower().Contains(searchTerm)
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
                .Include(w => w.Warehouse)
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
                .Include(w => w.Warehouse)
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
                .Include(w => w.Warehouse)
                .Where(u => u.IDPrefix == employeePrefix && u.IDNumber == employeeId);

            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<bool> AnyUsersWarehouseTaggedAsync(int warehouseId, CancellationToken cancellationToken)
        {
            IQueryable<User> query = _context.Users
                .Include(ur => ur.UserRoles!)
                    .ThenInclude(r => r.Role!)
                        .ThenInclude(rp => rp.RolePermissions)
                            .ThenInclude(p => p.Permission)
                .Include(w => w.Warehouse)
                .Where(u => u.WarehouseId == warehouseId);

            return await query.AnyAsync(cancellationToken);
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

        public async Task<bool> AnyDuplicateAsync(int id, string username, CancellationToken cancellationToken)
        {
            return await _context.Users.AnyAsync(u => u.Id != id && u.Username.ToLower() == username.ToLower(), cancellationToken);
        }
    }
}