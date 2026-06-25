namespace Server.Routes.User;

/// <summary>
/// Data Transfer Object for user-related operations. Contains the necessary information for signing up a new user, including email, username, password, and an optional profile image URL.
/// </summary>
public record UserDto
{
    public required string Email { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public string? ProfileImageURL { get; set; }

}

/// <summary>
/// Data Transfer Object for signing in a user. Contains the user's email and password. This is used to authenticate the user and generate a token for subsequent requests.
/// </summary>
public record SignInDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}

/// <summary>
/// The token will not be sent in the response object, but as a cookie. This is just for internal use to pass the token from the logic to the endpoint, so it can be set as a cookie in the response.
/// </summary>
public record SignInResponse
{
    public string Username { get; set; } = string.Empty;
    public string ProfileImageURL { get; set; } = string.Empty;
    public string? AccessToken { get; set; } = null;
    public string? RefreshToken { get; set; } = null;
}

public record DeleteUserResponse
{
    public string Message { get; set; } = string.Empty;
}

public record IsAdminResponse
{
    public bool IsAdmin { get; set; } = false;
}