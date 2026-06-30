using Application.DTO.Product;
using Application.Interfaces;
using Application.Results;
using MediatR;
using System.Net;

namespace Application.Features.Products.Commands
{
    /// <summary>Command to update an existing product.</summary>
    /// <param name="UserId">The ID of the authenticated user performing the action.</param>
    /// <param name="Id">The ID of the product to update.</param>
    /// <param name="UpdateProductDTO">The updated product data.</param>
    public record UpdateProductCommand(int? UserId, int Id, UpdateProductDTO UpdateProductDTO) : IRequest<Result<object>>;
    public class UpdateProductCommandHandler(IProductRepository productRepository, IUserRepository userRepository) : IRequestHandler<UpdateProductCommand, Result<object>>
    {
        private readonly IProductRepository _productRepository = productRepository;
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<Result<object>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
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

            var existingProduct = await _productRepository.GetByIdAsync(request.Id, cancellationToken);

            if (existingProduct == null)
            {
                return Result<object>.Failure("Product not found", System.Net.HttpStatusCode.NotFound);
            }

            if (!string.IsNullOrEmpty(request.UpdateProductDTO.ItemCode))
            {
                var existingName = await _productRepository.CheckDuplicateAsync(request.Id, request.UpdateProductDTO.ItemCode, cancellationToken);

                if (existingName)
                {
                    return Result<object>.Failure("Product name already exists", System.Net.HttpStatusCode.BadRequest);
                }

                existingProduct.ItemCode = request.UpdateProductDTO.ItemCode;
            }

            if (!string.IsNullOrEmpty(request.UpdateProductDTO.Description))
            {
                existingProduct.Description = request.UpdateProductDTO.Description;
            }

            if (request.UpdateProductDTO.ProductCategoryId.HasValue)
            {
                existingProduct.ProductCategoryId = (int)request.UpdateProductDTO.ProductCategoryId;
            }

            if (request.UpdateProductDTO.UomId.HasValue)
            {
                existingProduct.UomId = (int)request.UpdateProductDTO.UomId;
            }

            existingProduct.UpdatedAt = DateTime.UtcNow;
            existingProduct.UpdatedById = existingUser.Id;

            await _productRepository.UpdateAsync(existingProduct, cancellationToken);

            return Result<object>.Success(null, "Product updated successfully");
        }
    }
}
