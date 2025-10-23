using DotNetEnv;
using EventLink.Hubs;
using EventLink_Repositories.DBContext;
using EventLink_Repositories.Interface;
using EventLink_Repositories.Repository;
using Eventlink_Services.Interface;
using Eventlink_Services.Service;
using EventLink_Services.Services.Implementations;
using EventLink_Services.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Text;
using System.Threading.Tasks;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration["ConnectionStrings:DefaultConnection"] = Environment.GetEnvironmentVariable("DB_CONNECTION");
builder.Configuration["JwtSettings:SecretKey"] = Environment.GetEnvironmentVariable("JWT_SECRET");
builder.Configuration["JwtSettings:Issuer"] = Environment.GetEnvironmentVariable("JWT_ISSUER");
builder.Configuration["JwtSettings:Audience"] = Environment.GetEnvironmentVariable("JWT_AUDIENCE");
builder.Configuration["JwtSettings:ExpireHours"] = Environment.GetEnvironmentVariable("JWT_EXPIRE_HOURS");
builder.Configuration["EmailSettings:SmtpServer"] = Environment.GetEnvironmentVariable("EMAIL_SMTP");
builder.Configuration["EmailSettings:Port"] = Environment.GetEnvironmentVariable("EMAIL_PORT");
builder.Configuration["EmailSettings:SenderName"] = Environment.GetEnvironmentVariable("EMAIL_SENDER_NAME");
builder.Configuration["EmailSettings:SenderEmail"] = Environment.GetEnvironmentVariable("EMAIL_SENDER_EMAIL");
builder.Configuration["EmailSettings:Username"] = Environment.GetEnvironmentVariable("EMAIL_USERNAME");
builder.Configuration["EmailSettings:Password"] = Environment.GetEnvironmentVariable("EMAIL_PASSWORD");
builder.Configuration["CLOUDINARY_CLOUD_NAME"] = Environment.GetEnvironmentVariable("CLOUDINARY_CLOUD_NAME");
builder.Configuration["CLOUDINARY_API_KEY"] = Environment.GetEnvironmentVariable("CLOUDINARY_API_KEY");
builder.Configuration["CLOUDINARY_API_SECRET"] = Environment.GetEnvironmentVariable("CLOUDINARY_API_SECRET");
builder.Configuration["GROQ_API_KEY"] = Environment.GetEnvironmentVariable("GROQ_API_KEY");

// Add services to the container
builder.Services.AddControllers();

// Database Configuration
builder.Services.AddDbContext<EventLinkDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
});

// JWT Configuration
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET");

if (string.IsNullOrEmpty(secretKey))
{
    throw new ArgumentException("JWT SecretKey is not configured");
}

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
        ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
        ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };

    // 🔥 Enable JWT for SignalR
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chatHub"))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };
});

// Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("OrganizerOnly", policy => policy.RequireClaim("Role", "Organizer"));
    options.AddPolicy("SupplierOnly", policy => policy.RequireClaim("Role", "Supplier"));
    options.AddPolicy("SponsorOnly", policy => policy.RequireClaim("Role", "Sponsor"));
    options.AddPolicy("EmailVerified", policy => policy.RequireClaim("EmailVerified", "True"));
});

builder.Services.AddSignalR();

// ✅ Register SignalR notification service (từ EventLink.Services namespace)
builder.Services.AddScoped<INotificationService, SignalRNotificationService>();


// Repository Registration
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<ISponsorPackageRepo, SponsorPackageRepo>();
builder.Services.AddScoped<IUserProfileRepo, UserProfileRepo>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IEventProposalRepository, EventProposalRepository>();
builder.Services.AddScoped<IPartnershipRepository, PartnershipRepository>();
builder.Services.AddScoped<ISupplierServiceRepository, SupplierServiceRepository>();
builder.Services.AddScoped<IEventActivityRepository, EventActivityRepository>();

// Service Registration
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IGoogleAuthService, GoogleAuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ISponsorPackageService, SponsorPackageService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserProfileService, UserProfileService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IEventProposalService, EventProposalService>();
builder.Services.AddScoped<IPartnershipService, PartnershipService>();
builder.Services.AddScoped<ISupplierServiceService, SupplierServiceService>();
builder.Services.AddScoped<IMessageService, MessageService>();

builder.Services.AddSingleton<CloudinaryService>();
builder.Services.AddSingleton<OpenAIService>();

builder.Services.AddMemoryCache();

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", builder =>
    {
        builder
            .WithOrigins(
                "http://localhost:3000",
                "https://localhost:3000",
                "http://localhost:5173",
                "https://localhost:5173"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials(); // Important for SignalR
    });
});

// API Documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "EventLink API",
        Version = "v1",
        Description = "EventLink Authentication API for connecting Organizers, Suppliers, and Sponsors"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "EventLink API v1");
    });
}

app.UseHttpsRedirection();

// CORS must be before Authentication
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<ChatHub>("/chatHub");

app.MapControllers();

app.Run();