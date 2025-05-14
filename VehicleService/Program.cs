using System.Text;
using IronClead.SharedKernel.Shared.Domain.Repositories;
using IronClead.SharedKernel.Shared.Infraestructure.Interfaces.ASP.Configuration;
using IronClead.SharedKernel.Shared.Infraestructure.Persistences.EFC.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerUI;
using VehicleService.VehicleBounded.Application.Internal.CommandServices;
using VehicleService.VehicleBounded.Application.Internal.QueryServices;
using VehicleService.VehicleBounded.Domain.Repositories;
using VehicleService.VehicleBounded.Domain.Services;
using VehicleService.VehicleBounded.Infraestructure.Persistence.EFC;
using VehicleService.VehicleBounded.Infraestructure.Persistence.EFC.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = "Bearer";
        options.DefaultChallengeScheme = "Bearer";
    })
    .AddJwtBearer("Bearer", options =>
    {
        var secret = builder.Configuration["TokenSettings:Secret"];
        if (string.IsNullOrEmpty(secret))
        {
            throw new ArgumentNullException(nameof(secret), "JWT Secret is not configured in appsettings.json.");
        }

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Cookies.ContainsKey("AuthToken"))
                {
                    context.Token = context.Request.Cookies["AuthToken"];
                }
                return Task.CompletedTask;
            }
        };
    });


builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
        .LogTo(Console.WriteLine, LogLevel.Information)
        .EnableSensitiveDataLogging(builder.Environment.IsDevelopment())
        .EnableDetailedErrors(builder.Environment.IsDevelopment());
});

builder.Services.AddScoped<IVehicleRepository, VehicleRepository >();
builder.Services.AddScoped<IBrandRepository, BrandRepository >();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository >();
builder.Services.AddScoped<ILocationRepository, LocationRepository >();
builder.Services.AddScoped<IModelRepository, ModelRepository >();
builder.Services.AddScoped<IPricingRepository, PricingRepository >();
builder.Services.AddScoped<IVehicleQueryService, VehicleQueryService >();
builder.Services.AddScoped<IBrandQueryService, BrandQueryService >();
builder.Services.AddScoped<ICompanyQueryService, CompanyQueryService >();
builder.Services.AddScoped<ILocationQueryService, LocationQueryService >();
builder.Services.AddScoped<IModelQueryService, ModelQueryService >();
builder.Services.AddScoped<IPricingQueryService, PricingQueryService >();
builder.Services.AddScoped<IVehicleCommandService, VehicleCommandService >();
builder.Services.AddScoped<IBrandCommandService, BrandCommandService >();
builder.Services.AddScoped<ICompanyCommandService, CompanyCommandService >();
builder.Services.AddScoped<ILocationCommandService, LocationCommandService >();
builder.Services.AddScoped<IModelCommandService, ModelCommandService >();
builder.Services.AddScoped<IPricingCommandService, PricingCommandService >();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork<AppDbContext>>();


builder.Services.AddControllers(options =>
{
        options.Conventions.Add(new KebabCaseRouteNamingConvention());
});

// Configurar opciones de enrutamiento
builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",policy =>
    {
        policy.WithOrigins("http://localhost:5173")  
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();  
    });
});



builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true; // 🔹 Evita acceso JS
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // 🔹 Solo HTTPS en producción
    options.Cookie.SameSite = SameSiteMode.None; // 🔹 Necesario para Swagger y CORS
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "VehicleService API",
        Version = "v1",
        Description = "Plantita Platform API",
        TermsOfService = new Uri("http://localhost:5000/swagger/index.html"),
        Contact = new OpenApiContact
        {
            Name = "Forgeon",
            Email = "erickpalomino0723@gmail.com"
        },
        License = new OpenApiLicense
        {
            Name = "Apache 2.0",
            Url = new Uri("https://www.apache.org/licenses/LICENSE-2.0.html")
        }
    });

    // 🔹 Definir el esquema de autenticación JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Ingrese el token JWT con el prefijo 'Bearer '",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            Array.Empty<string>()
        }
    });
});



var app = builder.Build();

// Log Server Addresses
var serverAddresses = app.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>();


foreach (var address in serverAddresses.Addresses)
{
    Console.WriteLine($"Listening on {address}");
}

// Ensure Database is Created
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
}

// Configure Middleware Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SupportedSubmitMethods(new[] { SubmitMethod.Get, SubmitMethod.Post });
        c.ConfigObject.AdditionalItems["withCredentials"] = true;
    });}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
