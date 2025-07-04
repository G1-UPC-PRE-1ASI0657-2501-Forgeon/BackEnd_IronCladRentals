using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleService.VehicleBounded.Domain.Model.Entities;
using VehicleService.VehicleBounded.Domain.Services;
using VehicleService.VehicleBounded.Interfaces.Resources;
using VehicleService.VehicleBounded.Interfaces.Transform;

namespace VehicleService.VehicleBounded.Interfaces.Controllers;

[ApiController]
[Route("api/v1/companies")]
public class CompanyController(ICompanyCommandService commandService,ICompanyQueryService queryService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var companies = await queryService.GetAllAsync();
        return Ok(companies.Select(CompanyTransform.ToResourceFromEntity));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var company = await queryService.GetByIdAsync(id);
        if (company == null) return NotFound();
        return Ok(CompanyTransform.ToResourceFromEntity(company));
    }
    
    // 🔐 POST /api/v1/companies (solo usuarios autenticados con rol "Company")
    [HttpPost]
    [Authorize(Roles = "True")]
    public async Task<IActionResult> CreateCompany([FromBody] CompanyResource resource)
    {
        var authUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(authUserId)) return Unauthorized();

        var company = new Company(resource.Name, resource.RUC);
        company.SetAuthUserId(Guid.Parse(authUserId)); // Asegúrate de tener este método

        var created = await commandService.CreateCompanyAsync(company);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, CompanyTransform.ToResourceFromEntity(created));
    }

    [HttpGet("me")]
    [Authorize(Roles = "True")]
    public async Task<IActionResult> GetCompanyByAuthUserId()
    {
        var authUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(authUserId)) return Unauthorized();
        
        if (!Guid.TryParse(authUserId, out var userId))
            return BadRequest("Invalid user ID format.");
        
        var company = await queryService.GetByUserIdAsync(userId);
        if (company == null) return NotFound();

        return Ok(CompanyTransform.ToResourceFromEntity(company));
    }
}
