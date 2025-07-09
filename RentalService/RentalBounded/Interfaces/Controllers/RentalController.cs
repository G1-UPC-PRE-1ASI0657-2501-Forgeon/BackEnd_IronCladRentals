using System.Net.Mime;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RentalService.RentalBounded.Domain.Model.Aggregates;
using RentalService.RentalBounded.Domain.Services;
using RentalService.RentalBounded.Interfaces.Resources;
using RentalService.RentalBounded.Interfaces.Resources.Vehicle;
using RentalService.RentalBounded.Interfaces.Transform;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace RentalService.RentalBounded.Interfaces.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public class RentalController(
    IRentalQueryService rentalQueryService,
    IRentalCommandService rentalCommandService
) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var rentals = await rentalQueryService.GetAllAsync();
        return Ok(rentals.Select(RentalTransform.ToResourceFromEntity));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var rental = await rentalQueryService.GetByIdAsync(id);
        return rental is null ? NotFound() : Ok(RentalTransform.ToResourceFromEntity(rental));
    }

    [HttpGet("user")] // /user?userId=xxxxx
    public async Task<IActionResult> GetByUserId([FromQuery] Guid userId)
    {
        var rentals = await rentalQueryService.GetByUserIdAsync(userId);
        return Ok(rentals.Select(RentalTransform.ToResourceFromEntity));
    }

    [HttpGet("vehicle/{vehicleId:int}")]
    public async Task<IActionResult> GetByVehicleId(int vehicleId)
    {
        var rentals = await rentalQueryService.GetByVehicleIdAsync(vehicleId);
        return Ok(rentals.Select(RentalTransform.ToResourceFromEntity));
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateRental([FromBody] RentalResourceCreate resource)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // 1️⃣ Obtener datos del vehículo llamando al microservicio Vehicle
        using var httpClient = new HttpClient();

        // ⚠️ Cambia esta URL por la real de tu microservicio de vehicle
        var vehicleServiceUrl = $"http://localhost:5162/api/v1/vehicle/{resource.VehicleId}";
        var vehicleResponse = await httpClient.GetAsync(vehicleServiceUrl);

        if (!vehicleResponse.IsSuccessStatusCode)
        {
            return BadRequest("No se pudo obtener información del vehículo.");
        }

        var vehicleJson = await vehicleResponse.Content.ReadAsStringAsync();
        var vehicleData = JsonSerializer.Deserialize<VehicleResource>(vehicleJson, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (vehicleData == null || vehicleData.CompanyId == null)
        {
            return BadRequest("El vehículo no tiene compañía asociada.");
        }

        // 2️⃣ Mapear tu entidad Rental
        var rental = RentalTransform.ToEntityFromResource(resource);
        rental.UserId = userId;
        rental.CompanyId = vehicleData.CompanyId; // AQUÍ está la magia 🪄

        // 3️⃣ Guardar
        var created = await rentalCommandService.CreateRentalAsync(rental);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, RentalTransform.ToResourceFromEntity(created));
    }


    [Authorize]
    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        await rentalCommandService.CancelRentalAsync(id);
        return NoContent();
    }

    [Authorize]
    [HttpPost("{id:guid}/confirm")]
    public async Task<IActionResult> Confirm(Guid id)
    {
        await rentalCommandService.ConfirmRentalAsync(id);
        return NoContent();
    }
    
    [Authorize]
    [HttpPost("{id:guid}/paid")]
    public async Task<IActionResult> Paid(Guid id)
    {
        await rentalCommandService.PaidRentalAsync(id);
        return NoContent();
    }


    [Authorize]
    [HttpPost("{id:guid}/complete")]
    public async Task<IActionResult> Complete(Guid id)
    {
        await rentalCommandService.CompleteRentalAsync(id);
        return NoContent();
    }

    [Authorize]
    [HttpPost("{id:guid}/extend")]
    public async Task<IActionResult> Extend(Guid id, [FromBody] DateTime newEndDate)
    {
        await rentalCommandService.ExtendRentalAsync(id, newEndDate);
        return NoContent();
    }

    [Authorize]
    [HttpGet("rentalinformationvehicle/{id}")]
    public async Task<IActionResult> GetRentalInformationVehicle(Guid id)
    {
        var rental = await rentalQueryService.GetByIdAsync(id);
        if (rental is null){return NotFound("Rental not found");}
        
        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync($"http://localhost:5162/api/v1/vehicle/{rental.VehicleId}");

        if (!response.IsSuccessStatusCode) {return StatusCode(StatusCodes.Status500InternalServerError, "Error al obtener los datos del vehiculo");}

        
        var vehicleJson = await response.Content.ReadAsStringAsync();
        var vehicleData = JsonSerializer.Deserialize<VehicleResource>(vehicleJson,new JsonSerializerOptions {PropertyNameCaseInsensitive = true} );

        var result = new RentalWithVehicleResource
        {
            RentalId = rental.Id,
            AuthUserId = rental.UserId,
            VehicleId = vehicleData.Id,
            RentalStatus = rental.RentalStatus,
            StartDate = rental.StartDate,
            EndDate = rental.EndDate,
            BrandName = vehicleData.BrandName,
            ModelName = vehicleData.ModelName,
            Color = vehicleData.Color,
            LicensePlate = vehicleData.LicensePlate,
            Paid = rental.Paid,
            pricing = vehicleData.pricing.DailyRate,

        };
        return Ok(result); 
    }
    
    [Authorize]
    [HttpGet("me/active")]
    public async Task<IActionResult> GetMyActiveRentals()
    {
        // Tomar el userId del JWT
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized();

        var userId = Guid.Parse(userIdClaim);

        // Obtener todas las rentas del usuario
        var rentals = await rentalQueryService.GetByUserIdAsync(userId);

        // Filtrar solo las activas
        var activeRentals = rentals
            .Where(r => r.RentalStatus.Equals("Confirmed", StringComparison.OrdinalIgnoreCase))
            .Select(RentalTransform.ToResourceFromEntity);

        return Ok(activeRentals);
    }
    
    [Authorize]
    [HttpGet("me/pending")]
    public async Task<IActionResult> GetMyPendingRentals()
    {
        // Tomar el userId del JWT
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized();

        var userId = Guid.Parse(userIdClaim);

        // Obtener todas las rentas del usuario
        var rentals = await rentalQueryService.GetByUserIdAsync(userId);

        // Filtrar solo las activas
        var activeRentals = rentals
            .Where(r => r.RentalStatus.Equals("Pending", StringComparison.OrdinalIgnoreCase))
            .Select(RentalTransform.ToResourceFromEntity);

        return Ok(activeRentals);
    }
    [Authorize]
    [HttpGet("company/{companyId}/pending")]
    public async Task<IActionResult> GetPendingRentalsByCompanyId(int companyId)
    {
        // 1️⃣ Obtener todas las rentas de esa compañía
        var rentals = await rentalQueryService.GetByCompanyIdAsync(companyId);

        // 2️⃣ Filtrar por estado "Pending"
        var pendingRentals = rentals
            .Where(r => r.RentalStatus.Equals("Pending", StringComparison.OrdinalIgnoreCase))
            .Select(RentalTransform.ToResourceFromEntity);

        return Ok(pendingRentals);
    }
    [Authorize]
    [HttpGet("me/pending/paid")]
    public async Task<IActionResult> GetMyPendingAndPaidRentals()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized();

        var userId = Guid.Parse(userIdClaim);

        var rentals = await rentalQueryService.GetByUserIdAsync(userId);

        var result = rentals
            .Where(r => r.RentalStatus.Equals("Pending", StringComparison.OrdinalIgnoreCase) && r.Paid)
            .Select(RentalTransform.ToResourceFromEntity);

        return Ok(result);
    }
    [Authorize]
    [HttpGet("me/pending/unpaid")]
    public async Task<IActionResult> GetMyPendingAndUnpaidRentals()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized();

        var userId = Guid.Parse(userIdClaim);

        var rentals = await rentalQueryService.GetByUserIdAsync(userId);

        var result = rentals
            .Where(r => r.RentalStatus.Equals("Pending", StringComparison.OrdinalIgnoreCase) && !r.Paid)
            .Select(RentalTransform.ToResourceFromEntity);

        return Ok(result);
    }
    
    [Authorize]
    [HttpGet("company/{companyId}/pending/paid")]
    public async Task<IActionResult> GetCompanyPendingAndPaid(int companyId)
    {
        var rentals = await rentalQueryService.GetByCompanyIdAsync(companyId);

        var result = rentals
            .Where(r => r.RentalStatus.Equals("Pending", StringComparison.OrdinalIgnoreCase) && r.Paid)
            .Select(RentalTransform.ToResourceFromEntity);

        return Ok(result);
    }

    
    [Authorize]
    [HttpGet("company/{companyId}/pending/unpaid")]
    public async Task<IActionResult> GetCompanyPendingAndUnpaid(int companyId)
    {
        var rentals = await rentalQueryService.GetByCompanyIdAsync(companyId);

        var result = rentals
            .Where(r => r.RentalStatus.Equals("Pending", StringComparison.OrdinalIgnoreCase) && !r.Paid)
            .Select(RentalTransform.ToResourceFromEntity);

        return Ok(result);
    }

    [Authorize]
    [HttpGet("company/{companyId}/complete")]
    public async Task<IActionResult> GetCompanyComplete(int companyId)
    {
        var rentals = await rentalQueryService.GetByCompanyIdAsync(companyId);

        var result = rentals
            .Where(r => r.RentalStatus.Equals("Complete", StringComparison.OrdinalIgnoreCase))
            .Select(RentalTransform.ToResourceFromEntity);

        return Ok(result);
    }



}
