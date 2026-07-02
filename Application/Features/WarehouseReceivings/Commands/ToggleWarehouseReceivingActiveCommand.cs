using Application.Interfaces;
using Application.Results;
using MediatR;
using System.Net;

namespace Application.Features.WarehouseReceivings.Commands
{
    /// <summary>Command to activate or deactivate a warehouse entry.</summary>
    /// <param name="UserId">The ID of the authenticated user performing the action.</param>
    /// <param name="Id">The ID of the warehouse entry to toggle.</param>
    /// <param name="IsActive">The desired active state.</param>
    public record ToggleWarehouseReceivingActiveCommand(int? UserId, int Id, bool IsActive) : IRequest<Result<object>>;
    public class ToggleWarehouseReceivingActiveCommandHandler(IWarehouseReceivingRepository warehouseReceivingRepository, IUserRepository userRepository) : IRequestHandler<ToggleWarehouseReceivingActiveCommand, Result<object>>
    {
        private readonly IWarehouseReceivingRepository _warehouseReceivingRepository = warehouseReceivingRepository;
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<Result<object>> Handle(ToggleWarehouseReceivingActiveCommand request, CancellationToken cancellationToken)
        {
            if (request.UserId == null)
            {
                return Result<object>.Failure("User is not signed in", HttpStatusCode.Unauthorized);
            }

            var existingUser = await _userRepository.GetByIdAsync(request.UserId.Value, cancellationToken);

            if (existingUser == null)
            {
                return Result<object>.Failure("User not found", HttpStatusCode.NotFound);
            }

            var existingWarehouseReceiving = await _warehouseReceivingRepository.GetByIdAsync(request.Id, cancellationToken);

            if (existingWarehouseReceiving == null)
            {
                return Result<object>.Failure("Warehouse Receiving not found", HttpStatusCode.NotFound);
            }

            if (request.IsActive && existingWarehouseReceiving.IsActive)
            {
                return Result<object>.Failure("Warehouse Receiving is already active");
            }

            if (!request.IsActive && !existingWarehouseReceiving.IsActive)
            {
                return Result<object>.Failure("Warehouse Receiving is already archived");
            }

            existingWarehouseReceiving.IsActive = request.IsActive;
            existingWarehouseReceiving.UpdatedAt = DateTime.UtcNow;
            existingWarehouseReceiving.UpdatedById = request.UserId.Value;

            await _warehouseReceivingRepository.UpdateAsync(existingWarehouseReceiving, cancellationToken);

            var status = request.IsActive ? "restored" : "archived";

            return Result<object>.Success(existingWarehouseReceiving.Id, $"Warehouse Receiving {status} successfully");
        }
    }
}
