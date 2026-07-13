using Application.DTO.Uom;
using Application.Interfaces;
using Application.Results;
using MediatR;
using System.Net;

namespace Application.Features.Uoms.Queries
{
    /// <summary>Query to retrieve a single unit of measure by its ID.</summary>
    /// <param name="Id">The unique identifier of the unit of measure to retrieve.</param>
    public record GetUomByIdQuery(int Id) : IRequest<Result<GetUomDto>>;
    public class GetUomByIdQueryHandler(IUomRepository uomRepository) : IRequestHandler<GetUomByIdQuery, Result<GetUomDto>>
    {
        private readonly IUomRepository _uomRepository = uomRepository;

        public async Task<Result<GetUomDto>> Handle(GetUomByIdQuery request, CancellationToken cancellationToken)
        {
            var uom = await _uomRepository.GetByIdAsync(request.Id, cancellationToken);

            if (uom == null)
            {
                return Result<GetUomDto>.Failure("Uom not found", HttpStatusCode.NotFound);
            }

            var result = new GetUomDto
            {
                Id = uom.Id,
                IsActive = uom.IsActive,
                Name = uom.Name,
                ShortName = uom.ShortName,
                IsInteger = uom.IsInteger,
            };

            return Result<GetUomDto>.Success(result);
        }
    }
}