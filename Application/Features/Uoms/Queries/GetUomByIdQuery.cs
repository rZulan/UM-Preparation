using Application.DTO.Uom;
using Application.Interfaces;
using Application.Results;
using MediatR;
using System.Net;

namespace Application.Features.Uoms.Queries
{
    /// <summary>Query to retrieve a single unit of measure by its ID.</summary>
    /// <param name="Id">The unique identifier of the unit of measure to retrieve.</param>
    public record GetUomByIdQuery(int Id) : IRequest<Result<GetUomDTO>>;
    public class GetUomByIdQueryHandler(IUomRepository uomRepository) : IRequestHandler<GetUomByIdQuery, Result<GetUomDTO>>
    {
        private readonly IUomRepository _uomRepository = uomRepository;

        public async Task<Result<GetUomDTO>> Handle(GetUomByIdQuery request, CancellationToken cancellationToken)
        {
            var uom = await _uomRepository.GetByIdAsync(request.Id, cancellationToken);

            if (uom == null)
            {
                return Result<GetUomDTO>.Failure("Uom not found", HttpStatusCode.NotFound);
            }

            var result = new GetUomDTO
            {
                Id = uom.Id,
                IsActive = uom.IsActive,
                Name = uom.Name,
                ShortName = uom.ShortName,
                IsInteger = uom.IsInteger,
            };

            return Result<GetUomDTO>.Success(result);
        }
    }
}