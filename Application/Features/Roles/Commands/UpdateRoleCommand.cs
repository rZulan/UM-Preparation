using Application.DTO.Role;
using Application.Interfaces;
using Application.Results;
using Domain.Entities.Junction;
using MediatR;
using System.Net;

namespace Application.Features.Roles.Commands
{
    /// <summary>Command to update an existing role and its assigned permissions.</summary>
    /// <param name="UserId">The ID of the authenticated user performing the action.</param>
    /// <param name="Id">The ID of the role to update.</param>
    /// <param name="UpdateRoleDTO">The updated role data.</param>
    public record UpdateRoleCommand(int? UserId, int Id, UpdateRoleDTO UpdateRoleDTO) : IRequest<Result<object>>;
    public class UpdateRoleCommandHandler(IRoleRepository roleRepository, IUserRepository userRepository) : IRequestHandler<UpdateRoleCommand, Result<object>>
    {
        private readonly IRoleRepository _roleRepository = roleRepository;
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<Result<object>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
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

            var existingRole = await _roleRepository.GetByIdAsync(request.Id, cancellationToken);

            if (existingRole == null)
            {
                return Result<object>.Failure("Role not found", System.Net.HttpStatusCode.NotFound);
            }

            if (!string.IsNullOrEmpty(request.UpdateRoleDTO.Name))
            {
                var existingName = await _roleRepository.CheckDuplicateAsync(request.Id, request.UpdateRoleDTO.Name, cancellationToken);

                if (existingName)
                {
                    return Result<object>.Failure("Role name already exists", System.Net.HttpStatusCode.BadRequest);
                }

                existingRole.Name = request.UpdateRoleDTO.Name;
            }

            if (request.UpdateRoleDTO.Permissions != null)
            {
                var permissions = request.UpdateRoleDTO.Permissions.Count > 0
                    ? await _roleRepository.GetByIdsAsync(request.UpdateRoleDTO.Permissions, cancellationToken)
                    : [];

                existingRole.RolePermissions.Clear();

                foreach (var permission in permissions)
                {
                    existingRole.RolePermissions.Add(new RolePermissions
                    {
                        PermissionId = permission.Id,
                        RoleId = existingRole.Id
                    });
                }
            }

            existingUser.UpdatedAt = DateTime.UtcNow;
            existingUser.UpdatedById = existingUser.Id;

            await _roleRepository.UpdateAsync(existingRole, cancellationToken);

            return Result<object>.Success(null, "Role updated successfully");
        }
    }
}
