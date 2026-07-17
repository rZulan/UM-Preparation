using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class MiscellaneousReceiptRepository(AppDbContext context) : IMiscellaneousReceiptRepository
    {
        public async Task<List<MiscellaneousReceipt>> GetAllAsync(GenericFiltersDto genericFiltersDTO, Sort sort,
            CancellationToken cancellationToken)
        {
            IQueryable<MiscellaneousReceipt> query = context.MiscellaneousReceipts
                .Include(x => x.Warehouse)
                .Include(x => x.MiscellaneousReceiptProducts)
                .ThenInclude(x => x.Product)
                .ThenInclude(x => x.Uom);

            if (genericFiltersDTO.IsActive != null)
            {
                query = query.Where(x => x.IsActive == genericFiltersDTO.IsActive);
            }

            if (!string.IsNullOrEmpty(genericFiltersDTO.SearchTerm))
            {
                string searchTerm = genericFiltersDTO.SearchTerm.ToLower();
                query = query.Where(x =>
                    x.Id.ToString().Contains(searchTerm) ||
                    x.WarehouseId.ToString().Contains(searchTerm) ||
                    x.Warehouse.Name.ToLower().Contains(searchTerm) ||
                    x.MiscellaneousReceiptProducts.Any(product =>
                        product.ProductId.ToString().Contains(searchTerm) ||
                        product.Product.ItemCode.ToLower().Contains(searchTerm) ||
                        product.Product.Description.ToLower().Contains(searchTerm) ||
                        product.Product.Uom.ShortName.ToLower().Contains(searchTerm) ||
                        product.Quantity.ToString().Contains(searchTerm)) ||
                    x.Reason.ToLower().Contains(searchTerm));
            }

            if (sort?.SortBy != null)
            {
                bool isAsc = string.Equals(sort.SortDirection, "asc", StringComparison.OrdinalIgnoreCase);
                bool isDsc = string.Equals(sort.SortDirection, "dsc", StringComparison.OrdinalIgnoreCase);

                query = sort.SortBy.ToLower() switch
                {
                    "id" when isAsc => query.OrderBy(x => x.Id),
                    "id" when isDsc => query.OrderByDescending(x => x.Id),
                    "itemcode" when isAsc => query.OrderBy(x =>
                        x.MiscellaneousReceiptProducts.Min(product => product.Product.ItemCode)),
                    "itemcode" when isDsc => query.OrderByDescending(x =>
                        x.MiscellaneousReceiptProducts.Min(product => product.Product.ItemCode)),
                    "description" when isAsc => query.OrderBy(x =>
                        x.MiscellaneousReceiptProducts.Min(product => product.Product.Description)),
                    "description" when isDsc => query.OrderByDescending(x =>
                        x.MiscellaneousReceiptProducts.Min(product => product.Product.Description)),
                    "uom" when isAsc => query.OrderBy(x =>
                        x.MiscellaneousReceiptProducts.Min(product => product.Product.Uom.ShortName)),
                    "uom" when isDsc => query.OrderByDescending(x =>
                        x.MiscellaneousReceiptProducts.Min(product => product.Product.Uom.ShortName)),
                    "quantity" when isAsc => query.OrderBy(x =>
                        x.MiscellaneousReceiptProducts.Sum(product => product.Quantity)),
                    "quantity" when isDsc => query.OrderByDescending(x =>
                        x.MiscellaneousReceiptProducts.Sum(product => product.Quantity)),
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
            IQueryable<MiscellaneousReceipt> query = context.MiscellaneousReceipts;

            if (genericFiltersDTO.IsActive != null)
            {
                query = query.Where(x => x.IsActive == genericFiltersDTO.IsActive);
            }

            if (!string.IsNullOrEmpty(genericFiltersDTO.SearchTerm))
            {
                string searchTerm = genericFiltersDTO.SearchTerm.ToLower();
                query = query.Where(x =>
                    x.Id.ToString().Contains(searchTerm) ||
                    x.WarehouseId.ToString().Contains(searchTerm) ||
                    x.Warehouse.Name.ToLower().Contains(searchTerm) ||
                    x.MiscellaneousReceiptProducts.Any(product =>
                        product.ProductId.ToString().Contains(searchTerm) ||
                        product.Product.ItemCode.ToLower().Contains(searchTerm) ||
                        product.Product.Description.ToLower().Contains(searchTerm) ||
                        product.Product.Uom.ShortName.ToLower().Contains(searchTerm) ||
                        product.Quantity.ToString().Contains(searchTerm)) ||
                    x.Reason.ToLower().Contains(searchTerm));
            }

            return await query.CountAsync(cancellationToken);
        }

        public async Task<MiscellaneousReceipt?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await context.MiscellaneousReceipts
                .Include(x => x.Warehouse)
                .Include(x => x.MiscellaneousReceiptProducts)
                .ThenInclude(x => x.Product)
                .ThenInclude(x => x.Uom)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task AddAsync(MiscellaneousReceipt miscellaneousReceipt, CancellationToken cancellationToken)
        {
            await context.MiscellaneousReceipts.AddAsync(miscellaneousReceipt, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(MiscellaneousReceipt miscellaneousReceipt, CancellationToken cancellationToken)
        {
            context.MiscellaneousReceipts.Update(miscellaneousReceipt);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
