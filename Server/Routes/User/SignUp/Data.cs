using Microsoft.AspNetCore.Identity;
using Server.Data;
using Server.Entities;
using Server.Services;
using Microsoft.EntityFrameworkCore;
using Server.Exceptions;

namespace Server.Routes.User.SignUp;

/// <summary>
/// Responsible for handling the data operations related to signing up a new user.
/// </summary>
public class SignUpData(AppDbContext ctx, TokenService tokenService)
{
    public async Task<SignInResponse> SignUpAsync(UserDto request, CancellationToken ct)
    {
        if (await ctx.Users.AnyAsync(u => u.Email == request.Email, ct) || await ctx.Users.AnyAsync(u => u.Username == request.Username, ct))
        {
            throw new ConflictException("A user with this email or username already exists.");
        }

        RoleEntity role = await ctx.Roles.FirstOrDefaultAsync(r => r.Name == "User", ct) ??
            throw new InvalidOperationException("Default user role not found in the database.");

        UserEntity user = new()
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Username = request.Username,
            ProfileImageURL = request.ProfileImageURL ?? string.Empty,
            RoleId = role.Id,
            Role = role
        };

        string hashedPassword = new PasswordHasher<UserEntity>()
            .HashPassword(user, request.Password);

        user.PasswordHash = hashedPassword;

        ctx.Users.Add(user);
        await ctx.SaveChangesAsync(ct);

        return new SignInResponse
        {
            AccessToken = tokenService.GetAccessToken(user, request.Password),
            RefreshToken = await tokenService.GenerateAndSaveRefreshTokenAsync(user),
            Username = user.Username,
            ProfileImageURL = user.ProfileImageURL
        };
    }
}