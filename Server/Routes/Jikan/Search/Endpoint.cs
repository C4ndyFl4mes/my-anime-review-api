using FastEndpoints;
using Server.Data;
using Server.Utils;

namespace Server.Routes.Jikan.Search;

/// <summary>
/// Endpoint for searching anime using Jikan API. Supports optional query parameter 'q' for search term and 'page' for pagination.
/// </summary>
public class SearchEndpoint(AppDbContext ctx) : EndpointWithoutRequest<JikanSearchResponse>
{
    public override void Configure()
    {
        Get("/anime/search");
        AllowAnonymous();
    }

    public override async Task<JikanSearchResponse> ExecuteAsync(CancellationToken ct)
    {
        (string? q, int page) = (Query<string?>("q", isRequired: false), Query<int>("page", isRequired: false) > 0 ? Query<int>("page", isRequired: false) : 1);

        Guid currentUserId = UserUtils.GetAuthenticatedUserID();

        JikanSearchData data = new(ctx);
        return await data.SearchAnimeAsync(q, page, currentUserId, ct);
    }
}