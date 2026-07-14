using System.Net;
using Application.DTO.Product;
using Application.Interfaces;
using Application.Results;
using Domain.Entities.Masterlist;
using MediatR;

namespace Application.Features.Products.Commands;

/// <summary>Command to create a new product.</summary>
/// <param name="UserId">The ID of the authenticated user performing the action.</param>
/// <param name="AddProductDTO">The product data to be created.</param>
public record AddProductCommand(int? UserId, AddProductDto AddProductDTO) : IRequest<Result<object>>;

public class AddProductCommandHandler(IProductRepository productRepository, IUserRepository userRepository)
    : IRequestHandler<AddProductCommand, Result<object>>
{
    public async Task<Result<object>> Handle(AddProductCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId == null) return Result<object>.Failure("User is not signed in", HttpStatusCode.Unauthorized);

        var existingUser = await userRepository.GetByIdAsync(request.UserId.Value, cancellationToken);

        if (existingUser == null) return Result<object>.Failure("User not found", HttpStatusCode.NotFound);

        var existingProduct =
            await productRepository.GetByItemCodeAsync(request.AddProductDTO.ItemCode, cancellationToken);

        if (existingProduct != null) return Result<object>.Failure("Product already exists", HttpStatusCode.Conflict);

        var product = new Product
        {
            ItemCode = request.AddProductDTO.ItemCode,
            Description = request.AddProductDTO.Description,
            UomId = request.AddProductDTO.UomId,
            CreatedAt = DateTime.UtcNow,
            CreatedById = existingUser.Id
        };

        await productRepository.AddAsync(product, cancellationToken);

        return Result<object>.Success(product.Id, "Product created successfully", HttpStatusCode.Created);
    }
}