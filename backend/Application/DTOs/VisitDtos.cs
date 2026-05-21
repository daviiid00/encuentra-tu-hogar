namespace EncuentraTuHogar.Application.DTOs;

public record ScheduleVisitRequest(
    Guid PropertyId,
    DateTime ScheduledDate
);

public record VisitDto(
    Guid Id,
    Guid PropertyId,
    string VisitorId,
    string VisitorName,
    DateTime ScheduledDate,
    string Status,
    DateTime CreatedAt
);
