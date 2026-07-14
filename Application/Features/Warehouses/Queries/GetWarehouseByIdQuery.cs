using System.Net;
using Application.DTO.Warehouse;
using Application.Interfaces;
using Application.Results;
using MediatR;

namespace Application.Features.Warehouses.Queries;

/// <summary>Query to retrieve a warehouse by its ID.</summary>
/// <param name="Id">The unique identifier of the warehouse.</param>
public record GetWarehouseByIdQuery(int Id) : IRequest<Result<GetWarehouseDto>>;

public class GetWarehouseByIdQueryHandler(IWarehouseRepository warehouseRepository)
    : IRequestHandler<GetWarehouseByIdQuery, Result<GetWarehouseDto>>
{
    public async Task<Result<GetWarehouseDto>> Handle(GetWarehouseByIdQuery request,
        CancellationToken cancellationToken)
    {
        var warehouse = await warehouseRepository.GetByIdAsync(request.Id, cancellationToken);

        if (warehouse == null) return Result<GetWarehouseDto>.Failure("Warehouse not found", HttpStatusCode.NotFound);

        var result = new GetWarehouseDto
        {
            Id = warehouse.Id,
            IsActive = warehouse.IsActive,
            Name = warehouse.Name
        };

        return Result<GetWarehouseDto>.Success(result);
    }
}