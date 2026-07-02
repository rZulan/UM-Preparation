using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Application.DTO.Warehouse;
using Application.Interfaces;
using Application.Results;
using MediatR;

namespace Application.Features.Warehouses.Queries
{
    /// <summary>Query to retrieve a filtered, sorted, and paginated list of warehouses.</summary>
    /// <param name="GenericFiltersDTO">Search and pagination filters.</param>
    /// <param name="Sort">Sort direction and field.</param>
    public record GetWarehousesQuery(GenericFiltersDTO GenericFiltersDTO, Sort Sort) : IRequest<GetAllResult<List<GetWarehouseDTO>>>;
    public class GetWarehousesQueryHandler(IWarehouseRepository warehouseRepository) : IRequestHandler<GetWarehousesQuery, GetAllResult<List<GetWarehouseDTO>>>
    {
        private readonly IWarehouseRepository _warehouseRepository = warehouseRepository;

        public async Task<GetAllResult<List<GetWarehouseDTO>>> Handle(GetWarehousesQuery request, CancellationToken cancellationToken)
        {
            var warehouses = await _warehouseRepository.GetAllAsync(request.GenericFiltersDTO, request.Sort, cancellationToken);

            var result = warehouses.Select(x => new GetWarehouseDTO
            {
                Id = x.Id,
                IsActive = x.IsActive,
                Name = x.Name
            }).ToList();

            PaginationInfo? paginationInfo = null;

            if (request.GenericFiltersDTO.UsePagination == true)
            {
                paginationInfo = new PaginationInfo
                {
                    CurrentPage = request.GenericFiltersDTO.PageNumber,
                    PageSize = request.GenericFiltersDTO.PageSize,
                    TotalCount = await _warehouseRepository.GetCountAsync(request.GenericFiltersDTO, cancellationToken)
                };
            }

            var sortInfo = new SortInfo
            {
                SortColumns = ["id", "name"],
                CurrentSort = request.Sort != null ? new CurrentSort
                {
                    Column = request.Sort.SortBy,
                    Direction = request.Sort.SortDirection
                } : null
            };

            return GetAllResult<List<GetWarehouseDTO>>.Success(result, paginationInfo: paginationInfo, sortInfo: sortInfo);
        }
    }
}
