using Application.DTO.Permission;
using Application.DTO.Role;
using Application.Interfaces;
using Application.Results;
using MediatR;
using System.Net;

namespace Application.Features.Roles.Queries
{
    /// <summary>Query to retrieve a single role by its ID, including its assigned permissions.</summary>
    /// <param name="Id">The unique identifier of the role to retrieve.</param>
    public record GetRoleByIdQuery(int Id) : IRequest<Result<GetRoleDTO>>;
    public class GetRoleByIdQueryHandler(IRoleRepository roleRepository) : IRequestHandler<GetRoleByIdQuery, Result<GetRoleDTO>>
    {
        private readonly IRoleRepository _roleRepository = roleRepository;

        public async Task<Result<GetRoleDTO>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            var role = await _roleRepository.GetByIdAsync(request.Id, cancellationToken);

            if (role == null)
            {
                return Result<GetRoleDTO>.Failure("Role not found", HttpStatusCode.NotFound);
            }

            var result = new GetRoleDTO
            {
                Id = role.Id,
                IsActive = role.IsActive,
                Name = role.Name,
                Permissions = role.RolePermissions
                    .Where(rp => rp.Permission != null)
                    .Select(rp => new GetPermissionDTO
                    {
                        Id = rp.Permission!.Id,
                        IsActive = rp.Permission!.IsActive,
                        Name = rp.Permission.Name
                    }).ToList() ?? []
            };

            return Result<GetRoleDTO>.Success(result);
        }
    }
}