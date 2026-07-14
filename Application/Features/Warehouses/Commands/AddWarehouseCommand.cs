using System.Net;
using Application.DTO.Warehouse;
using Application.Interfaces;
using Application.Results;
using Domain.Entities.Masterlist;
using MediatR;

namespace Application.Features.Warehouses.Commands;

/// <summary>Command to create a new warehouse.</summary>
/// <param name="UserId">The ID of the authenticated user performing the action.</param>
/// <param name="AddWarehouseDTO">The warehouse data to be created.</param>
public record AddWarehouseCommand(int? UserId, AddWarehouseDto AddWarehouseDTO) : IRequest<Result<object>>;

public class AddWarehouseCommandHandler(IWarehouseRepository warehouseRepository, IUserRepository userRepository)
    : IRequestHandler<AddWarehouseCommand, Result<object>>
{
    public async Task<Result<object>> Handle(AddWarehouseCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId == null) return Result<object>.Failure("User is not signed in", HttpStatusCode.Unauthorized);

        var existingUser = await userRepository.GetByIdAsync(request.UserId.Value, cancellationToken);

        if (existingUser == null) return Result<object>.Failure("User not found", HttpStatusCode.NotFound);

        var existingWarehouse =
            await warehouseRepository.GetByNameAsync(request.AddWarehouseDTO.Name, cancellationToken);

        if (existingWarehouse != null)
            return Result<object>.Failure("Warehouse already exists", HttpStatusCode.Conflict);

        var warehouse = new Warehouse
        {
            Name = request.AddWarehouseDTO.Name,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedById = existingUser.Id
        };

        await warehouseRepository.AddAsync(warehouse, cancellationToken);

        return Result<object>.Success(warehouse.Id, "Warehouse created successfully");
    }
}