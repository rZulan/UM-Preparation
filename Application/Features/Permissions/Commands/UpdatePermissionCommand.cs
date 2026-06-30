using Application.DTO.Permission;
using Application.Interfaces;
using Application.Results;
using MediatR;
using System.Net;

namespace Application.Features.Permissions.Commands
{
    /// <summary>Command to update an existing permission.</summary>
    /// <param name="UserId">The ID of the authenticated user performing the action.</param>
    /// <param name="Id">The ID of the permission to update.</param>
    /// <param name="UpdatePermissionDTO">The updated permission data.</param>
    public record UpdatePermissionCommand(int? UserId, int Id, UpdatePermissionDTO UpdatePermissionDTO) : IRequest<Result<object>>;
    public class UpdatePermissionCommandHandler(IPermissionRepository permissionRepository, IUserRepository userRepository) : IRequestHandler<UpdatePermissionCommand, Result<object>>
    {
        private readonly IPermissionRepository _permissionRepository = permissionRepository;
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<Result<object>> Handle(UpdatePermissionCommand request, CancellationToken cancellationToken)
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

            var existingPermission = await _permissionRepository.GetByIdAsync(request.Id, cancellationToken);

            if (existingPermission == null)
            {
                return Result<object>.Failure("Permission not found", System.Net.HttpStatusCode.NotFound);
            }

            if (!string.IsNullOrEmpty(request.UpdatePermissionDTO.Name))
            {
                var existingName = await _permissionRepository.CheckDuplicateAsync(request.Id, request.UpdatePermissionDTO.Name, cancellationToken);

                if (existingName)
                {
                    return Result<object>.Failure("Permission name already exists", System.Net.HttpStatusCode.BadRequest);
                }

                existingPermission.Name = request.UpdatePermissionDTO.Name;
            }

            existingPermission.UpdatedAt = DateTime.UtcNow;
            existingPermission.UpdatedById = existingUser.Id;

            await _permissionRepository.UpdateAsync(existingPermission, cancellationToken);

            return Result<object>.Success(null, "Permission updated successfully");
        }
    }
}
