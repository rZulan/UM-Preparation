using Application.DTO.Warehouse;
using Application.Interfaces;
using Application.Results;
using MediatR;
using System.Net;

namespace Application.Features.Warehouses.Queries
{
    /// <summary>Query to retrieve a warehouse by its ID.</summary>
    /// <param name="Id">The unique identifier of the warehouse.</param>
    public record GetWarehouseByIdQuery(int Id) : IRequest<Result<GetWarehouseDTO>>;
    public class GetWarehouseByIdQueryHandler(IWarehouseRepository warehouseRepository) : IRequestHandler<GetWarehouseByIdQuery, Result<GetWarehouseDTO>>
    {
        private readonly IWarehouseRepository _warehouseRepository = warehouseRepository;

        public async Task<Result<GetWarehouseDTO>> Handle(GetWarehouseByIdQuery request, CancellationToken cancellationToken)
        {
            var warehouse = await _warehouseRepository.GetByIdAsync(request.Id, cancellationToken);

            if (warehouse == null)
            {
                return Result<GetWarehouseDTO>.Failure("Warehouse not found", HttpStatusCode.NotFound);
            }

            var result = new GetWarehouseDTO
            {
                Id = warehouse.Id,
                IsActive = warehouse.IsActive,
                Name = warehouse.Name
            };

            return Result<GetWarehouseDTO>.Success(result);
        }
    }
}
