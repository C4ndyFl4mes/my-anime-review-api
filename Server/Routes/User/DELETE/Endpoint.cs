using FastEndpoints;
using Server.Data;

namespace Server.Routes.User.DELETE;

public class DeleteUserEndpoint(AppDbContext ctx) : EndpointWithoutRequest<DeleteUserResponse>
{
    public override void Configure()
    {
        Delete("/user/delete/{id}");
        Roles("Admin");
    }

    public override async Task<DeleteUserResponse> ExecuteAsync(CancellationToken ct)
    {
        Guid userId = Route<Guid>("id", isRequired: true);

        DeleteUserData data = new(ctx);
        return await data.DeleteUserAsync(userId, ct);
    }
}