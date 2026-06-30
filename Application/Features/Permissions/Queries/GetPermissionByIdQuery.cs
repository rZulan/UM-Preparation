using Application.DTO.Permission;
using Application.Interfaces;
using Application.Results;
using MediatR;
using System.Net;

namespace Application.Features.Permissions.Queries
{
    /// <summary>Query to retrieve a single permission by its ID.</summary>
    /// <param name="Id">The unique identifier of the permission to retrieve.</param>
    public record GetPermissionByIdQuery(int Id) : IRequest<Result<GetPermissionDTO>>;
    public class GetPermissionByIdQueryHandler(IPermissionRepository permissionRepository) : IRequestHandler<GetPermissionByIdQuery, Result<GetPermissionDTO>>
    {
        private readonly IPermissionRepository _permissionRepository = permissionRepository;

        public async Task<Result<GetPermissionDTO>> Handle(GetPermissionByIdQuery request, CancellationToken cancellationToken)
        {
            var permission = await _permissionRepository.GetByIdAsync(request.Id, cancellationToken);

            if (permission == null)
            {
                return Result<GetPermissionDTO>.Failure("Permission not found", HttpStatusCode.NotFound);
            }

            var result = new GetPermissionDTO
            {
                Id = permission.Id,
                IsActive = permission.IsActive,
                Name = permission.Name
            };

            return Result<GetPermissionDTO>.Success(result);
        }
    }
}