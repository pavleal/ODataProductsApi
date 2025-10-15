using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using ODataProductsApi.Dtos;
using ODataProductsApi.Mappings;
using ODataProductsApi.Repositories;

namespace ODataProductsApi.Controllers;

public class ProductsController : ODataController
{
    private readonly IProductRepository _repo;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IProductRepository repo, ILogger<ProductsController> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    // GET /odata/Products?$filter=Price gt 100&$orderby=Name&$select=Id,Name,Price&$top=5&$skip=0&$expand=Category
    public async Task<IActionResult> Get(ODataQueryOptions<ProductDto> options, CancellationToken ct)
    {
        _logger.LogInformation("Fetching ProductDto list (async) with OData options.");
        ct.ThrowIfCancellationRequested();

        var baseQueryDto = _repo.Query(asNoTracking: true).ToProductDto();

        var settings = new ODataQuerySettings
        {
            HandleNullPropagation = HandleNullPropagationOption.False,
            PageSize = 5
        };

        var applied = options.ApplyTo(baseQueryDto, settings);

        if (options.SelectExpand is not null)
            return Ok(applied);

        var typed = (IQueryable<ProductDto>)applied;

        long? total = null;
        if (options.Count?.Value == true)
        {
            var filteredForCount = options.Filter != null
                ? (IQueryable<ProductDto>)options.Filter.ApplyTo(baseQueryDto, settings)
                : baseQueryDto;

            total = await filteredForCount.LongCountAsync(ct);
        }

        var items = await typed.ToListAsync(ct);
        return Ok(new PageResult<ProductDto>(items, nextPageLink: null, count: total));
    }
    
    // POST /odata/Products
    public async Task<IActionResult> Post([FromBody] ProductDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var entity = dto.ToEntity();
        var created = await _repo.AddAsync(entity, ct);

        var createdDto = await _repo.Query()
            .Where(p => p.Id == created.Id)
            .ToProductDto()
            .FirstAsync(ct);

        return Created(createdDto);
    }
    
    // PUT /odata/Products(1)
    public async Task<IActionResult> Put([FromRoute] int key, [FromBody] ProductDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (key != dto.Id)
            return BadRequest("Mismatched key. Provide key in route and in body.");

        var existing = await _repo.GetByIdAsync(key, ct);
        if (existing is null)
            return NotFound();

        dto.MapOntoEntity(existing);

        var updated = await _repo.UpdateAsync(existing, ct);
        if (updated is null)
            return NotFound();

        var updatedDto = await _repo.Query()
            .Where(p => p.Id == key)
            .ToProductDto()
            .FirstAsync(ct);

        _logger.LogInformation("Product {Id} updated successfully.", key);
        return Updated(updatedDto);
    }
    
    public async Task<IActionResult> Delete([FromRoute] int key, CancellationToken ct)
    {
        var success = await _repo.DeleteAsync(key, ct);
        if (!success)
            return NotFound();

        _logger.LogInformation("Product {Id} deleted successfully.", key);
        return NoContent();
    }
}