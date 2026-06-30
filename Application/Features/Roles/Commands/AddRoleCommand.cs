using Application.DTO.Role;
using Application.Interfaces;
using Application.Results;
using Domain.Entities.Masterlist;
using MediatR;
using System.Net;

namespace Application.Features.Roles.Commands
{
    /// <summary>Command to create a new role.</summary>
    /// <param name="UserId">The ID of the authenticated user performing the action.</param>
    /// <param name="AddRoleDTO">The role data to be created.</param>
    public record AddRoleCommand(int? UserId, AddRoleDTO AddRoleDTO) : IRequest<Result<object>>;
    public class AddRoleCommandHandler(IRoleRepository roleRepository, IUserRepository userRepository) : IRequestHandler<AddRoleCommand, Result<object>>
    {
        private readonly IRoleRepository _roleRepository = roleRepository;
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<Result<object>> Handle(AddRoleCommand request, CancellationToken cancellationToken)
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

            var existingRole = await _roleRepository.GetByNameAsync(request.AddRoleDTO.Name, cancellationToken);

            if (existingRole != null)
            {
                return Result<object>.Failure("Role already exists", HttpStatusCode.Conflict);
            }

            var role = new Role
            {
                Name = request.AddRoleDTO.Name,
                CreatedAt = DateTime.UtcNow,
                CreatedById = existingUser.Id
            };

            await _roleRepository.AddAsync(role, cancellationToken);

            return Result<object>.Success(role.Id, "Role created successfully", HttpStatusCode.Created);
        }
    }
}
