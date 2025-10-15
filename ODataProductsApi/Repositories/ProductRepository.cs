using Microsoft.EntityFrameworkCore;
using ODataProductsApi.Data;
using ODataProductsApi.Models;

namespace ODataProductsApi.Repositories;

public class ProductRepository: IProductRepository
{
    private readonly AppDbContext _dbContext;

    public ProductRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IQueryable<Product> Query(bool asNoTracking = true)
    {
        var query = _dbContext.Products.Include(p => p.Category);
        return asNoTracking ? query.AsNoTracking() : query;
    }
    
    public async Task<Product?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _dbContext.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<Product> AddAsync(Product entity, CancellationToken ct = default)
    {
        await _dbContext.Products.AddAsync(entity, ct);
        await _dbContext.SaveChangesAsync(ct);
        return entity;
    }

    public async Task<Product?> UpdateAsync(Product entity, CancellationToken ct = default)
    {
        var existing = await _dbContext.Products.FindAsync([entity.Id], ct);
        if (existing is null)
            return null;

        _dbContext.Entry(existing).CurrentValues.SetValues(entity);
        await _dbContext.SaveChangesAsync(ct);
        return existing;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Products.FindAsync([id], ct);
        if (entity is null)
            return false;

        _dbContext.Products.Remove(entity);
        await _dbContext.SaveChangesAsync(ct);
        return true;
    }
}