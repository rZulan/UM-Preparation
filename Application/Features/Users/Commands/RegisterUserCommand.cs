using Application.DTO.User;
using Application.Interfaces;
using Application.Results;
using Domain.Entities;
using Domain.Entities.Junction;
using MediatR;
using System.Net;

namespace Application.Features.Users.Commands
{
    /// <summary>Command to register a new user account.</summary>
    /// <param name="RegisterDTO">The registration data (employee info, credentials, and role).</param>
    public record RegisterUserCommand(RegisterUserDTO RegisterDTO) : IRequest<Result<object>>;
    public class RegisterUserCommandHandler(IUserRepository userRepository, IPasswordHasherService passwordHasher, IRoleRepository roleRepository) : IRequestHandler<RegisterUserCommand, Result<object>>
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IPasswordHasherService _paswordHasher = passwordHasher;
        private readonly IRoleRepository _roleRepository = roleRepository;

        public async Task<Result<object>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _userRepository.GetByUsernameAsync(request.RegisterDTO.Username, cancellationToken);

            if (existingUser != null)
            {
                return Result<object>.Failure("Username already exists", HttpStatusCode.Conflict);
            }

            var hashPassword = _paswordHasher.Hash(request.RegisterDTO.Password);

            var existingRole = await _roleRepository.GetByIdAsync(request.RegisterDTO.RoleId, cancellationToken);

            if (existingRole == null)
            {
                return Result<object>.Failure("Role not found", HttpStatusCode.NotFound);
            }

            var user = new User
            {
                CreatedAt = DateTime.UtcNow,
                PasswordHash = hashPassword,
                Username = request.RegisterDTO.Username.ToLower(),
                FirstName = request.RegisterDTO.FirstName,
                MiddleName = request.RegisterDTO.MiddleName,
                LastName = request.RegisterDTO.LastName,
                Suffix = request.RegisterDTO.Suffix,
                IDPrefix = request.RegisterDTO.IDPrefix,
                IDNumber = request.RegisterDTO.IDNumber,
            };

            user.UserRoles.Add(new UserRoles
            {
                RoleId = existingRole.Id,
                UserId = user.Id
            });

            await _userRepository.AddAsync(user, cancellationToken);

            return Result<object>.Success(user.Id, "User created successfully", HttpStatusCode.Created);
        }
    }
}
