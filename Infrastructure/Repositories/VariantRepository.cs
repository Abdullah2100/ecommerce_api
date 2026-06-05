using api.application;
using api.domain.entity;
using api.domain.Interface;
using Microsoft.EntityFrameworkCore;

namespace api.Infrastructure.Repositories;

public class VariantRepository(AppDbContext context) : IVarientRepository
{
    public async Task<IEnumerable<Variant>> GetAllAsync(int page, int length)
    {
        return await context
            .Variants
            .AsNoTracking()
            .Skip((page - 1) * length)
            .Take(length)
            .ToListAsync();
    }

    public void Add(Variant entity)
    {
        context.Variants.AddAsync(entity);
    }

    public void Update(Variant entity)
    {
        context.Variants.Update(entity);
    }

    public void Delete(Guid id)
    {
        var variants = context
            .Variants
            .Where(i => i.Id == id)
            .ToList();
        context.Variants.RemoveRange(variants);
    }

    public async Task<Variant?> GetVarient(Guid id)
    {
        return await context
            .Variants
            .FindAsync(id);
    }

    public async Task<List<Variant>> GetVarients(int page, int length)
    {
        var variants = await context
            .Variants
            .AsNoTracking()
            .Skip((page - 1) * length)
            .Take(length)
            .ToListAsync();
        return variants;
    }

    public async Task<int> GetVarientCount(int variantPerPage)
    {
        int count = await context
            .Stores
            .AsNoTracking()
            .CountAsync();
        if (count == 0) return 0;
        count = (int)Math.Ceiling((double)count / variantPerPage);
        return count;
    }

    public async Task<bool> IsExist(Guid id)
    {
        return await context
            .Variants
            .AsNoTracking()
            .AnyAsync(i => i.Id == id);
    }

    public async Task<bool> IsExist(string name)
    {
        return await context
            .Variants
            .AsNoTracking()
            .AnyAsync(i => i.Name == name);
    }

    public async Task<bool> IsExist(string name, Guid id)
    {
        return await context
            .Variants
            .AsNoTracking()
            .AnyAsync(i => i.Name == name && i.Id != id);
    }
}