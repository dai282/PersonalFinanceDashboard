using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PersonalFinance.API.Services;
using PersonalFinance.Core.Interfaces;
using PersonalFinance.Infrastructure.Data;
using PersonalFinance.Infrastructure.Repositories;
using System.Text;
using System.Text.Json;



var builder = WebApplication.CreateBuilder(args);

//Aspire service defaults.
builder.AddServiceDefaults();

// --- START INSERT: AWS Secrets Manager Logic ---

// 1. Get the Default connection string (Local/Aspire)
string connectionString = builder.Configuration.GetConnectionString("PersonalFinanceDB");

// 2. Check if we are running in AWS (The CDK passes these variables)
var dbSecretArn = Environment.GetEnvironmentVariable("DB_SECRET_ARN");
var dbHost = Environment.GetEnvironmentVariable("DB_HOST");

if (!string.IsNullOrEmpty(dbSecretArn) && !string.IsNullOrEmpty(dbHost))
{
    try
    {
        Console.WriteLine($"Fetching secret for database from ARN: {dbSecretArn}...");

        // Initialize AWS Client (It uses the Task Role automatically)
        var secretsClient = new AmazonSecretsManagerClient();

        // Fetch the secret
        var response = await secretsClient.GetSecretValueAsync(new GetSecretValueRequest
        {
            SecretId = dbSecretArn
        });

        if (!string.IsNullOrEmpty(response.SecretString))
        {
            // Parse the JSON secret {"username": "...", "password": "..."}
            using var secretDoc = JsonDocument.Parse(response.SecretString);
            var root = secretDoc.RootElement;
            var username = root.GetProperty("username").GetString();
            var password = root.GetProperty("password").GetString();

            // Build the production connection string
            // Note: TrustServerCertificate=true is often needed for RDS internal connections unless you install the AWS CA certs
            connectionString = $"Server={dbHost};Database=PersonalFinanceDB;User Id={username};Password={password};TrustServerCertificate=true";

            Console.WriteLine("Successfully retrieved database credentials from Secrets Manager.");
        }
    }
    catch (Exception ex)
    {
        // Log this critically - if this fails, the app cannot start
        Console.WriteLine($"CRITICAL ERROR fetching DB Secret: {ex.Message}");
        throw;
    }
}
// --- END INSERT ---

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString,
    //retry on failure for transient faults
    sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null
        )
    ));



builder.Services.AddControllers();
//Register AuthService
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IBudgetRepository, BudgetRepository>();
builder.Services.AddScoped<IReportsRepository, ReportsRepository>();

//Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]!))
    };
});

builder.Services.AddAuthorization();

//Add CORS to allow requests from frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.SetIsOriginAllowed(origin =>
        {
            if (string.IsNullOrWhiteSpace(origin)) return false;

            //allow localhost for development
            if (origin.StartsWith("http://localhost") || origin.StartsWith("https://localhost"))
                return true;

            // Allow Vercel deployment
            if (origin.EndsWith(".vercel.app"))
                return true;

            return false;
        })
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });

    options.AddPolicy("AllowVercel", policy =>
    {
        policy.WithOrigins("https://personal-finance-dashboard-bjk7f7iv3-dais-projects-9ac44d04.vercel.app") // Your Vercel Domain
              .AllowAnyHeader()
              .AllowAnyMethod();
    });

});

var app = builder.Build();

// Map Aspire default endpoints (add before app.Run())
app.MapDefaultEndpoints();

app.UseCors("AllowFrontend");
app.UseCors("AllowVercel");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Seed default categories, added wait logic to ensure DB is ready
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();

        // Wait for database to be ready.
        //var maxAttempts = 10;
        //var delay = TimeSpan.FromSeconds(3);

        //for (int i = 0; i < maxAttempts; i++)
        //{
        //    try
        //    {
        //        logger.LogInformation("Attempting to connect to database... (Attempt {Attempt}/{MaxAttempts})", i + 1, maxAttempts);
        //        await context.Database.CanConnectAsync();
        //        logger.LogInformation("Database connection successful!");
        //        break;
        //    }
        //    catch (Exception ex)
        //    {
        //        if (i == maxAttempts - 1)
        //        {
        //            logger.LogError(ex, "Failed to connect to database after {MaxAttempts} attempts", maxAttempts);
        //            throw;
        //        }
        //        logger.LogWarning("Database not ready, waiting {Delay} seconds...", delay.TotalSeconds);
        //        await Task.Delay(delay);
        //    }
        //}

        // Run migrations automatically
        logger.LogInformation("Running database migrations...");
        // This will create the database if it doesn't exist
        await context.Database.MigrateAsync();

        // Seed data
        logger.LogInformation("Seeding default data...");
        await DbSeeder.SeedDefaultCategories(context);

        logger.LogInformation("Database initialization completed successfully!");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
        throw;
    }
}

//app.UseHttpsRedirection();
app.MapGet("/health", () => Results.Ok("Healthy"));

app.Run();