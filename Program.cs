using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using progressa.Data;
using progressa.Middleware;
using progressa.Models;
using progressa.Services;
using progressa.Services.IServices;
using Scalar.AspNetCore;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// =========================
// SERVICES
// =========================
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IActivitySessionService, ActivitySessionService>();
builder.Services.AddScoped<IGoalService, GoalService>();

// E-post (Azure Communication Services)
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Email"));
builder.Services.AddScoped<IEmailService, AzureEmailService>();

// =========================
// DATABAS
// =========================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

// =========================
// IDENTITY
// =========================
builder.Services
    .AddIdentityCore<ApplicationUser>(options =>
    {
        options.User.RequireUniqueEmail = true;

        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
    })
    .AddRoles<ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Återställningstoken gäller i 1 timme
builder.Services.Configure<DataProtectionTokenProviderOptions>(o =>
    o.TokenLifespan = TimeSpan.FromHours(1));

// =========================
// JWT
// =========================
var jwt = builder.Configuration.GetSection("Jwt");

var key = jwt["Key"]
    ?? throw new InvalidOperationException("Jwt:Key saknas i konfigurationen.");

builder.Services
    .AddAuthentication(options =>
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

            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),

            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// =========================
// RATE LIMITING
// =========================
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // 5 anrop per 15 minuter och IP-adress
    options.AddPolicy("password-reset", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(15),
                QueueLimit = 0
            }));
});

// =========================
// CORS (för React-frontend)
// =========================
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// =========================
// ROLLER VID UPPSTART
// =========================
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

    foreach (var roleName in new[] { "User", "GroupAdmin", "SuperAdmin" })
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            var result = await roleManager.CreateAsync(new ApplicationRole(roleName));

            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Kunde inte skapa rollen {roleName}: " +
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }
}


app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseRateLimiter();

app.UseCors("Frontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();