using Application.Interfaces;
using Application.Results;
using MediatR;
using System.Net;

namespace Application.Features.Uoms.Commands
{
    /// <summary>Command to activate or deactivate a unit of measure.</summary>
    /// <param name="UserId">The ID of the authenticated user performing the action.</param>
    /// <param name="Id">The ID of the unit of measure to toggle.</param>
    /// <param name="IsActive">The desired active state.</param>
    public record ToggleUomActiveCommand(int? UserId, int Id, bool IsActive) : IRequest<Result<object>>;
    public class ToggleUomActiveCommandHandler(IUomRepository uomRepository, IUserRepository userRepository) : IRequestHandler<ToggleUomActiveCommand, Result<object>>
    {
        private readonly IUomRepository _uomRepository = uomRepository;
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<Result<object>> Handle(ToggleUomActiveCommand request, CancellationToken cancellationToken)
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

            var existingUom = await _uomRepository.GetByIdAsync(request.Id, cancellationToken);

            if (existingUom == null)
            {
                return Result<object>.Failure("Uom not found");
            }

            if (request.IsActive && existingUom.IsActive)
            {
                return Result<object>.Failure("Uom is already active");
            }

            if (!request.IsActive && !existingUom.IsActive)
            {
                return Result<object>.Failure("Uom is already archived");
            }

            existingUom.IsActive = request.IsActive;
            existingUom.UpdatedAt = DateTime.UtcNow;
            existingUom.UpdatedById = existingUser.Id;

            await _uomRepository.UpdateAsync(existingUom, cancellationToken);

            var status = existingUom.IsActive ? "restored" : "archived";

            return Result<object>.Success(existingUom.Id, $"Uom {status} successfully");
        }
    }
}
