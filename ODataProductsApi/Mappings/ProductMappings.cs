using System.Linq.Expressions;
using ODataProductsApi.Dtos;
using ODataProductsApi.Models;

namespace ODataProductsApi.Mappings;

public static class ProductMappings
{
    private static readonly Expression<Func<Product, ProductDto>> Selector =
        p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            CategoryId = p.CategoryId,
            Category = p.Category == null ? null : new CategoryDto
            {
                Id = p.Category.Id,
                Name = p.Category.Name
            }
        };

    public static IQueryable<ProductDto> ToProductDto(this IQueryable<Product> query)
        => query.Select(Selector);
    
    public static Product ToEntity(this ProductDto dto)
        => new Product
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            CategoryId = dto.CategoryId
        };

    public static void MapOntoEntity(this ProductDto dto, Product entity)
    {
        entity.Name = dto.Name;
        entity.Description = dto.Description ?? entity.Description;
        entity.Price = dto.Price;
        entity.CategoryId = dto.CategoryId;
    }
}