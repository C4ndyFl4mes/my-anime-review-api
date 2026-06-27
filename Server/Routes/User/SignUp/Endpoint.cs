using FastEndpoints;
using Server.Data;
using Server.Services;

namespace Server.Routes.User.SignUp;

/// <summary>
/// Endpoint for signing up a new user.
/// Automatically signs in the user after successful sign up, so the client can immediately log them in without needing to make a separate sign in request.
/// </summary>
public class SignUpEndpoint(AppDbContext ctx, TokenService tokenService) : Endpoint<UserDto, SignInResponse>
{
    public override void Configure()
    {
        Post("/user/signup");
        AllowAnonymous();
    }

    public override async Task<SignInResponse> ExecuteAsync(UserDto request, CancellationToken ct)
    {
        SignUpData signUpData = new(ctx, tokenService);
        
        SignInResponse responseWithToken = await signUpData.SignUpAsync(request, ct);

        HttpContext.Response.Cookies.Append("accessToken", responseWithToken.AccessToken ?? string.Empty, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddMinutes(8)
        });

        HttpContext.Response.Cookies.Append("refreshToken", responseWithToken.RefreshToken ?? string.Empty, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddDays(7)
        });

        SignInResponse responseWithoutToken = new()
        {
            UserId = responseWithToken.UserId,
            IsAdmin = responseWithToken.IsAdmin
        };

        return responseWithoutToken;
    }
}