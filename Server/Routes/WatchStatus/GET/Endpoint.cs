using FastEndpoints;
using Server.Data;
using Server.Exceptions;

namespace Server.Routes.WatchStatus.GET;

public class GetWatchStatusAnimeEndpoint(AppDbContext ctx) : EndpointWithoutRequest<GetWatchStatusAnimeResponse>
{
    public override void Configure()
    {
        Get("/watch-status/{userId}/{status}");
        AllowAnonymous();
    }

    public override async Task<GetWatchStatusAnimeResponse> ExecuteAsync(CancellationToken ct)
    {
        Guid targetUserId = Route<Guid>("userId", isRequired: true);
        string status = Route<string>("status", isRequired: true) ?? "All";

        if (!status.Equals("All", StringComparison.OrdinalIgnoreCase)
            && !Enum.TryParse(status, true, out Enums.WatchStatus _))
            throw new BadRequestException("Invalid status value. Allowed values are: Planned, Watching, Completed, OnHold, Dropped.");
        
        GetWatchStatusAnimeData data = new(ctx);
        return await data.GetUserAnimeListAsync(targetUserId, status, ct);
    }
}