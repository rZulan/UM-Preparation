using Application.DTO.MoveOrder;
using Application.Interfaces;
using Application.Results;
using MediatR;
using System.Net;

namespace Application.Features.MoveOrders.Queries
{
    public record GetMoveOrderByIdQuery(int Id) : IRequest<Result<GetMoveOrderDto>>;

    public class GetMoveOrderByIdQueryHandler(IMoveOrderRepository moveOrderRepository) : IRequestHandler<GetMoveOrderByIdQuery, Result<GetMoveOrderDto>>
    {
        private readonly IMoveOrderRepository _moveOrderRepository = moveOrderRepository;

        public async Task<Result<GetMoveOrderDto>> Handle(GetMoveOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var moveOrder = await _moveOrderRepository.GetByIdAsync(request.Id, cancellationToken);

            if (moveOrder == null)
            {
                return Result<GetMoveOrderDto>.Failure("Move order not found", HttpStatusCode.NotFound);
            }

            var result = new GetMoveOrderDto
            {
                Id = moveOrder.Id,
                IsActive = moveOrder.IsActive,
                IsTransacted = moveOrder.IsTransacted,
                CreatedAt = moveOrder.CreatedAt,
                WarehouseId = moveOrder.WarehouseId,
                Warehouse = moveOrder.Warehouse.Name,
                MoveOrderProducts = moveOrder.MoveOrderProducts.Select(product => new GetMoveOrderProductDTO
                {
                    ProductId = product.ProductId,
                    ItemCode = product.Product.ItemCode,
                    Description = product.Product.Description,
                    TotalQuantity = product.TotalQuantity
                }).ToList()
            };

            return Result<GetMoveOrderDto>.Success(result);
        }
    }
}
