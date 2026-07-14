using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Application.DTO.Product;
using Application.Interfaces;
using Application.Results;
using MediatR;

namespace Application.Features.Products.Queries;

/// <summary>Query to retrieve a filtered, sorted, and paginated list of products.</summary>
/// <param name="GenericFiltersDTO">Search and pagination filters.</param>
/// <param name="Sort">Sort direction and field.</param>
public record GetProductsQuery(GenericFiltersDto GenericFiltersDTO, Sort Sort)
    : IRequest<GetAllResult<List<GetProductDto>>>;

public class GetProductsQueryHandler(IProductRepository productRepository)
    : IRequestHandler<GetProductsQuery, GetAllResult<List<GetProductDto>>>
{
    private readonly IProductRepository _productRepository = productRepository;

    public async Task<GetAllResult<List<GetProductDto>>> Handle(GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetAllAsync(request.GenericFiltersDTO, request.Sort, cancellationToken);

        var result = products.Select(x => new GetProductDto
        {
            Id = x.Id,
            IsActive = x.IsActive,
            ItemCode = x.ItemCode,
            Description = x.Description,
            Uom = x.Uom.ShortName + " - " + x.Uom.Name
        }).ToList();

        PaginationInfo? paginationInfo = null;

        if (request.GenericFiltersDTO.UsePagination)
            paginationInfo = new PaginationInfo
            {
                CurrentPage = request.GenericFiltersDTO.PageNumber,
                PageSize = request.GenericFiltersDTO.PageSize,
                TotalCount = await _productRepository.GetCountAsync(request.GenericFiltersDTO, cancellationToken)
            };

        var sortInfo = new SortInfo
        {
            SortColumns = ["id", "name"],
            CurrentSort = request.Sort != null
                ? new CurrentSort
                {
                    Column = request.Sort.SortBy,
                    Direction = request.Sort.SortDirection
                }
                : null
        };

        return GetAllResult<List<GetProductDto>>.Success(result, paginationInfo: paginationInfo, sortInfo: sortInfo);
    }
}