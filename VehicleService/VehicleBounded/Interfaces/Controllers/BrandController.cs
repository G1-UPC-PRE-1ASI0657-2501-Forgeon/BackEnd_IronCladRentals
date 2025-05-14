using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleService.VehicleBounded.Domain.Model.Entities;
using VehicleService.VehicleBounded.Domain.Services;
using VehicleService.VehicleBounded.Interfaces.Resources;
using VehicleService.VehicleBounded.Interfaces.Transform;

namespace VehicleService.VehicleBounded.Interfaces.Controllers;

[ApiController]
[Route("api/v1/brands")]
public class BrandController(IBrandQueryService brandQueryService, IBrandCommandService brandCommandService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var brands = await brandQueryService.GetAllBrandsAsync();
        return Ok(brands.Select(BrandTransform.ToResourceFromEntity));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var brand = await brandQueryService.GetBrandByIdAsync(id);
        if (brand == null) return NotFound();
        return Ok(BrandTransform.ToResourceFromEntity(brand));
    }
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateBrand([FromBody] BrandResource resource)
    {
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;

        if (email != "admin")
            return Forbid("Solo el admin puede crear nuevas marcas.");

        var brand = new Brand(resource.BrandName);
        var created = await brandCommandService.CreateBrandAsync(brand);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, BrandTransform.ToResourceFromEntity(created));
    }

    
}