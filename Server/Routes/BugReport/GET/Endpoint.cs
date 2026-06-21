using FastEndpoints;
using Server.Data;
using Server.Enums;
using Server.Exceptions;

namespace Server.Routes.BugReport.GET;

public class GetBugReportsEndpoint(AppDbContext ctx) : EndpointWithoutRequest<GetBugReportsResponse>
{
    public override void Configure()
    {
        Get("/report/bugs");
        Roles("Admin");
    }

    public override async Task<GetBugReportsResponse> ExecuteAsync(CancellationToken ct)
    {
        string? state = Query<string>("state", isRequired: false); // When state is null, it will list all bug reports regardless of their state.
        int page = Query<int>("page", isRequired: false);
        if (page < 1)
            page = 1;
        
        if (!Enum.TryParse(state, true, out BugState parsedState) && state is not null)
            throw new BadRequestException("Invalid state value. Allowed values are: Pending, Planned, InProgress, Completed, Rejected.");
        
        GetBugReportsData data = new(ctx);
        return await data.GetBugReportsAsync(state is not null ? parsedState : null, page, ct);
    }
}