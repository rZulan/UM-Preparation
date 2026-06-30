using Application.DTO.MiscellaneousReceipt;
using Application.Interfaces;
using Application.Results;
using MediatR;
using System.Net;

namespace Application.Features.MiscellaneousReceipts.Queries
{
    /// <summary>Query to retrieve a single miscellaneous receipt by its ID.</summary>
    /// <param name="Id">The unique identifier of the miscellaneous receipt to retrieve.</param>
    public record GetMiscellaneousReceiptByIdQuery(int Id) : IRequest<Result<GetMiscellaneousReceiptDTO>>;
    public class GetMiscellaneousReceiptByIdQueryHandler(IMiscellaneousReceiptRepository miscellaneousReceiptRepository) : IRequestHandler<GetMiscellaneousReceiptByIdQuery, Result<GetMiscellaneousReceiptDTO>>
    {
        private readonly IMiscellaneousReceiptRepository _miscellaneousReceiptRepository = miscellaneousReceiptRepository;

        public async Task<Result<GetMiscellaneousReceiptDTO>> Handle(GetMiscellaneousReceiptByIdQuery request, CancellationToken cancellationToken)
        {
            var miscellaneousReceipt = await _miscellaneousReceiptRepository.GetByIdAsync(request.Id, cancellationToken);

            if (miscellaneousReceipt == null)
            {
                return Result<GetMiscellaneousReceiptDTO>.Failure("Miscellaneous receipt not found", HttpStatusCode.NotFound);
            }

            var result = new GetMiscellaneousReceiptDTO
            {
                Id = miscellaneousReceipt.Id,
                IsActive = miscellaneousReceipt.IsActive,
                ItemCode = miscellaneousReceipt.Product.ItemCode,
                Description = miscellaneousReceipt.Product.Description,
                Uom = miscellaneousReceipt.Product.Uom.ShortName,
                Quantity = miscellaneousReceipt.Quantity,
            };

            return Result<GetMiscellaneousReceiptDTO>.Success(result);
        }
    }
}