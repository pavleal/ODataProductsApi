using ODataProductsApi.Models;

namespace ODataProductsApi.Repositories;

public interface IProductRepository
{
    IQueryable<Product> Query(bool asNoTracking = true);
    Task<Product?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Product> AddAsync(Product entity, CancellationToken ct = default);
    Task<Product?> UpdateAsync(Product entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}