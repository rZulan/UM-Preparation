using Application.DTO.Product;
using Application.Interfaces;
using Application.Results;
using MediatR;
using System.Net;

namespace Application.Features.Products.Queries
{
    /// <summary>Query to retrieve a single product by its ID.</summary>
    /// <param name="Id">The unique identifier of the product to retrieve.</param>
    public record GetProductByIdQuery(int Id) : IRequest<Result<GetProductDto>>;
    public class GetProductByIdQueryHandler(IProductRepository productRepository) : IRequestHandler<GetProductByIdQuery, Result<GetProductDto>>
    {
        private readonly IProductRepository _productRepository = productRepository;

        public async Task<Result<GetProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);

            if (product == null)
            {
                return Result<GetProductDto>.Failure("Product not found", HttpStatusCode.NotFound);
            }

            var result = new GetProductDto
            {
                Id = product.Id,
                IsActive = product.IsActive,
                ItemCode = product.ItemCode,
                Description = product.Description,
                Uom = product.Uom.ShortName + " - " + product.Uom.Name
            };

            return Result<GetProductDto>.Success(result);
        }
    }
}