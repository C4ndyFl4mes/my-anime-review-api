using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Entities;
using Server.Exceptions;
using Server.Services;

namespace Server.Routes.User.Refresh;

/// <summary>
/// This class encapsulates the logic for refreshing JWT access tokens using refresh tokens.
/// It validates the provided refresh token against the stored token in the database, checks for expiration, and if valid, generates a new access token and refresh token for the user.
/// The new refresh token is also saved in the database to replace the old one.
/// </summary>
public class RefreshData(AppDbContext ctx, TokenService tokenService)
{
    public async Task<(string AccessToken, string RefreshToken, Guid UserId)> RefreshTokensAsync(Guid userId, string refreshToken)
    {
        UserEntity user = await ValidateRefreshTokenAsync(userId, refreshToken, CancellationToken.None) ??
            throw new UnauthorizedException("Invalid refresh token.");

        return (
            AccessToken: tokenService.CreateToken(user),
            RefreshToken: await tokenService.GenerateAndSaveRefreshTokenAsync(user),
            UserId: user.Id
        );
    }

    private async Task<UserEntity?> ValidateRefreshTokenAsync(Guid userId, string refreshToken, CancellationToken ct)
    {
        UserEntity? user = await ctx.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == userId, ct);
        if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            return null;
        return user;
    }
}