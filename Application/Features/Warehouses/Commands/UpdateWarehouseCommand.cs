using System.Net;
using Application.DTO.Warehouse;
using Application.Interfaces;
using Application.Results;
using MediatR;

namespace Application.Features.Warehouses.Commands;

/// <summary>Command to update an existing warehouse.</summary>
/// <param name="UserId">The ID of the authenticated user performing the action.</param>
/// <param name="Id">The ID of the warehouse to update.</param>
/// <param name="UpdateWarehouseDTO">The updated warehouse data.</param>
public record UpdateWarehouseCommand(int? UserId, int Id, UpdateWarehouseDto UpdateWarehouseDTO)
    : IRequest<Result<object>>;

public class UpdateWarehouseCommandHandler(IWarehouseRepository warehouseRepository, IUserRepository userRepository)
    : IRequestHandler<UpdateWarehouseCommand, Result<object>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IWarehouseRepository _warehouseRepository = warehouseRepository;

    public async Task<Result<object>> Handle(UpdateWarehouseCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId == null) return Result<object>.Failure("User is not signed in", HttpStatusCode.Unauthorized);

        var existingUser = await _userRepository.GetByIdAsync(request.UserId.Value, cancellationToken);

        if (existingUser == null) return Result<object>.Failure("User not found", HttpStatusCode.NotFound);

        var existingWarehouse = await _warehouseRepository.GetByIdAsync(request.Id, cancellationToken);

        if (existingWarehouse == null) return Result<object>.Failure("Warehouse not found", HttpStatusCode.NotFound);

        if (!string.IsNullOrEmpty(request.UpdateWarehouseDTO.Name))
        {
            var existingName =
                await _warehouseRepository.AnyDuplicateAsync(request.Id, request.UpdateWarehouseDTO.Name,
                    cancellationToken);

            if (existingName) return Result<object>.Failure("Warehouse name already exists");

            existingWarehouse.Name = request.UpdateWarehouseDTO.Name;
        }

        existingWarehouse.UpdatedAt = DateTime.UtcNow;
        existingWarehouse.UpdatedById = existingUser.Id;

        await _warehouseRepository.UpdateAsync(existingWarehouse, cancellationToken);

        return Result<object>.Success(existingWarehouse.Id, "Warehouse updated successfully");
    }
}