using Application.DTO.WarehouseReceiving;
using Application.Interfaces;
using Application.Results;
using MediatR;
using System.Net;

namespace Application.Features.WarehouseReceivings.Queries
{
    /// <summary>Query to retrieve a single warehouse entry by its ID.</summary>
    /// <param name="Id">The unique identifier of the warehouse entry to retrieve.</param>
    public record GetWarehouseReceivingByIdQuery(int Id) : IRequest<Result<GetWarehouseReceivingDto>>;
    public class GetWarehouseReceivingByIdQueryHandler(IWarehouseReceivingRepository warehouseReceivingRepository) : IRequestHandler<GetWarehouseReceivingByIdQuery, Result<GetWarehouseReceivingDto>>
    {
        private readonly IWarehouseReceivingRepository _warehouseReceivingRepository = warehouseReceivingRepository;

        public async Task<Result<GetWarehouseReceivingDto>> Handle(GetWarehouseReceivingByIdQuery request, CancellationToken cancellationToken)
        {
            var warehouseReceiving = await _warehouseReceivingRepository.GetByIdAsync(request.Id, cancellationToken);

            if (warehouseReceiving == null)
            {
                return Result<GetWarehouseReceivingDto>.Failure("Warehouse Receiving not found", HttpStatusCode.NotFound);
            }

            var result = new GetWarehouseReceivingDto
            {
                Id = warehouseReceiving.Id,
                WarehouseId = warehouseReceiving.WarehouseId,
                Warehouse = warehouseReceiving.Warehouse.Name,
                Quantity = warehouseReceiving.Quantity,
                ProductId = warehouseReceiving.ProductId,
                ProductCode = warehouseReceiving.Product.ItemCode,
                ProductDescription = warehouseReceiving.Product.Description,
                IsInteger = warehouseReceiving.Product.Uom.IsInteger,
            };

            return Result<GetWarehouseReceivingDto>.Success(result);
        }
    }
}
