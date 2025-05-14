using Microsoft.AspNetCore.Mvc;
using VehicleService.VehicleBounded.Domain.Services;
using VehicleService.VehicleBounded.Interfaces.Transform;

namespace VehicleService.VehicleBounded.Interfaces.Controllers;

[ApiController]
[Route("api/v1/brands")]
public class BrandController(IBrandQueryService brandQueryService) : ControllerBase
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
}