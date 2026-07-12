using FastEndpoints;
using Microsoft.Extensions.Caching.Memory;
using Server.Data;
using Server.Utils;

namespace Server.Routes.Tenrai.Search;

/// <summary>
/// Endpoint for searching anime using Tenrai API. Supports optional query parameter 'q' for search term and 'page' for pagination.
/// </summary>
public class SearchEndpoint(AppDbContext ctx, IMemoryCache cache) : EndpointWithoutRequest<TenraiSearchResponse>
{
    public override void Configure()
    {
        Get("/anime/search");
        AllowAnonymous();
    }

    public override async Task<TenraiSearchResponse> ExecuteAsync(CancellationToken ct)
    {
        (string? q, int page) = (Query<string?>("q", isRequired: false), Query<int>("page", isRequired: false) > 0 ? Query<int>("page", isRequired: false) : 1);

        Guid currentUserId = UserUtils.GetAuthenticatedUserID();

        TenraiSearchData data = new(ctx, cache);
        return await data.SearchAnimeAsync(q, page, currentUserId, ct);
    }
}
