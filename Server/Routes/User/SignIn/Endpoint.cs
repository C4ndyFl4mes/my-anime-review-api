using FastEndpoints;
using Server.Data;
using Server.Services;
namespace Server.Routes.User.SignIn;

/// <summary>
/// Endpoint for signing in an existing user.
/// </summary>
public class SignInEndpoint(AppDbContext ctx, TokenService tokenService) : Endpoint<SignInDto, SignInResponse>
{
    public override void Configure()
    {
        Post("/user/signin");
        AllowAnonymous();
    }

    public override async Task<SignInResponse> ExecuteAsync(SignInDto request, CancellationToken ct)
    {
        SignInData signInData = new(ctx, tokenService);

        SignInResponse responseWithToken = await signInData.SignInAsync(request, ct);

        HttpContext.Response.Cookies.Append("accessToken", responseWithToken.AccessToken ?? string.Empty, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddMinutes(8)
        });

        HttpContext.Response.Cookies.Append("refreshToken", responseWithToken.RefreshToken ?? string.Empty, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        });

        SignInResponse responseWithoutToken = new()
        {
            Username = responseWithToken.Username,
            ProfileImageURL = responseWithToken.ProfileImageURL
        };

        return responseWithoutToken;
    }
}