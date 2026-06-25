using FastEndpoints;
using Server.Utils;

namespace Server.Routes.User.IsLoggedIn;

public class IsAdminEndpoint : EndpointWithoutRequest<IsAdminResponse>
{
    public override void Configure()
    {
        Post("/user/is-admin");
        AllowAnonymous();
    }

    public override async Task<IsAdminResponse> ExecuteAsync(CancellationToken ct)
    {
        return new IsAdminResponse
        {
            IsAdmin = "Admin" == UserUtils.GetAuthenticatedUserRole()
        };
    }
}