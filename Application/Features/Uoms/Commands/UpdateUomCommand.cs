using Application.DTO.Uom;
using Application.Interfaces;
using Application.Results;
using MediatR;
using System.Net;

namespace Application.Features.Uoms.Commands
{
    /// <summary>Command to update an existing unit of measure.</summary>
    /// <param name="UserId">The ID of the authenticated user performing the action.</param>
    /// <param name="Id">The ID of the unit of measure to update.</param>
    /// <param name="UpdateUomDTO">The updated unit of measure data.</param>
    public record UpdateUomCommand(int? UserId, int Id, UpdateUomDTO UpdateUomDTO) : IRequest<Result<object>>;
    public class UpdateUomCommandHandler(IUomRepository uomRepository, IUserRepository userRepository) : IRequestHandler<UpdateUomCommand, Result<object>>
    {
        private readonly IUomRepository _uomRepository = uomRepository;
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<Result<object>> Handle(UpdateUomCommand request, CancellationToken cancellationToken)
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

            var existingUom = await _uomRepository.GetByIdAsync(request.Id, cancellationToken);

            if (existingUom == null)
            {
                return Result<object>.Failure("Uom not found", System.Net.HttpStatusCode.NotFound);
            }

            if (!string.IsNullOrEmpty(request.UpdateUomDTO.Name))
            {
                var existingName = await _uomRepository.CheckDuplicateAsync(request.Id, request.UpdateUomDTO.Name, cancellationToken);

                if (existingName)
                {
                    return Result<object>.Failure("Uom name already exists", System.Net.HttpStatusCode.BadRequest);
                }

                existingUom.Name = request.UpdateUomDTO.Name;
            }

            if (!string.IsNullOrEmpty(request.UpdateUomDTO.ShortName))
            {
                existingUom.ShortName = request.UpdateUomDTO.ShortName;
            }

            if (request.UpdateUomDTO.IsInteger.HasValue)
            {
                existingUom.IsInteger = (bool)request.UpdateUomDTO.IsInteger;
            }

            existingUom.UpdatedAt = DateTime.UtcNow;
            existingUom.UpdatedById = existingUser.Id;

            await _uomRepository.UpdateAsync(existingUom, cancellationToken);

            return Result<object>.Success(null, "Uom updated successfully");
        }
    }
}
