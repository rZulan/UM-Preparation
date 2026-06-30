using Application.DTO.Product;
using Application.Interfaces;
using Application.Results;
using Domain.Entities.Masterlist;
using MediatR;
using System.Net;

namespace Application.Features.Products.Commands
{
    /// <summary>Command to create a new product.</summary>
    /// <param name="UserId">The ID of the authenticated user performing the action.</param>
    /// <param name="AddProductDTO">The product data to be created.</param>
    public record AddProductCommand(int? UserId, AddProductDTO AddProductDTO) : IRequest<Result<object>>;
    public class AddProductCommandHandler(IProductRepository productRepository, IUserRepository userRepository) : IRequestHandler<AddProductCommand, Result<object>>
    {
        private readonly IProductRepository _productRepository = productRepository;
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<Result<object>> Handle(AddProductCommand request, CancellationToken cancellationToken)
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

            var existingProduct = await _productRepository.GetByItemCodeAsync(request.AddProductDTO.ItemCode, cancellationToken);

            if (existingProduct != null)
            {
                return Result<object>.Failure("Product already exists", HttpStatusCode.Conflict);
            }

            var product = new Product
            {
                ItemCode = request.AddProductDTO.ItemCode,
                Description = request.AddProductDTO.Description,
                ProductCategoryId = request.AddProductDTO.ProductCategoryId,
                UomId = request.AddProductDTO.UomId,
                CreatedAt = DateTime.UtcNow,
                CreatedById = existingUser.Id
            };

            await _productRepository.AddAsync(product, cancellationToken);

            return Result<object>.Success(product.Id, "Product created successfully", HttpStatusCode.Created);
        }
    }
}
