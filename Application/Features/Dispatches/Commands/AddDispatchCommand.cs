using Application.DTO.Dispatch;
using Application.Interfaces;
using Application.Results;
using Domain.Entities;
using MediatR;
using System.Net;

namespace Application.Features.Dispatches.Commands
{
    /// <summary>Command to create a new dispatch.</summary>
    /// <param name="UserId">The ID of the authenticated user performing the action.</param>
    /// <param name="AddDispatchDTO">The dispatch data to be created.</param>
    public record AddDispatchCommand(int? UserId, AddDispatchDTO AddDispatchDTO) : IRequest<Result<object>>;
    public class AddDispatchCommandHandler(IDispatchRepository dispatchRepository, IUserRepository userRepository) : IRequestHandler<AddDispatchCommand, Result<object>>
    {
        private readonly IDispatchRepository _dispatchRepository = dispatchRepository;
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<Result<object>> Handle(AddDispatchCommand request, CancellationToken cancellationToken)
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

            var existingInBatchNumber = await _dispatchRepository.ExistsInSameBatchNumber(request.AddDispatchDTO.BatchNumber, cancellationToken);

            if (existingInBatchNumber)
            {
                return Result<object>.Failure("Dispatch Form with the same Batch Number already exists", HttpStatusCode.Conflict);
            }

            var dispatch = new Dispatch
            {
                BatchNumber = request.AddDispatchDTO.BatchNumber,
                ProductId = request.AddDispatchDTO.ProductId,
                FarmId = request.AddDispatchDTO.FarmId,
                QuantityOut = request.AddDispatchDTO.QuantityOut,
                QuantityReturn = request.AddDispatchDTO.QuantityReturn,
                HarvestDate = request.AddDispatchDTO.HarvestDate,
                CreatedAt = DateTime.UtcNow,
                CreatedById = existingUser.Id
            };

            await _dispatchRepository.AddAsync(dispatch, cancellationToken);

            return Result<object>.Success(dispatch.Id, "Dispatch created successfully", HttpStatusCode.Created);
        }
    }
}
