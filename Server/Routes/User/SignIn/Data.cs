using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Entities;
using Server.Exceptions;
using Server.Services;

namespace Server.Routes.User.SignIn;

/// <summary>
/// Responsible for handling the data operations related to signing in a user.
/// </summary>
public class SignInData(AppDbContext ctx, TokenService tokenService)
{
    public async Task<SignInResponse> SignInAsync(SignInDto request, CancellationToken ct)
    {
        UserEntity user = await ctx.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == request.Email, ct) ??
            throw new BadRequestException("Invalid credentials.");

        return new SignInResponse
        {
            AccessToken = tokenService.GetAccessToken(user, request.Password),
            RefreshToken = await tokenService.GenerateAndSaveRefreshTokenAsync(user),
            Username = user.Username,
            ProfileImageURL = user.ProfileImageURL
        };
    }

    
}