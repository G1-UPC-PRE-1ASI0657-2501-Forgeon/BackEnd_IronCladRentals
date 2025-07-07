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
    [HttpPost]
    public async Task<IActionResult> CreateRental([FromBody] RentalResourceCreate resource)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var rental = RentalTransform.ToEntityFromResource(resource);
        rental.UserId = userId;

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
            LicensePlate = vehicleData.LicensePlate
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


}
