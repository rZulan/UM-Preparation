using Application.DTO.Permission;
using Application.Interfaces;
using Application.Results;
using Domain.Entities.Masterlist;
using MediatR;
using System.Net;

namespace Application.Features.Permissions.Commands
{
    /// <summary>Command to create a new permission.</summary>
    /// <param name="UserId">The ID of the authenticated user performing the action.</param>
    /// <param name="AddPermissionDTO">The permission data to be created.</param>
    public record AddPermissionCommand(int? UserId, AddPermissionDTO AddPermissionDTO) : IRequest<Result<object>>;
    public class AddPermissionCommandHandler(IPermissionRepository permissionRepository, IUserRepository userRepository) : IRequestHandler<AddPermissionCommand, Result<object>>
    {
        private readonly IPermissionRepository _permissionRepository = permissionRepository;
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<Result<object>> Handle(AddPermissionCommand request, CancellationToken cancellationToken)
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

            var existingPermission = await _permissionRepository.GetByNameAsync(request.AddPermissionDTO.Name, cancellationToken);

            if (existingPermission != null)
            {
                return Result<object>.Failure("Permission already exists", HttpStatusCode.Conflict);
            }

            var permission = new Permission
            {
                CreatedAt = DateTime.UtcNow,
                CreatedById = existingUser.Id,
                Name = request.AddPermissionDTO.Name,
            };

            await _permissionRepository.AddAsync(permission, cancellationToken);

            return Result<object>.Success(permission.Id, "Permission created successfully", HttpStatusCode.Created);
        }
    }
}
