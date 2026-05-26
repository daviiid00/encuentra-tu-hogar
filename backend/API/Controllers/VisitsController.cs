using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using EncuentraTuHogar.Application.DTOs;
using EncuentraTuHogar.Application.Interfaces;
using EncuentraTuHogar.Domain.Entities;

namespace EncuentraTuHogar.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // All visits require authentication
public class VisitsController : ControllerBase
{
    private readonly IVisitRepository _visitRepository;

    public VisitsController(IVisitRepository visitRepository)
    {
        _visitRepository = visitRepository;
    }

    // POST /api/visits  — create a visit
    [HttpPost]
    public async Task<IActionResult> ScheduleVisit([FromBody] ScheduleVisitRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        if (request.ScheduledDate < DateTime.Now)
            return BadRequest("La fecha debe ser en el futuro.");

        var visit = Visit.Create(request.PropertyId, userId, request.ScheduledDate);
        await _visitRepository.AddAsync(visit);

        return Ok(new VisitDto(
            visit.Id,
            visit.PropertyId,
            visit.VisitorId,
            "Usuario", // Visitor name lookup could be added here if needed, but keeping it simple
            visit.ScheduledDate,
            visit.Status.ToString(),
            visit.CreatedAt
        ));
    }
}
