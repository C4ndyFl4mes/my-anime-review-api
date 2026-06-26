using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FastEndpoints;
using Server.Data;
using Server.Exceptions;
using Server.Services;

namespace Server.Routes.User.Refresh;

/// <summary>
/// Endpoint responsible for refreshing JWT access tokens using a valid refresh token. 
/// It validates the provided access and refresh tokens, generates new tokens if valid, and sets them as HTTP-only cookies in the response.
/// If the refresh token is invalid or expired, it clears the cookies and returns an unauthorized response, prompting the user to sign in again.
/// </summary>
public class RefreshEndpoint(AppDbContext ctx, TokenService tokenService) : EndpointWithoutRequest<string>
{
    public override void Configure()
    {
        Post("/user/refresh");
        AllowAnonymous();
    }

    public override async Task<string> ExecuteAsync(CancellationToken ct)
    {
        if (!HttpContext.Request.Cookies.TryGetValue("accessToken", out string? accessToken))
            throw new UnauthorizedException("No access token provided.");

        if (!HttpContext.Request.Cookies.TryGetValue("refreshToken", out string? refreshToken))
            throw new UnauthorizedException("No refresh token provided.");

        JwtSecurityTokenHandler handler = new();
        if (!handler.CanReadToken(accessToken))
            throw new UnauthorizedException("Invalid access token.");

        JwtSecurityToken jwt = handler.ReadJwtToken(accessToken);

        Claim? userIdClaim = jwt.Claims.FirstOrDefault(c =>
            c.Type == ClaimTypes.NameIdentifier ||
            c.Type == "nameid"
        );

        if (!Guid.TryParse(userIdClaim?.Value, out Guid userId))
            throw new UnauthorizedException("Invalid user ID claim in access token.");

        try
        {
            RefreshData data = new(ctx, tokenService);
            (string newAccessToken, string newRefreshToken) = await data.RefreshTokensAsync(userId, refreshToken ?? string.Empty);

            HttpContext.Response.Cookies.Append("accessToken", newAccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddMinutes(8)
            });

            HttpContext.Response.Cookies.Append("refreshToken", newRefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return "Tokens refreshed successfully.";
        }
        catch (UnauthorizedException)
        {
            // Clear the cookies if the refresh token is invalid or expired
            HttpContext.Response.Cookies.Append("accessToken", string.Empty, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(-1)
            });

            HttpContext.Response.Cookies.Append("refreshToken", string.Empty, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(-1)
            });

            throw new UnauthorizedException("Invalid or expired refresh token. Please sign in again.");
        }
    }
}