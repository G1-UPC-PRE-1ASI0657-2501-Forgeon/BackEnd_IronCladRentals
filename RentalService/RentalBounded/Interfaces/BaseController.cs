using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace RentalService.RentalBounded.Interfaces;

public abstract class BaseController : ControllerBase
{
    protected Guid GetAuthenticatedUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
        return Guid.Parse(claim.Value);
    }

    protected string? GetUserRole()
    {
        return User.FindFirst(ClaimTypes.Role)?.Value;
    }
}