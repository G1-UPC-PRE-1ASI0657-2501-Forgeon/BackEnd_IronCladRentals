using System.Net.Mime;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleService.VehicleBounded.Domain.Model.Aggregates;
using VehicleService.VehicleBounded.Domain.Repositories;
using VehicleService.VehicleBounded.Domain.Services;
using VehicleService.VehicleBounded.Interfaces.Resources;
using VehicleService.VehicleBounded.Interfaces.Transform;

namespace VehicleService.VehicleBounded.Interfaces.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public class VehicleController(
    IVehicleQueryService vehicleQueryService,
    IVehicleCommandService vehicleCommandService,
    ICompanyRepository companyRepository,
    ICompanyQueryService companyQueryService) : BaseController
{
    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableVehicles([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] string? city = null)
    {
        var vehicles = await vehicleQueryService.GetAvailableVehiclesAsync(startDate, endDate, city);
        var resources = vehicles.Select(VehicleTransform.ToResourceFromEntity);
        return Ok(resources);
    }

    [HttpGet("company/{companyId:int}")]
    public async Task<IActionResult> GetVehiclesByCompany(int companyId)
    {
        var vehicles = await vehicleQueryService.GetVehiclesByCompanyIdAsync(companyId);
        var resources = vehicles.Select(VehicleTransform.ToResourceFromEntity);
        return Ok(resources);
    }

    [HttpGet("{vehicleId:int}")]
    public async Task<IActionResult> GetVehicleDetails(int vehicleId)
    {
        var vehicle = await vehicleQueryService.GetVehicleDetailsAsync(vehicleId);
        if (vehicle == null) return NotFound();
        return Ok(VehicleTransform.ToResourceFromEntity(vehicle));
    }

    // 🔐 Solo usuarios autenticados con rol "Company"
    [Authorize(Roles = "True")]
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> CreateVehicle([FromForm] VehicleFormResource resource, IFormFile image)
    {
        var authUserId = GetAuthenticatedUserId();
        var company = await companyRepository.GetByUserIdAsync(authUserId);
        if (company == null)
            return BadRequest("No tienes una empresa registrada.");

        string imagePath = string.Empty;

        // ✅ Guardar imagen en disco
        if (image != null && image.Length > 0)
        {
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
            var directory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            Directory.CreateDirectory(directory); // crea si no existe

            var fullPath = Path.Combine(directory, fileName);
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            // Ruta accesible por frontend (ej: http://localhost:5000/uploads/nombre.jpg)
            imagePath = $"/uploads/{fileName}";
        }

        // Convertir a entidad y setear imagen
        var vehicle = new Vehicle(
            resource.Passengers,
            resource.LuggageCapacity,
            resource.LicensePlate,
            resource.Color,
            resource.Year,
            resource.Transmission,
            resource.FuelType,
            imagePath,
            resource.Address,
            resource.City,
            resource.Country,
            resource.Latitude,
            resource.Longitude,
            resource.ModelId,
            resource.BrandId,
            company.Id
        );

        if (resource.Pricing != null)
        {
            vehicle.SetPricing(resource.Pricing.DailyRate, resource.Pricing.WeeklyRate, resource.Pricing.Discount);
        }

        var created = await vehicleCommandService.CreateVehicleAsync(vehicle);
        return CreatedAtAction(nameof(GetVehicleDetails), new { vehicleId = created.Id }, VehicleTransform.ToResourceFromEntity(created));
    }


    [Authorize(Roles = "true")]
    [HttpPut("{vehicleId:int}")]
    public async Task<IActionResult> UpdateVehicle(int vehicleId, [FromBody] VehicleResource resource)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var company = await companyQueryService.GetByUserIdAsync(userId);
        if (company == null) return Forbid("Este usuario no tiene una empresa registrada.");

        var updatedEntity = VehicleTransform.ToEntityFromResource(resource);
        updatedEntity.SetCompany(company.Id);

        var updated = await vehicleCommandService.UpdateVehicleAsync(vehicleId, updatedEntity);
        return Ok(VehicleTransform.ToResourceFromEntity(updated));
    }

    [Authorize(Roles = "Company")]
    [HttpDelete("{vehicleId:int}")]
    public async Task<IActionResult> DeleteVehicle(int vehicleId)
    {
        var result = await vehicleCommandService.DeleteVehicleAsync(vehicleId);
        if (!result) return NotFound();
        return NoContent();
    }
}
