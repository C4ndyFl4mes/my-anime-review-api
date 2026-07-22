# My Anime Review API

## Description
This API integrates with TenraiAPI to make an application that handles a review system for anime. Go to the [architecture-repo](https://github.com/C4ndyFl4mes/my-anime-review-architecture) for installation guide. The API is written in C# as an ASP.NET WebAPI application. Entity Framework is used to generate the database from the entities definied under /Server/Entities. Very basic Identity is used for authentication.

## Database
The database is PostgreSQL. This is the following entities/tables:

### Anime
Properties:
<ul>
    <li>Id</li>
    <li>Title</li>
    <li>ImageUrl</li>
    <li>TrailerUrl</li>
    <li>MalUrl</li>
    <li>Synopsis</li>
    <li>AgeRating</li>
    <li>AiringStatus</li>
    <li>TotalEpisodes</li>
    <li>Duration</li>
    <li>Season</li>
    <li>Year</li>
    <li>Source</li>
    <li>Type</li>
    <li>MetaDataJSON</li>
    <li>LastSynced</li>
    <li>Reviews</li>
    <li>WatchStatuses</li>
</ul>

### Following
Properties:
<ul>
    <li>FollowerUserId</li>
    <li>FollowedUserId</li>
    <li>CreatedAt</li>
    <li>FollowerUser</li>
    <li>FollowedUser</li>
</ul>

### Helpful
Properties:
<ul>
    <li>UserId</li>
    <li>ReviewId</li>
    <li>CreatedAt</li>
    <li>User</li>
    <li>Review</li>
</ul>

### ReportedBug
Properties:
<ul>
    <li>Id</li>
    <li>State</li>
    <li>Details</li>
    <li>CreatedAt</li>
</ul>

### ReportedReview
Properties:
<ul>
    <li>Id</li>
    <li>ReportedReviewId</li>
    <li>CreatedAt</li>
    <li>ReportedReview</li>
</ul>

### ReportedUser
Properties:
<ul>
    <li>Id</li>
    <li>ReportedUserId</li>
    <li>Reason</li>
    <li>CreatedAt</li>
    <li>ReportedUser</li>
</ul>

### Review
Properties:
<ul>
    <li>Id</li>
    <li>AnimeId</li>
    <li>UserId</li>
    <li>Text</li>
    <li>Score</li>
    <li>CreatedAt</li>
    <li>UpdatedAt</li>
    <li>Anime</li>
    <li>User</li>
    <li>HelpfulByUsers</li>
    <li>Reports</li>
</ul>

### Role
Properties:
<ul>
    <li>Id</li>
    <li>Name</li>
</ul>

### User
Properties:
<ul>
    <li>Id</li>
    <li>Email</li>
    <li>Username</li>
    <li>PasswordHash</li>
    <li>ProfileImageURL</li>
    <li>CreatedAt</li>
    <li>RefreshToken</li>
    <li>RefreshTokenExpiryTime</li>
    <li>RoleId</li>
    <li>Role</li>
    <li>Reviews</li>
    <li>HelpfulReviews</li>
    <li>Following</li>
    <li>Followers</li>
    <li>Reports</li>
    <li>WatchStatuses</li>
</ul>

### WatchStatus
Properties:
<ul>
    <li>UserId</li>
    <li>AnimeId</li>
    <li>EpisodesWatched</li>
    <li>Status</li>
    <li>UpdatedAt</li>
    <li>User</li>
    <li>Anime</li>
</ul>

## Endpoints
There's 33 endpoints and only 31 is used in the [frontend application](https://github.com/C4ndyFl4mes/my-anime-review-application). The ones that aren't used is GetCurrentProfileImage and BugReport/Put. The put endpoint wasn't used because of reducing development time dealing with keeping track of bug reports' states. 

All headers look the same and have application/json.

### BugReport
<table>
  <tr>
      <th>Method</th>
      <th>Endpoint</th>
      <th>Query</th>
      <th>Req</th>
      <th>Res</th>
      <th>Description</th>
  </tr>
  <tr>
    <td>GET</td>
        <td>/report/bugs</td>
    <td>
        <ul>
            <li>State</li>
            <li>Page</li>
        </ul>
    </td>
    <td>None</td>
    <td>GetBugReportsResponse</td>
    <td>Fetches paginated bug reports with state as filter.</td>
  </tr>
  <tr>
        <td>POST</td>
        <td>/report/bugs</td>
        <td>None</td>
        <td>PostBugReportRequest</td>
        <td>BugReportMessageResponse</td>
        <td>Creates a new bug report with pending state.</td>
    </tr>
    <tr>
        <td>PUT</td>
        <td>/report/bugs/{id}</td>
        <td>None</td>
        <td>None</td>
        <td>ChangeStateResponse</td>
        <td>Cycles a bug report to its next state.</td>
    </tr>
    <tr>
        <td>DELETE</td>
        <td>/report/bugs/{id}</td>
        <td>None</td>
        <td>None</td>
        <td>BugReportMessageResponse</td>
        <td>Deletes a bug report by id.</td>
  </tr>
</table>

### Feed
<table>
    <tr>
            <th>Method</th>
            <th>Endpoint</th>
            <th>Query</th>
            <th>Req</th>
            <th>Res</th>
            <th>Description</th>
    </tr>
    <tr>
        <td>GET</td>
        <td>/feed</td>
        <td>
                <ul>
                        <li>pageSize</li>
                        <li>page</li>
                </ul>
        </td>
        <td>None</td>
        <td>FeedResponse</td>
        <td>Gets paginated feed events from users you follow.</td>
    </tr>
</table>

### Follow
<table>
    <tr>
            <th>Method</th>
            <th>Endpoint</th>
            <th>Query</th>
            <th>Req</th>
            <th>Res</th>
            <th>Description</th>
    </tr>
    <tr>
        <td>POST</td>
        <td>/follow</td>
        <td>None</td>
        <td>FollowPostRequest</td>
        <td>FollowPostResponse</td>
        <td>Follows or unfollows a target user.</td>
    </tr>
</table>

### Profile
<table>
    <tr>
            <th>Method</th>
            <th>Endpoint</th>
            <th>Query</th>
            <th>Req</th>
            <th>Res</th>
            <th>Description</th>
    </tr>
    <tr>
        <td>GET</td>
        <td>/user/{userId}/profile</td>
        <td>None</td>
        <td>None</td>
        <td>GetProfileResponse</td>
        <td>Returns profile information and stats for a user.</td>
    </tr>
    <tr>
        <td>GET</td>
        <td>/user/profile-image/</td>
        <td>None</td>
        <td>None</td>
        <td>ProfileImageResponse</td>
        <td>Returns the authenticated user's profile image URL.</td>
    </tr>
    <tr>
        <td>PUT</td>
        <td>/user/profile-image/</td>
        <td>None</td>
        <td>ChangeProfileImageRequest</td>
        <td>ChangeProfileImageResponse</td>
        <td>Updates the authenticated user's profile image.</td>
    </tr>
</table>

### Review
<table>
    <tr>
            <th>Method</th>
            <th>Endpoint</th>
            <th>Query</th>
            <th>Req</th>
            <th>Res</th>
            <th>Description</th>
    </tr>
    <tr>
        <td>GET</td>
        <td>/anime/reviews/{malId}</td>
        <td>
                <ul>
                        <li>page</li>
                </ul>
        </td>
        <td>None</td>
        <td>ReviewResponse</td>
        <td>Gets paginated reviews for one anime.</td>
    </tr>
    <tr>
        <td>GET</td>
        <td>/user/{userId}/reviews/</td>
        <td>
                <ul>
                        <li>page</li>
                </ul>
        </td>
        <td>None</td>
        <td>ReviewExtendedResponse</td>
        <td>Gets paginated reviews written by a specific user.</td>
    </tr>
    <tr>
        <td>POST</td>
        <td>/review/{malId}</td>
        <td>None</td>
        <td>ReviewPostRequest</td>
        <td>ReviewPostResponse</td>
        <td>Creates a review for an anime.</td>
    </tr>
    <tr>
        <td>PUT</td>
        <td>/review/edit/{reviewId}</td>
        <td>None</td>
        <td>ReviewPostRequest</td>
        <td>ReviewPostResponse</td>
        <td>Updates an existing review.</td>
    </tr>
    <tr>
        <td>PUT</td>
        <td>/review/markashelpful</td>
        <td>None</td>
        <td>MarkAsHelpfulRequest</td>
        <td>MarkAsHelpfulResponse</td>
        <td>Marks or unmarks a review as helpful.</td>
    </tr>
    <tr>
        <td>DELETE</td>
        <td>/review/delete/{reviewId}</td>
        <td>None</td>
        <td>None</td>
        <td>ReviewPostResponse</td>
        <td>Deletes a review by id.</td>
    </tr>
</table>

### ReviewReport
<table>
    <tr>
            <th>Method</th>
            <th>Endpoint</th>
            <th>Query</th>
            <th>Req</th>
            <th>Res</th>
            <th>Description</th>
    </tr>
    <tr>
        <td>GET</td>
        <td>/report/reviews</td>
        <td>
                <ul>
                        <li>page</li>
                </ul>
        </td>
        <td>None</td>
        <td>GetReviewReportsResponse</td>
        <td>Gets paginated review reports.</td>
    </tr>
    <tr>
        <td>POST</td>
        <td>/report/reviews</td>
        <td>None</td>
        <td>PostReviewReportRequest</td>
        <td>ReviewReportResponse</td>
        <td>Creates a report for a review.</td>
    </tr>
    <tr>
        <td>DELETE</td>
        <td>/report/reviews/{id}</td>
        <td>None</td>
        <td>None</td>
        <td>ReviewReportResponse</td>
        <td>Deletes a review report by id.</td>
    </tr>
</table>

### Tenrai
<table>
    <tr>
            <th>Method</th>
            <th>Endpoint</th>
            <th>Query</th>
            <th>Req</th>
            <th>Res</th>
            <th>Description</th>
    </tr>
    <tr>
        <td>GET</td>
        <td>/anime/search</td>
        <td>
                <ul>
                        <li>q</li>
                        <li>page</li>
                </ul>
        </td>
        <td>None</td>
        <td>TenraiSearchResponse</td>
        <td>Searches anime and returns paginated results.</td>
    </tr>
    <tr>
        <td>GET</td>
        <td>/anime/inspect/{malId}</td>
        <td>None</td>
        <td>None</td>
        <td>Anime</td>
        <td>Returns detailed information for one anime.</td>
    </tr>
</table>

### User
<table>
    <tr>
            <th>Method</th>
            <th>Endpoint</th>
            <th>Query</th>
            <th>Req</th>
            <th>Res</th>
            <th>Description</th>
    </tr>
    <tr>
        <td>POST</td>
        <td>/user/signup</td>
        <td>None</td>
        <td>UserDto</td>
        <td>SignInResponse</td>
        <td>Registers a new user and sets auth cookies.</td>
    </tr>
    <tr>
        <td>POST</td>
        <td>/user/signin</td>
        <td>None</td>
        <td>SignInDto</td>
        <td>SignInResponse</td>
        <td>Signs in a user and sets auth cookies.</td>
    </tr>
    <tr>
        <td>POST</td>
        <td>/user/refresh</td>
        <td>None</td>
        <td>None</td>
        <td>RefreshResponse</td>
        <td>Refreshes access and refresh tokens from cookies.</td>
    </tr>
    <tr>
        <td>GET</td>
        <td>/user/is-authenticated</td>
        <td>None</td>
        <td>None</td>
        <td>UserAuthenticationStatus</td>
        <td>Returns current authentication state and role flag.</td>
    </tr>
    <tr>
        <td>DELETE</td>
        <td>/user/delete/{id}</td>
        <td>None</td>
        <td>None</td>
        <td>DeleteUserResponse</td>
        <td>Deletes a user account by id (admin only).</td>
    </tr>
</table>

### UserReport
<table>
    <tr>
            <th>Method</th>
            <th>Endpoint</th>
            <th>Query</th>
            <th>Req</th>
            <th>Res</th>
            <th>Description</th>
    </tr>
    <tr>
        <td>GET</td>
        <td>/report/users</td>
        <td>
                <ul>
                        <li>page</li>
                </ul>
        </td>
        <td>None</td>
        <td>GetUserReportsResponse</td>
        <td>Gets paginated user reports.</td>
    </tr>
    <tr>
        <td>POST</td>
        <td>/report/users</td>
        <td>None</td>
        <td>PostUserReportRequest</td>
        <td>UserReportResponse</td>
        <td>Creates a report for a user.</td>
    </tr>
    <tr>
        <td>DELETE</td>
        <td>/report/users/{id}</td>
        <td>None</td>
        <td>None</td>
        <td>UserReportResponse</td>
        <td>Deletes a user report by id.</td>
    </tr>
</table>

### WatchStatus
<table>
    <tr>
            <th>Method</th>
            <th>Endpoint</th>
            <th>Query</th>
            <th>Req</th>
            <th>Res</th>
            <th>Description</th>
    </tr>
    <tr>
        <td>GET</td>
        <td>/watch-status/{userId}/{status}</td>
        <td>None</td>
        <td>None</td>
        <td>GetWatchStatusAnimeResponse</td>
        <td>Gets a user's anime list filtered by watch status.</td>
    </tr>
    <tr>
        <td>GET</td>
        <td>/watch-status/{malId}</td>
        <td>None</td>
        <td>None</td>
        <td>GetAnimeWatchStatusResponse</td>
        <td>Gets the authenticated user's watch status for one anime.</td>
    </tr>
    <tr>
        <td>POST</td>
        <td>/watch-status/{malId}</td>
        <td>None</td>
        <td>PostWatchStatusRequest</td>
        <td>WatchStatusResponse</td>
        <td>Adds or replaces a watch status entry for an anime.</td>
    </tr>
    <tr>
        <td>PUT</td>
        <td>/watch-status/{malId}</td>
        <td>None</td>
        <td>PostWatchStatusRequest</td>
        <td>WatchStatusResponse</td>
        <td>Updates an existing watch status entry.</td>
    </tr>
    <tr>
        <td>DELETE</td>
        <td>/watch-status/{malId}</td>
        <td>None</td>
        <td>None</td>
        <td>WatchStatusResponse</td>
        <td>Removes a watch status entry for an anime.</td>
    </tr>
</table>
