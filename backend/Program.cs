using backend.Features.Auth;
using backend.Features.Auth.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Supabase;
using Serilog;
using Serilog.Events;

// Bootstrap logger — captures startup errors before full config loads
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting IncomeApp backend");

    var builder = WebApplication.CreateBuilder(args);

    // Replace default .NET logging with Serilog, reading config from appsettings.json
    builder.Host.UseSerilog((context, services, configuration) =>
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
    );

    // ── Config / Environment Variables ──────────────────────────────────────────
    var supabaseUrl = Environment.GetEnvironmentVariable("SUPABASE_URL")
        ?? throw new InvalidOperationException("SUPABASE_URL not found in environment variables");
    var supabaseKey = Environment.GetEnvironmentVariable("SUPABASE_SERVICE_KEY")
        ?? throw new InvalidOperationException("SUPABASE_SERVICE_KEY not found in environment variables");

    var jwtKey = builder.Configuration["Jwt:Key"] ?? "super_secret_key_for_development_purpose_only_very_long_string";
    var backendUrl = builder.Configuration["Cors:BackendUrl"] ?? "http://localhost:5098";
    var frontendUrls = (builder.Configuration["Cors:FrontendUrl"] ?? "http://localhost:5173")
        .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
        .Select(url => url.Trim().TrimEnd('/'))
        .ToArray();
    // ────────────────────────────────────────────────────────────────────────────

    var supabaseOptions = new SupabaseOptions
    {
        AutoConnectRealtime = false
    };

    builder.Services.AddSingleton(provider => new Supabase.Client(supabaseUrl, supabaseKey, supabaseOptions));

    builder.Services.AddScoped<IFacebookLoginService, FacebookLoginService>();
    builder.Services.AddScoped<IncomeApp.Features.Financial.Services.IFinancialService, IncomeApp.Features.Financial.Services.FinancialService>();

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = backendUrl,
                ValidAudiences = frontendUrls,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
            };
        });

    builder.Services.AddAuthorization();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowFrontend", policy =>
        {
            policy.WithOrigins(frontendUrls)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
    });

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // Serilog request logging — replaces default ASP.NET Core request logs
    // Placed before CORS and auth to capture all requests including rejected ones
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value ?? string.Empty);
            diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);

            var userAgent = httpContext.Request.Headers.UserAgent.FirstOrDefault();
            if (!string.IsNullOrEmpty(userAgent))
                diagnosticContext.Set("UserAgent", (object)userAgent);

            var userId = httpContext.User?.FindFirst("sub")?.Value
                      ?? httpContext.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
                diagnosticContext.Set("UserId", userId);
        };
    });

    app.UseCors("AllowFrontend");
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapFacebookLoginEndpoint();
    app.MapControllers();

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
