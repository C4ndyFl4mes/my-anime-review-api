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
        string state = Query<string>("state", isRequired: true) ?? "All";
        int page = Query<int>("page", isRequired: false);
        if (page < 1)
            page = 1;

        bool hasState = !state.Equals("All", StringComparison.OrdinalIgnoreCase);
        BugState parsedState = default;

        if (hasState && !Enum.TryParse(state, true, out parsedState))
            throw new BadRequestException("Invalid state value. Allowed values are: Pending, Planned, InProgress, Completed, Rejected.");
        
        GetBugReportsData data = new(ctx);
        return await data.GetBugReportsAsync(hasState ? parsedState : null, page, ct);
    }
}