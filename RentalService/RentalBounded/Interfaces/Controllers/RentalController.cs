using System.Net.Mime;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentalService.RentalBounded.Domain.Model.Aggregates;
using RentalService.RentalBounded.Domain.Services;
using RentalService.RentalBounded.Interfaces.Resources;
using RentalService.RentalBounded.Interfaces.Transform;

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
    public async Task<IActionResult> CreateRental([FromBody] RentalResource resource)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var rental = new Rental(userId, resource.VehicleId, resource.StartDate, resource.EndDate, "Pending", resource.TotalPrice);
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
}
