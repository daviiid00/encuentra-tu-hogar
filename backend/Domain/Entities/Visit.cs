using EncuentraTuHogar.Domain.Common;
using EncuentraTuHogar.Domain.ValueObjects;
using System;

namespace EncuentraTuHogar.Domain.Entities;

public class Visit : Entity
{
    public Guid Id { get; private set; }
    public Guid PropertyId { get; private set; }
    public string VisitorId { get; private set; } // Map to Identity User Id
    public DateTime ScheduledDate { get; private set; }
    public VisitStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    private Visit() { }
    
    public static Visit Create(Guid propertyId, string visitorId, DateTime scheduledDate)
    {
        return new Visit
        {
            Id = Guid.NewGuid(),
            PropertyId = propertyId,
            VisitorId = visitorId,
            ScheduledDate = scheduledDate,
            Status = VisitStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
    }
    
    public void Confirm() => Status = VisitStatus.Confirmed;
    public void Cancel() => Status = VisitStatus.Cancelled;
    public void Complete() => Status = VisitStatus.Completed;
}
