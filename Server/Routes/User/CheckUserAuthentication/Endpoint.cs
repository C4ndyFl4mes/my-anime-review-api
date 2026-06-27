using FastEndpoints;
using Server.Utils;

namespace Server.Routes.User.CheckUserAuthentication;

public class CheckUserAuthenticationEndpoint : EndpointWithoutRequest<UserAuthenticationStatus>
{
    public override void Configure()
    {
        Get("/user/is-authenticated");
        AllowAnonymous();
    }

    public override async Task<UserAuthenticationStatus> ExecuteAsync(CancellationToken ct)
    {
        return new UserAuthenticationStatus
        {
            UserId = UserUtils.GetAuthenticatedUserID(),
            IsAdmin = "Admin" == UserUtils.GetAuthenticatedUserRole()
        };
    }
}