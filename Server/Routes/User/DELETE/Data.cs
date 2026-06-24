using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Entities;
using Server.Exceptions;

namespace Server.Routes.User.DELETE;

public class DeleteUserData(AppDbContext ctx)
{
    public async Task<DeleteUserResponse> DeleteUserAsync(Guid userId, CancellationToken ct)
    {
        UserEntity user = await ctx.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == userId, ct) ??
            throw new NotFoundException("The user doesn't exist.");
        
        if (user.Role.Name == "Admin")
            throw new UnauthorizedException("You can't delete an admin.");

        ctx.Users.Remove(user);

        await ctx.SaveChangesAsync(ct);

        return new()
        {
            Message = "The user has been removed."
        };
    }
}