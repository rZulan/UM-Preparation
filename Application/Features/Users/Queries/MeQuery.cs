using Application.DTO.User;
using Application.Interfaces;
using Application.Results;
using MediatR;
using System.Net;

namespace Application.Features.Users.Queries
{
    /// <summary>Query to retrieve the current user's information.</summary>
    public record MeQuery(int? UserId) : IRequest<Result<MeResultDto>>;
    public class MeQueryHandler(IUserRepository userRepository) : IRequestHandler<MeQuery, Result<MeResultDto>>
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<Result<MeResultDto>> Handle(MeQuery request, CancellationToken cancellationToken)
        {
            if (request.UserId == null)
            {
                return Result<MeResultDto>.Failure("User is not signed in", HttpStatusCode.Unauthorized);
            }

            var existingUser = await _userRepository.GetByIdAsync(request.UserId.Value, cancellationToken);

            if (existingUser == null)
            {
                return Result<MeResultDto>.Failure("User not found", HttpStatusCode.NotFound);
            }

            var result = new MeResultDto
            {
                Id = existingUser.Id,
                Username = existingUser.Username,
                FirstName = existingUser.FirstName,
                MiddleName = existingUser.MiddleName ?? "N/A",
                LastName = existingUser.LastName,
                Suffix = existingUser.Suffix ?? "N/A",
                IDPrefix = existingUser.IDPrefix,
                IDNumber = existingUser.IDNumber,
                Role = existingUser.UserRoles.FirstOrDefault()?.Role?.Name ?? "N/A",
                Warehouse = existingUser.Warehouse?.Name ?? "N/A",
                Permissions = existingUser.UserRoles
                    .SelectMany(ur => ur.Role!.RolePermissions)
                    .Select(rp => rp.Permission!.Name)
                    .Distinct()
                    .ToList()
            };

            return Result<MeResultDto>.Success(result);
        }
    }
}
