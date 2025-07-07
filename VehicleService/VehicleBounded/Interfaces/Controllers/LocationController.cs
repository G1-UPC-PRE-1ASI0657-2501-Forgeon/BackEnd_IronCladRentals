using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleService.VehicleBounded.Domain.Model.Entities;
using VehicleService.VehicleBounded.Domain.Services;
using VehicleService.VehicleBounded.Interfaces.Transform;
using VehicleService.VehicleBounded.Interfaces.Resources;

namespace VehicleService.VehicleBounded.Interfaces.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class LocationsController : ControllerBase
{
    private readonly ILocationCommandService _commandService;
    private readonly ILocationQueryService _queryService;
    private readonly ICompanyCommandService _companyCommandService;
    private readonly ICompanyQueryService _companyQueryService; 

    public LocationsController(
        ILocationCommandService commandService,
        ILocationQueryService queryService,
        ICompanyCommandService companyCommandService,
        ICompanyQueryService companyQueryService)
    
    {
        _commandService = commandService;
        _queryService = queryService;
        _companyCommandService = companyCommandService;
        _companyQueryService = companyQueryService;
    }

    // GET: api/v1/locations
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var locations = await _queryService.GetAllLocationsAsync();
        var resources = locations.Select(LocationTransform.ToResourceFromEntity);
        return Ok(resources);
    }

    // GET: api/v1/locations/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var location = await _queryService.GetLocationByIdAsync(id);
        if (location == null)
            return NotFound();

        return Ok(LocationTransform.ToResourceFromEntity(location));
    }

    // GET: api/v1/companies/{companyId}/locations
    [HttpGet("/api/v1/companies/{companyId}/locations")]
    public async Task<IActionResult> GetByCompanyId(int companyId)
    {
        var locations = await _queryService.GetByCompanyIdAsync(companyId);
        var resources = locations.Select(LocationTransform.ToResourceFromEntity);
        return Ok(resources);
    }

    // POST: api/v1/locations
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] LocationResource resource)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim))
            return Unauthorized();

        Guid userId = Guid.Parse(userIdClaim);

        // Obtener la compañía del usuario autenticado
        var company = await _companyQueryService.GetByUserIdAsync(userId);
        if (company == null)
            return BadRequest("El usuario no tiene una compañía registrada.");

        var location = new Location(
            resource.Address,
            resource.City,
            resource.Country,
            resource.LocationStatus,
            resource.Latitude,
            resource.Longitude,
            company.Id // Aquí aseguras que sea su company
        );

        var created = await _commandService.CreateLocationAsync(location);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, LocationTransform.ToResourceFromEntity(created));
    }

}
