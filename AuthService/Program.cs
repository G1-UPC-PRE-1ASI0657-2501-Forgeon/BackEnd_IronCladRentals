using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using AuthService.User.Application.Internal.CommandServices;
using AuthService.User.Application.Internal.OutboundServices;
using AuthService.User.Application.Internal.QueryServices;
using AuthService.User.Domain.Repositories;
using AuthService.User.Domain.Services;
using AuthService.User.Infraestructure.Hashing.BCrypt.Services;
using AuthService.User.Infraestructure.Persistence.EFC;
using AuthService.User.Infraestructure.Persistence.EFC.Repositories;
using AuthService.User.Infraestructure.Tokens.JWT.Configurations;
using AuthService.User.Infraestructure.Tokens.JWT.Services;
using IronClead.SharedKernel.Shared.Domain.Repositories;
using IronClead.SharedKernel.Shared.Infraestructure.Persistences.EFC.Repositories;


var builder = WebApplication.CreateBuilder(args);

var jwtSettings = builder.Configuration.GetSection("TokenSettings");
builder.Services.Configure<TokenSettings>(jwtSettings);

var secretKey = jwtSettings["Secret"];
if (string.IsNullOrEmpty(secretKey))
    throw new ArgumentNullException(nameof(secretKey), "JWT secret no configurado.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            NameClaimType = ClaimTypes.NameIdentifier, 
            RoleClaimType = ClaimTypes.Role            
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

// 📦 MySQL
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

// 🔄 Repositorios y Servicios
builder.Services.AddScoped<IAuthUserRepository, AuthUserRepository>();
builder.Services.AddScoped<IAuthUserRefreshTokenRepository, AuthUserRefreshTokenRepository>();
builder.Services.AddScoped<IAuthUserCommandService, AuthUserCommandService>();
builder.Services.AddScoped<IAuthUserQueryService, AuthUserQueryService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IHashingService, HashingService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork<AppDbContext>>();

// 🌐 CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // o tu frontend real
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// 🍪 Cookies seguras
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

// 📚 Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "AuthService API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "JWT con formato: Bearer {token}",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// 🔧 MVC
builder.Services.AddControllers();

var app = builder.Build();

// 🔁 DB EnsureCreated (solo desarrollo)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// 🔄 Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
