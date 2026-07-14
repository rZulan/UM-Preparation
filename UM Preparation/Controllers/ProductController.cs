using Application.DTO.Misc;
using Application.DTO.Misc.Sorts;
using Application.DTO.Product;
using Application.Features.Products.Commands;
using Application.Features.Products.Queries;
using Application.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UM_Preparation.Extensions;

namespace UM_Preparation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] GenericFiltersDto genericFiltersDto,
            [FromQuery] Sort sort)
        {
            GetAllResult<List<GetProductDto>> result =
                await _mediator.Send(new GetProductsQuery(genericFiltersDto, sort));

            return StatusCode((int)result.StatusCode!.Value, result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            Result<GetProductDto> result = await _mediator.Send(new GetProductByIdQuery(id));

            return StatusCode((int)result.StatusCode!.Value, result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddProduct([FromBody] AddProductDto addProductDto)
        {
            int? userId = this.GetCurrentUserId();

            Result<object> result = await _mediator.Send(new AddProductCommand(userId, addProductDto));

            return StatusCode((int)result.StatusCode!.Value, result);
        }

        [HttpPatch("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDto updateProductDto)
        {
            int? userId = this.GetCurrentUserId();

            Result<object> result = await _mediator.Send(new UpdateProductCommand(userId, id, updateProductDto));

            return StatusCode((int)result.StatusCode!.Value, result);
        }

        [HttpPatch("{id:int}/archive")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ArchiveProduct(int id)
        {
            int? userId = this.GetCurrentUserId();

            Result<object> result = await _mediator.Send(new ToggleProductActiveCommand(userId, id, false));

            return StatusCode((int)result.StatusCode!.Value, result);
        }

        [HttpPatch("{id:int}/restore")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RestoreProduct(int id)
        {
            int? userId = this.GetCurrentUserId();

            Result<object> result = await _mediator.Send(new ToggleProductActiveCommand(userId, id, true));

            return StatusCode((int)result.StatusCode!.Value, result);
        }
    }
}