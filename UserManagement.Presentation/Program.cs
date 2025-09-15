using Asp.Versioning;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using UserManagement.API;
using UserManagement.API.Hubs;
using UserManagement.Application.RepositoryContracts;
using UserManagement.Application.ServiceContracts;
using UserManagement.Application.Services;
using UserManagement.Domain.Entities.Identity;
using UserManagement.Infrastructure.CustomIdentity;
using UserManagement.Infrastructure.DbContexts;
using UserManagement.Infrastructure.RepositoryServices;
using UserManagement.Infrastructure.Services;
using UserManagement.Presentation.StartUpConfigurations;

var builder = WebApplication.CreateBuilder(args);


// Define a CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularClient", policy =>
    {
        policy.WithOrigins(builder.Configuration.GetSection("AllowOrigins").Get<string[]>()!)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services
    .AddControllers(opt =>
    {
        var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
        opt.Filters.Add(new AuthorizeFilter(policy));
    })
    .AddApplicationPart(typeof(AssemblyMarker).Assembly);

//Real-time features
builder.Services.AddSignalR();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

//versioning

builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true; // Include API versions in response headers
    options.AssumeDefaultVersionWhenUnspecified = true; // Assume default version if none specified
    options.DefaultApiVersion = new ApiVersion(1, 0); // Set default API version
    options.ApiVersionReader = new UrlSegmentApiVersionReader(); // Read version from URL segment
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV"; // Format the version as 'v'major[.minor][-status]
    options.SubstituteApiVersionInUrl = true; // Substitute the version in the URL
});

// swagger

var xmlPath = Path.Combine(Directory.GetCurrentDirectory(),
    builder.Configuration.GetSection("ApiDocumentation").GetValue<string>("XmlFilePath") ?? "");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
    }
    // Add this if you want to support multiple versions in Swagger
    c.SwaggerDoc("v1", new OpenApiInfo()
    {
        Title = "User Management API",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
{
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        },
        Array.Empty<string>()
    }
});

});


// register the publisher (below)
builder.Services.AddScoped<IUserPublisher, SignalRUserPublisher>();

builder.Services.AddTransient<IJwtService, JwtService>();
builder.Services.AddTransient<IEmailSender, EmailSender>();


builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IAdminService, AdminService>();

builder.Services.AddScoped<ILogRepo, LogRepo>();
builder.Services.AddScoped<ILogService, LogService>();


// logging
builder.Services.AddLogging();

// Add authentication

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;

}).AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders()
.AddTokenProvider<EmailConfirmationTokenProvider<ApplicationUser>>("EmailConfirmation")
.AddUserStore<UserStore<ApplicationUser, ApplicationRole, AppDbContext, Guid>>()
.AddRoleStore<RoleStore<ApplicationRole, AppDbContext, Guid>>()
.AddRoleManager<RoleManager<ApplicationRole>>()
.AddUserManager<UserManager<ApplicationUser>>();

builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, CustomUserClaimsPrincipalFactory>();

builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromMinutes(3);
});


builder.Services.Configure<EmailConfirmationTokenProviderOptions>(options =>
{
    // Specific for email confirmation
    options.TokenLifespan = TimeSpan.FromMinutes(10);
});
//Jwt
builder.Services.AddAuthentication(opts =>
{
    opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

    opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;


}).AddJwtBearer(opts =>
{
    opts.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
        RoleClaimType = ClaimTypes.Role

    };

    opts.Events = new JwtBearerEvents
    {
        OnMessageReceived = ctx =>
        {
            var accessToken = ctx.Request.Query["access_token"];

            // If the request is for our hub
            var path = ctx.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) &&
                path.StartsWithSegments("/api/v1/admin/hubs/users"))
            {
                ctx.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHsts();

app.UseHttpsRedirection();
app.UseStaticFiles();

// Use the CORS policy

app.UseRouting();
app.UseCors("AllowAngularClient");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<UserHub>("/api/v1/admin/hubs/users");
//swagger

app.UseSwagger();
app.UseSwaggerUI(c =>
{

    c.SwaggerEndpoint("/swagger/v1/swagger.json", "1.0");
});

app.Run();
