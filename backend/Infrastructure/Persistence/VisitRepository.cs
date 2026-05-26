using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EncuentraTuHogar.Domain.Entities;
using EncuentraTuHogar.Application.Interfaces;

namespace EncuentraTuHogar.Infrastructure.Persistence;

public class VisitRepository : IVisitRepository
{
    private readonly AppDbContext _context;

    public VisitRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Visit?> GetByIdAsync(Guid id)
    {
        return await _context.Visits.FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<IEnumerable<Visit>> FindByPropertyIdAsync(Guid propertyId)
    {
        return await _context.Visits.Where(v => v.PropertyId == propertyId).ToListAsync();
    }

    public async Task<IEnumerable<Visit>> FindByPropertyIdsAsync(IEnumerable<Guid> propertyIds)
    {
        var ids = propertyIds.ToList();
        if (!ids.Any()) return new List<Visit>();
        return await _context.Visits.Where(v => ids.Contains(v.PropertyId)).ToListAsync();
    }

    public async Task<IEnumerable<Visit>> FindByVisitorIdAsync(string visitorId)
    {
        return await _context.Visits.Where(v => v.VisitorId == visitorId).ToListAsync();
    }

    public async Task AddAsync(Visit visit)
    {
        await _context.Visits.AddAsync(visit);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Visit visit)
    {
        _context.Visits.Update(visit);
        await _context.SaveChangesAsync();
    }
}
