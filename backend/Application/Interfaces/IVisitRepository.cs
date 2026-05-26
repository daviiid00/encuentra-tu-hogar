using EncuentraTuHogar.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EncuentraTuHogar.Application.Interfaces;

public interface IVisitRepository
{
    Task<Visit?> GetByIdAsync(Guid id);
    Task<IEnumerable<Visit>> FindByPropertyIdAsync(Guid propertyId);
    Task<IEnumerable<Visit>> FindByVisitorIdAsync(string visitorId);
    Task AddAsync(Visit visit);
    Task UpdateAsync(Visit visit);
}
