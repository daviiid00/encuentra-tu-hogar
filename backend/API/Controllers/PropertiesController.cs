using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using EncuentraTuHogar.Application.DTOs;
using EncuentraTuHogar.Application.Interfaces;
using EncuentraTuHogar.Domain.Common;

namespace EncuentraTuHogar.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PropertiesController : ControllerBase
{
    private readonly IPropertyService _propertyService;

    public PropertiesController(IPropertyService propertyService)
    {
        _propertyService = propertyService;
    }

    // GET /api/properties  — public, with optional filters — spec: properties.spec.md
    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] PropertyFilterRequest filter)
    {
        var results = await _propertyService.SearchAsync(filter);
        return Ok(results);
    }

    // GET /api/properties/{id}  — public
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _propertyService.GetByIdAsync(id);
        return result switch
        {
            Result<PropertyDto>.Success s => Ok(s.Value),
            Result<PropertyDto>.Failure f => NotFound(new { error = f.Error }),
            _ => StatusCode(500)
        };
    }

    // POST /api/properties  — spec: Only landlords can publish
    [Authorize(Roles = "Landlord")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePropertyRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? User.FindFirstValue(ClaimTypes.Name);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _propertyService.CreateAsync(request, userId);
        return result switch
        {
            Result<PropertyDto>.Success s => CreatedAtAction(nameof(GetById), new { id = s.Value.Id }, s.Value),
            Result<PropertyDto>.Failure f => BadRequest(new { error = f.Error }),
            _ => StatusCode(500)
        };
    }

    // PUT /api/properties/{id}  — spec: Only owner can edit
    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdatePropertyRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? User.FindFirstValue(ClaimTypes.Name);

        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _propertyService.UpdateAsync(id, request, userId);
        return result switch
        {
            Result<PropertyDto>.Success s => Ok(s.Value),
            Result<PropertyDto>.Failure f when f.Error.Contains("permiso") => Forbid(),
            Result<PropertyDto>.Failure f when f.Error.Contains("encontrada") => NotFound(new { error = f.Error }),
            Result<PropertyDto>.Failure f => BadRequest(new { error = f.Error }),
            _ => StatusCode(500)
        };
    }

    // DELETE /api/properties/{id}  — spec: owner or admin
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? User.FindFirstValue(ClaimTypes.Name);

        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var isAdmin = User.IsInRole("Admin");
        var result = await _propertyService.DeleteAsync(id, userId, isAdmin);

        return result switch
        {
            Result<bool>.Success => NoContent(),
            Result<bool>.Failure f when f.Error.Contains("permiso") => Forbid(),
            Result<bool>.Failure f when f.Error.Contains("encontrada") => NotFound(new { error = f.Error }),
            Result<bool>.Failure f => BadRequest(new { error = f.Error }),
            _ => StatusCode(500)
        };
    }
}
