using System.Net;
using Application.DTO.Role;
using Application.Interfaces;
using Application.Results;
using Domain.Entities.Junction;
using MediatR;

namespace Application.Features.Roles.Commands;

/// <summary>Command to update an existing role and its assigned permissions.</summary>
/// <param name="UserId">The ID of the authenticated user performing the action.</param>
/// <param name="Id">The ID of the role to update.</param>
/// <param name="UpdateRoleDTO">The updated role data.</param>
public record UpdateRoleCommand(int? UserId, int Id, UpdateRoleDto UpdateRoleDTO) : IRequest<Result<object>>;

public class UpdateRoleCommandHandler(
    IRoleRepository roleRepository,
    IPermissionRepository permissionRepository,
    IUserRepository userRepository) : IRequestHandler<UpdateRoleCommand, Result<object>>
{
    public async Task<Result<object>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId == null) return Result<object>.Failure("User is not signed in", HttpStatusCode.Unauthorized);

        var existingUser = await userRepository.GetByIdAsync(request.UserId.Value, cancellationToken);

        if (existingUser == null) return Result<object>.Failure("User not found", HttpStatusCode.NotFound);

        var existingRole = await roleRepository.GetByIdAsync(request.Id, cancellationToken);

        if (existingRole == null) return Result<object>.Failure("Role not found", HttpStatusCode.NotFound);

        if (!string.IsNullOrEmpty(request.UpdateRoleDTO.Name))
        {
            var existingName =
                await roleRepository.AnyDuplicateAsync(request.Id, request.UpdateRoleDTO.Name, cancellationToken);

            if (existingName) return Result<object>.Failure("Role name already exists");

            existingRole.Name = request.UpdateRoleDTO.Name;
        }

        if (request.UpdateRoleDTO.Permissions != null)
        {
            var permissions = request.UpdateRoleDTO.Permissions.Count > 0
                ? await permissionRepository.GetByIdsAsync(request.UpdateRoleDTO.Permissions, cancellationToken)
                : [];

            existingRole.RolePermissions.Clear();

            foreach (var permission in permissions)
                existingRole.RolePermissions.Add(new RolePermissions
                {
                    PermissionId = permission.Id,
                    RoleId = existingRole.Id
                });
        }

        existingRole.UpdatedAt = DateTime.UtcNow;
        existingRole.UpdatedById = existingUser.Id;

        await roleRepository.UpdateAsync(existingRole, cancellationToken);

        return Result<object>.Success(null, "Role updated successfully");
    }
}