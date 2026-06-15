using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Server.Data;
using Server.Entities;
using Server.Exceptions;

namespace Server.Services;

/// <summary>
/// Service responsible for generating JWT access tokens for authenticated users.
/// </summary>
public class TokenService(IConfiguration config, AppDbContext ctx)
{
    /// <summary>
    /// Generates a JWT access token for the given user if the provided password is correct.
    /// </summary>
    /// <returns>A JWT access token.</returns>
    /// <exception cref="BadRequestException">Thrown when the provided credentials are invalid.</exception>
    public string GetAccessToken(UserEntity user, string password)
    {
        if (new PasswordHasher<UserEntity>().VerifyHashedPassword(user, user.PasswordHash, password) == PasswordVerificationResult.Failed)
        {
            throw new BadRequestException("Invalid credentials.");
        }

        return CreateToken(user);
    }

    /// <summary>
    /// Generates a secure random refresh token.
    /// </summary>
    /// <returns>A secure random refresh token.</returns>
    private string GenerateRefreshToken()
    {
        byte[] randomNumber = new byte[64];
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }

    /// <summary>
    /// Generates a new refresh token for the given user, saves it to the database, and returns it.
    /// </summary>
    /// <returns>The newly generated refresh token.</returns>
    public async Task<string> GenerateAndSaveRefreshTokenAsync(UserEntity user)
    {
        string refreshToken = GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        ctx.Users.Update(user);
        await ctx.SaveChangesAsync();

        return refreshToken;
    }

    /// <summary>
    /// Creates a JWT access token for the given user based on their claims and the secret key from configuration.
    /// </summary>
    /// <returns>A JWT access token.</returns>
    /// <exception cref="Exception">Thrown when the secret key, issuer, or audience is not found in the configuration.</exception>
    public string CreateToken(UserEntity user)
    {
        List<Claim> claims =
        [
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.AuthenticationInstant, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.Name)
        ];

        string secretKey = config["secret_key.txt"] ?? throw new InternalServerException("Secret key not found in configuration.");
        string issuer = config["issuer.txt"] ?? throw new InternalServerException("Issuer not found in configuration.");
        string audience = config["audience.txt"] ?? throw new InternalServerException("Audience not found in configuration.");

        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(secretKey));

        SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha512);

        JwtSecurityToken tokenDescriptor = new(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(8),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }
}