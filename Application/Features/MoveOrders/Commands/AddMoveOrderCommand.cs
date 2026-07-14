using System.Net;
using Application.DTO.MoveOrder;
using Application.Interfaces;
using Application.Results;
using Domain.Entities;
using Domain.Entities.Junction;
using MediatR;

namespace Application.Features.MoveOrders.Commands;

public class AvailableMoveOrderProductWarehouseReceivingsDto
{
    public required int ProductId { get; set; }
    public required int WarehouseReceivingId { get; set; }
    public required decimal AvailableQuantity { get; set; }
}

/// <summary>Command to create a new product.</summary>
/// <param name="UserId">The ID of the authenticated user performing the action.</param>
/// <param name="AddMoveOrderDto">The product data to be created.</param>
public record AddMoveOrderCommand(int? UserId, AddMoveOrderDto AddMoveOrderDto) : IRequest<Result<object>>;

public class AddMoveOrderCommandHandler(
    IMoveOrderRepository moveOrderRepository,
    IUserRepository userRepository,
    IProductRepository productRepository,
    IWarehouseReceivingRepository warehouseReceivingRepository) : IRequestHandler<AddMoveOrderCommand, Result<object>>
{
    private readonly IMoveOrderRepository _moveOrderRepository = moveOrderRepository;
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IWarehouseReceivingRepository _warehouseReceivingRepository = warehouseReceivingRepository;

    public async Task<Result<object>> Handle(AddMoveOrderCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId == null) return Result<object>.Failure("User is not signed in", HttpStatusCode.Unauthorized);

        var existingUser = await _userRepository.GetByIdAsync(request.UserId.Value, cancellationToken);

        if (existingUser == null) return Result<object>.Failure("User not found", HttpStatusCode.NotFound);

        foreach (var product in request.AddMoveOrderDto.AddMoveOrderProducts)
        {
            var duplicateCount = request.AddMoveOrderDto.AddMoveOrderProducts
                .Count(p => p.ProductId == product.ProductId);

            if (duplicateCount > 1) return Result<object>.Failure("Duplicate product found");
        }

        var existingProducts = await _productRepository.AllExistsAsync(
            [.. request.AddMoveOrderDto.AddMoveOrderProducts.Select(x => x.ProductId)], cancellationToken);

        if (!existingProducts) return Result<object>.Failure("One or more products do not exist");

        var products = await _productRepository.GetByIdsAsync(
            [.. request.AddMoveOrderDto.AddMoveOrderProducts.Select(x => x.ProductId)], cancellationToken);

        var consolidatedProducts = products.Select(x => new ConsolidatedProductsDto
        {
            Id = x.Id,
            Quantity = request.AddMoveOrderDto.AddMoveOrderProducts
                .Where(p => p.ProductId == x.Id)
                .Sum(p => p.Quantity),
            ItemCode = x.ItemCode,
            Description = x.Description
        }).ToList();

        foreach (var product in consolidatedProducts)
        {
            var hasAvailableReserve =
                await _warehouseReceivingRepository.ProductHasAvailableReserve(request.AddMoveOrderDto.WarehouseId,
                    product.Id, product.Quantity, cancellationToken);

            if (!hasAvailableReserve)
                return Result<object>.Failure($"Not enough reserve for product {product.ItemCode}");
        }

        var moveOrder = new MoveOrder
        {
            WarehouseId = request.AddMoveOrderDto.WarehouseId,
            CreatedAt = DateTime.UtcNow,
            CreatedById = existingUser.Id
        };

        foreach (var product in consolidatedProducts)
        {
            var moveOrderProduct = new MoveOrderProducts
            {
                MoveOrderId = 0,
                ProductId = product.Id,
                TotalQuantity = product.Quantity,
                MoveOrder = moveOrder
            };

            var affectedWarehouseReceivings =
                await _warehouseReceivingRepository.GetProductAffectedWarehouseReceivings(
                    request.AddMoveOrderDto.WarehouseId, product.Id, product.Quantity, cancellationToken);

            foreach (var warehouseReceiving in affectedWarehouseReceivings)
            {
                var moveOrderProductWarehouseReceiving = new MoveOrderProductWarehouseReceivings
                {
                    Quantity = warehouseReceiving.AvailableQuantity,
                    MoveOrderId = 0,
                    ProductId = product.Id,
                    WarehouseReceivingId = warehouseReceiving.WarehouseReceivingId,
                    MoveOrderProduct = moveOrderProduct
                };

                moveOrderProduct.MoveOrderProductWarehouseReceivings.Add(moveOrderProductWarehouseReceiving);
            }

            moveOrder.MoveOrderProducts.Add(moveOrderProduct);
        }

        await _moveOrderRepository.AddAsync(moveOrder, cancellationToken);

        return Result<object>.Success(moveOrder.Id, "MoveOrder created successfully", HttpStatusCode.Created);
    }
}