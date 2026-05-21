using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EncuentraTuHogar.Domain.Entities;
using EncuentraTuHogar.Domain.ValueObjects;
using EncuentraTuHogar.Application.Interfaces;

namespace EncuentraTuHogar.Infrastructure.Persistence;

public class PropertyRepository : IPropertyRepository
{
    private readonly AppDbContext _context;

    public PropertyRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Property>> FindByFiltersAsync(SearchFilter filter)
    {
        var query = _context.Properties.AsQueryable();

        if (!string.IsNullOrEmpty(filter.City))
        {
            query = query.Where(p => EF.Functions.Like(p.Address.City, $"%{filter.City}%"));
        }

        if (filter.Type.HasValue)
        {
            query = query.Where(p => p.Type == filter.Type.Value);
        }
        
        if (filter.Transaction.HasValue)
        {
            query = query.Where(p => p.Transaction == filter.Transaction.Value);
        }

        if (filter.MinPrice.HasValue)
        {
            query = query.Where(p => p.Price.Amount >= filter.MinPrice.Value);
        }

        if (filter.MaxPrice.HasValue)
        {
            query = query.Where(p => p.Price.Amount <= filter.MaxPrice.Value);
        }

        return await query.ToListAsync();
    }

    public async Task<Property?> FindByIdAsync(PropertyId id)
    {
        return await _context.Properties.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task AddAsync(Property property)
    {
        await _context.Properties.AddAsync(property);
        await _context.SaveChangesAsync();
    }
}
