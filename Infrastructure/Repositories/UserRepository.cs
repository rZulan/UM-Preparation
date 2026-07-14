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

        public async Task<List<User>> GetAllAsync(GenericFiltersDto genericFiltersDTO, Sort userSort,
            CancellationToken cancellationToken)
        {
            IQueryable<User> query = _context.Users
                .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role!)
                .ThenInclude(x => x.RolePermissions)
                .ThenInclude(x => x.Permission)
                .Include(x => x.Warehouse);

            if (genericFiltersDTO.IsActive != null)
            {
                query = query.Where(x => x.IsActive == genericFiltersDTO.IsActive);
            }

            if (!string.IsNullOrEmpty(genericFiltersDTO.SearchTerm))
            {
                string searchTerm = genericFiltersDTO.SearchTerm.ToLower();
                query = query.Where(x =>
                    x.Id.ToString().Contains(searchTerm) ||
                    x.Username.ToLower().Contains(searchTerm) ||
                    x.FirstName.ToLower().Contains(searchTerm) ||
                    (x.MiddleName ?? "").ToLower().Contains(searchTerm) ||
                    x.LastName.ToLower().Contains(searchTerm) ||
                    (x.Suffix ?? "").ToLower().Contains(searchTerm) ||
                    x.IDPrefix.ToLower().Contains(searchTerm) ||
                    x.IDNumber.ToLower().Contains(searchTerm) ||
                    x.Warehouse!.Name.ToLower().Contains(searchTerm)
                );
            }

            if (userSort.SortBy != null)
            {
                bool isAsc = string.Equals(userSort.SortDirection, "asc", StringComparison.OrdinalIgnoreCase);
                bool isDsc = string.Equals(userSort.SortDirection, "dsc", StringComparison.OrdinalIgnoreCase);

                query = userSort.SortBy.ToLower() switch
                {
                    "id" when isAsc => query.OrderBy(x => x.Id),
                    "id" when isDsc => query.OrderByDescending(x => x.Id),
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
                    "idprefix" when isAsc => query.OrderBy(x => x.IDPrefix),
                    "idprefix" when isDsc => query.OrderByDescending(x => x.IDPrefix),
                    "idnumber" when isAsc => query.OrderBy(x => x.IDNumber),
                    "idnumber" when isDsc => query.OrderByDescending(x => x.IDNumber),
                    "warehouseid" when isAsc => query.OrderBy(x => x.WarehouseId),
                    "warehouseid" when isDsc => query.OrderByDescending(x => x.WarehouseId),
                    "warehouse" when isAsc => query.OrderBy(x => x.Warehouse!.Name),
                    "warehouse" when isDsc => query.OrderByDescending(x => x.Warehouse!.Name),
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

        public async Task<int> GetCountAsync(GenericFiltersDto genericFiltersDTO, CancellationToken cancellationToken)
        {
            IQueryable<User> query = _context.Users;

            if (genericFiltersDTO.IsActive != null)
            {
                query = query.Where(x => x.IsActive == genericFiltersDTO.IsActive);
            }

            if (!string.IsNullOrEmpty(genericFiltersDTO.SearchTerm))
            {
                string searchTerm = genericFiltersDTO.SearchTerm.ToLower();
                query = query.Where(x =>
                    x.Id.ToString().Contains(searchTerm) ||
                    x.Username.ToLower().Contains(searchTerm) ||
                    x.FirstName.ToLower().Contains(searchTerm) ||
                    (x.MiddleName ?? "").ToLower().Contains(searchTerm) ||
                    x.LastName.ToLower().Contains(searchTerm) ||
                    (x.Suffix ?? "").ToLower().Contains(searchTerm) ||
                    x.IDPrefix.ToLower().Contains(searchTerm) ||
                    x.IDNumber.ToLower().Contains(searchTerm) ||
                    x.Warehouse!.Name.ToLower().Contains(searchTerm)
                );
            }

            return await query.CountAsync(cancellationToken);
        }

        public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken)
        {
            IQueryable<User> query = _context.Users
                .Include(x => x.UserRoles!)
                .ThenInclude(x => x.Role!)
                .ThenInclude(x => x.RolePermissions)
                .ThenInclude(x => x.Permission)
                .Include(x => x.Warehouse)
                .Where(x => x.Username.ToLower() == username.ToLower());

            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            IQueryable<User> query = _context.Users
                .Include(x => x.UserRoles!)
                .ThenInclude(x => x.Role!)
                .ThenInclude(x => x.RolePermissions)
                .ThenInclude(x => x.Permission)
                .Include(x => x.Warehouse)
                .Where(x => x.Id == id);

            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<User?> GetByFullIdNoAsync(string employeePrefix, string employeeId,
            CancellationToken cancellationToken)
        {
            IQueryable<User> query = _context.Users
                .Include(x => x.UserRoles!)
                .ThenInclude(x => x.Role!)
                .ThenInclude(x => x.RolePermissions)
                .ThenInclude(x => x.Permission)
                .Include(x => x.Warehouse)
                .Where(x => x.IDPrefix == employeePrefix && x.IDNumber == employeeId);

            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<bool> AnyUsersWarehouseTaggedAsync(int warehouseId, CancellationToken cancellationToken)
        {
            IQueryable<User> query = _context.Users
                .Include(x => x.UserRoles!)
                .ThenInclude(x => x.Role!)
                .ThenInclude(x => x.RolePermissions)
                .ThenInclude(x => x.Permission)
                .Include(x => x.Warehouse)
                .Where(x => x.WarehouseId == warehouseId);

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
            return await _context.Users.AnyAsync(x => x.Id != id && x.Username.ToLower() == username.ToLower(),
                cancellationToken);
        }
    }
}