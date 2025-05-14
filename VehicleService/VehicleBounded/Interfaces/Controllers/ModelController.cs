using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleService.VehicleBounded.Domain.Model.Entities;
using VehicleService.VehicleBounded.Domain.Services;
using VehicleService.VehicleBounded.Interfaces.Resources;
using VehicleService.VehicleBounded.Interfaces.Transform;

namespace VehicleService.VehicleBounded.Interfaces.Controllers;

[ApiController]
[Route("api/v1/models")]
public class ModelController(
    IModelCommandService modelCommandService,
    IModelQueryService modelQueryService) : ControllerBase
{
    // 🔐 Solo el admin puede crear modelos
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateModel([FromBody] ModelResource resource)
    {
        var email = User.FindFirstValue(ClaimTypes.Name);
        if (email != "admin@admin.com") return Forbid();

        var model = new Model(resource.CarModel, resource.BrandId);
        var created = await modelCommandService.CreateModelAsync(model);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ModelTransform.ToResourceFromEntity(created));
    }

    // 📦 Obtener modelo por ID
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var model = await modelQueryService.GetModelByIdAsync(id);
        if (model == null) return NotFound();
        return Ok(ModelTransform.ToResourceFromEntity(model));
    }

    // 📦 Obtener todos los modelos
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var models = await modelQueryService.GetAllModelsAsync();
        return Ok(models.Select(ModelTransform.ToResourceFromEntity));
    }
}