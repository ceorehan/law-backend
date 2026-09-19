using System.Security.Claims;

namespace ZALaw.Api.Services;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var value = principal.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? principal.FindFirstValue("sub");
        return Guid.TryParse(value, out var id) ? id : Guid.Empty;
    }

    public static string? GetRole(this ClaimsPrincipal principal) =>
        principal.FindFirstValue(ClaimTypes.Role);
}
