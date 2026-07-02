using Application.DTO.WarehouseReceiving;
using Application.Interfaces;
using Application.Results;
using MediatR;
using System.Net;

namespace Application.Features.WarehouseReceivings.Queries
{
    /// <summary>Query to retrieve a single warehouse entry by its ID.</summary>
    /// <param name="Id">The unique identifier of the warehouse entry to retrieve.</param>
    public record GetWarehouseReceivingByIdQuery(int Id) : IRequest<Result<GetWarehouseReceivingDTO>>;
    public class GetWarehouseReceivingByIdQueryHandler(IWarehouseReceivingRepository warehouseReceivingRepository) : IRequestHandler<GetWarehouseReceivingByIdQuery, Result<GetWarehouseReceivingDTO>>
    {
        private readonly IWarehouseReceivingRepository _warehouseReceivingRepository = warehouseReceivingRepository;

        public async Task<Result<GetWarehouseReceivingDTO>> Handle(GetWarehouseReceivingByIdQuery request, CancellationToken cancellationToken)
        {
            var warehouse = await _warehouseReceivingRepository.GetByIdAsync(request.Id, cancellationToken);

            if (warehouse == null)
            {
                return Result<GetWarehouseReceivingDTO>.Failure("Warehouse not found", HttpStatusCode.NotFound);
            }

            var result = new GetWarehouseReceivingDTO
            {
                Id = warehouse.Id,
                Quantity = warehouse.Quantity,
                ProductId = warehouse.ProductId,
                ProductCode = warehouse.Product.ItemCode,
                ProductDescription = warehouse.Product.Description,
                MiscellaneousReceiptId = warehouse.MiscellaneousReceiptId
            };

            return Result<GetWarehouseReceivingDTO>.Success(result);
        }
    }
}
