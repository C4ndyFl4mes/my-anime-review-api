using FastEndpoints;
using FastEndpoints.Swagger;

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

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddFastEndpoints().SwaggerDocument();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseHttpsRedirection();

app.UseFastEndpoints().UseSwaggerGen();

app.Run();
