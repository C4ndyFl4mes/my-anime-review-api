using System.Text;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Server.Data;
using Server.Entities;
using Server.Exceptions;
using Server.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Load secrets from either /run/secrets or .secrets in the project root.
if (Directory.Exists("/run/secrets"))
{
    builder.Configuration.AddKeyPerFile("/run/secrets", optional: true);
}
else
{
    string contentRoot = builder.Environment.ContentRootPath;
    if (Directory.Exists(Path.GetFullPath(Path.Combine(contentRoot, "..", ".secrets"))))
    {
        builder.Configuration.AddKeyPerFile(Path.GetFullPath(Path.Combine(contentRoot, "..", ".secrets")), optional: true);
        Console.WriteLine("Loaded secrets from the .secrets directory.");
    }
    else
    {
        throw new InvalidOperationException("Secrets directory not found. Please ensure that either /run/secrets or .secrets in project root exists.");
    }
}

// Load the database connection string from secrets and set it in the configuration.
string? dbConnectionString = builder.Configuration["db_connection_string.txt"];
if (!string.IsNullOrEmpty(dbConnectionString))
{
    builder.Configuration["ConnectionStrings:DefaultConnection"] = dbConnectionString;
    Console.WriteLine("Database connection string loaded from secrets.");
}
else
{
    throw new InvalidOperationException("Database connection string not found in secrets. Please ensure that db_connection_string.txt exists in the secrets directory.");
}

// Register the AppDbContext with the connection string from configuration.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddAuthorization();

string secretKey = builder.Configuration["secret_key.txt"] ?? throw new InvalidOperationException("Secret key is not configured.");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["issuer.txt"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["audience.txt"],
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ValidateIssuerSigningKey = true
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Cookies.TryGetValue("accessToken", out string? accessToken))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

// Register services.
builder.Services.AddScoped<TokenService>();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddFastEndpoints().SwaggerDocument();

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendDev", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173",
                "https://localhost:5173",
                "https://myanimereview.se",
                "https://www.myanimereview.se"
              )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddMemoryCache();

WebApplication app = builder.Build();

// Seed the database with initial data if necessary.
using (IServiceScope scope = app.Services.CreateScope())
{
    AppDbContext ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await ctx.Database.MigrateAsync();

    if (!await ctx.Roles.AnyAsync())
    {
        ctx.Roles.AddRange
        (
            new RoleEntity { Id = Guid.NewGuid(), Name = "Admin" },
            new RoleEntity { Id = Guid.NewGuid(), Name = "User" }
        );
        await ctx.SaveChangesAsync();
    }

    if (!await ctx.Users.AnyAsync())
    {
        string adminName = builder.Configuration["admin_username.txt"] ?? throw new InvalidOperationException("Admin username not found in configuration.");
        string adminEmail = builder.Configuration["admin_email.txt"] ?? throw new InvalidOperationException("Admin email not found in configuration.");
        string adminPassword = builder.Configuration["admin_password.txt"] ?? throw new InvalidOperationException("Admin password not found in configuration.");
        Guid adminRoleId = await ctx.Roles.Where(r => r.Name == "Admin").Select(r => r.Id).FirstOrDefaultAsync();

        UserEntity adminUser = new()
        {
            Id = Guid.NewGuid(),
            Username = adminName,
            Email = adminEmail,
            PasswordHash = new PasswordHasher<UserEntity>().HashPassword(null!, adminPassword),
            RoleId = adminRoleId,
            Role = null! // This will be set by EF Core when the user is added to the database due to the foreign key relationship.
        };

        ctx.Users.Add(adminUser);
        await ctx.SaveChangesAsync();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<GlobalExceptionHandler>();

app.UseHttpsRedirection();

app.UseCors("FrontendDev");

app.UseAuthentication();
app.UseAuthorization();

app.UseFastEndpoints().UseSwaggerGen();

app.Run();
