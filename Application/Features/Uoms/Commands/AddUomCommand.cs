using Application.DTO.Uom;
using Application.Interfaces;
using Application.Results;
using Domain.Entities.Masterlist;
using MediatR;
using System.Net;

namespace Application.Features.Uoms.Commands
{
    /// <summary>Command to create a new unit of measure.</summary>
    /// <param name="UserId">The ID of the authenticated user performing the action.</param>
    /// <param name="AddUomDTO">The unit of measure data to be created.</param>
    public record AddUomCommand(int? UserId, AddUomDTO AddUomDTO) : IRequest<Result<object>>;
    public class AddUomCommandHandler(IUomRepository uomRepository, IUserRepository userRepository) : IRequestHandler<AddUomCommand, Result<object>>
    {
        private readonly IUomRepository _uomRepository = uomRepository;
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<Result<object>> Handle(AddUomCommand request, CancellationToken cancellationToken)
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

            var existingUom = await _uomRepository.GetByNameAsync(request.AddUomDTO.Name, cancellationToken);

            if (existingUom != null)
            {
                return Result<object>.Failure("Uom already exists", HttpStatusCode.Conflict);
            }

            var uom = new Uom
            {
                Name = request.AddUomDTO.Name,
                ShortName = request.AddUomDTO.ShortName,
                IsInteger = request.AddUomDTO.IsInteger,
                CreatedAt = DateTime.UtcNow,
                CreatedById = existingUser.Id
            };

            await _uomRepository.AddAsync(uom, cancellationToken);

            return Result<object>.Success(uom.Id, "Uom created successfully", HttpStatusCode.Created);
        }
    }
}
