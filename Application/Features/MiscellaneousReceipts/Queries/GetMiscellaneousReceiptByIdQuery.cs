using System.Net;
using Application.DTO.MiscellaneousReceipt;
using Application.Interfaces;
using Application.Results;
using MediatR;

namespace Application.Features.MiscellaneousReceipts.Queries;

/// <summary>Query to retrieve a single miscellaneous receipt by its ID.</summary>
/// <param name="Id">The unique identifier of the miscellaneous receipt to retrieve.</param>
public record GetMiscellaneousReceiptByIdQuery(int Id) : IRequest<Result<GetMiscellaneousReceiptDto>>;

public class GetMiscellaneousReceiptByIdQueryHandler(IMiscellaneousReceiptRepository miscellaneousReceiptRepository)
    : IRequestHandler<GetMiscellaneousReceiptByIdQuery, Result<GetMiscellaneousReceiptDto>>
{
    public async Task<Result<GetMiscellaneousReceiptDto>> Handle(GetMiscellaneousReceiptByIdQuery request,
        CancellationToken cancellationToken)
    {
        var miscellaneousReceipt = await miscellaneousReceiptRepository.GetByIdAsync(request.Id, cancellationToken);

        if (miscellaneousReceipt == null)
            return Result<GetMiscellaneousReceiptDto>.Failure("Miscellaneous receipt not found",
                HttpStatusCode.NotFound);

        var result = new GetMiscellaneousReceiptDto
        {
            Id = miscellaneousReceipt.Id,
            IsActive = miscellaneousReceipt.IsActive,
            WarehouseId = miscellaneousReceipt.WarehouseId,
            Warehouse = miscellaneousReceipt.Warehouse.Name,
            ProductId = miscellaneousReceipt.ProductId,
            ItemCode = miscellaneousReceipt.Product.ItemCode,
            Description = miscellaneousReceipt.Product.Description,
            Uom = miscellaneousReceipt.Product.Uom.ShortName,
            Quantity = miscellaneousReceipt.Quantity,
            Reason = miscellaneousReceipt.Reason
        };

        return Result<GetMiscellaneousReceiptDto>.Success(result);
    }
}