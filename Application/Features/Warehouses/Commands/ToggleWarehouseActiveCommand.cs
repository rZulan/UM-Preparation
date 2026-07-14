using System.Net;
using Application.Interfaces;
using Application.Results;
using MediatR;

namespace Application.Features.Warehouses.Commands;

/// <summary>Command to activate or deactivate a warehouse.</summary>
/// <param name="UserId">The ID of the authenticated user performing the action.</param>
/// <param name="Id">The ID of the warehouse to toggle.</param>
/// <param name="IsActive">The desired active state.</param>
public record ToggleWarehouseActiveCommand(int? UserId, int Id, bool IsActive) : IRequest<Result<object>>;

public class ToggleWarehouseActiveCommandHandler(
    IWarehouseRepository warehouseRepository,
    IUserRepository userRepository) : IRequestHandler<ToggleWarehouseActiveCommand, Result<object>>
{
    public async Task<Result<object>> Handle(ToggleWarehouseActiveCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId == null) return Result<object>.Failure("User is not signed in", HttpStatusCode.Unauthorized);

        var existingUser = await userRepository.GetByIdAsync(request.UserId.Value, cancellationToken);

        if (existingUser == null) return Result<object>.Failure("User not found", HttpStatusCode.NotFound);

        var existingWarehouse = await warehouseRepository.GetByIdAsync(request.Id, cancellationToken);

        if (existingWarehouse == null) return Result<object>.Failure("Warehouse not found");

        if (request.IsActive && existingWarehouse.IsActive)
            return Result<object>.Failure("Warehouse is already active");

        if (!request.IsActive && !existingWarehouse.IsActive)
            return Result<object>.Failure("Warehouse is already archived");

        var anyUsersTagged =
            await userRepository.AnyUsersWarehouseTaggedAsync(existingWarehouse.Id, cancellationToken);

        if (!request.IsActive && anyUsersTagged)
            return Result<object>.Failure(
                "Cannot archive this warehouse while it is assigned to users. Remove the user assignments first");

        existingWarehouse.IsActive = request.IsActive;
        existingWarehouse.UpdatedAt = DateTime.UtcNow;
        existingWarehouse.UpdatedById = existingUser.Id;

        await warehouseRepository.UpdateAsync(existingWarehouse, cancellationToken);

        var status = existingWarehouse.IsActive ? "restored" : "archived";

        return Result<object>.Success(existingWarehouse.Id, $"Warehouse {status} successfully");
    }
}