using System.Security.Claims;
using Server.Exceptions;

namespace Server.Utils;

/// <summary>
/// Utility class for retrieving information about the currently authenticated user from the HttpContext.
/// </summary>
public static class UserUtils
{
    private static HttpContext? CurrentHttpContext => new HttpContextAccessor().HttpContext;

    public static Guid GetAuthenticatedUserID()
    {
        Guid currentUserId = Guid.Empty;
        if (CurrentHttpContext?.User.Identity?.IsAuthenticated == true)
        {
            string? userIdRaw =
                CurrentHttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                CurrentHttpContext?.User.FindFirst("nameid")?.Value;
            
            if (userIdRaw is not null)
            {
                if (!Guid.TryParse(userIdRaw, out currentUserId))
                    throw new BadRequestException("Unable to parse Guid for the current user.");
            }
        }

        return currentUserId;
    }

    public static string GetAuthenticatedUserRole()
    {
        string role = string.Empty;
        if (CurrentHttpContext?.User.Identity?.IsAuthenticated == true)
        {
            role = CurrentHttpContext?.User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
        }

        return role;
    }
}