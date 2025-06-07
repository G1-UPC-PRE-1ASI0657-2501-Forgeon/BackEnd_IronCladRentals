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
using RentalService.RentalBounded.Application.Internal.CommandServices;
using RentalService.RentalBounded.Application.Internal.QueryServices;
using RentalService.RentalBounded.Domain.Repositories;
using RentalService.RentalBounded.Domain.Services;
using RentalService.RentalBounded.Infraestructure.Persistence.EFC;
using RentalService.RentalBounded.Infraestructure.Persistence.EFC.Repositories;
using Swashbuckle.AspNetCore.SwaggerUI;


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

builder.Services.AddScoped<IRentalRepository, RentalRepository >();
builder.Services.AddScoped<IRentalQueryService, RentalQueryService >();
builder.Services.AddScoped<IRentalCommandService, RentalCommandService >();
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
        Title = "Rental API",
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

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
}

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
