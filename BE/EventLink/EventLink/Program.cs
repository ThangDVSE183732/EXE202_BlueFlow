using EventLink_Repositories.DBContext;
using EventLink_Repositories.Interface;
using EventLink_Repositories.Repository;
using Eventlink_Services.Interface;
using Eventlink_Services.Service;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ISponsorPackageRepo, SponsorPackageRepo>();
builder.Services.AddScoped<ISponsorPackageService, SponsorPackageService>();
builder.Services.AddScoped<IUserProfileRepo, UserProfileRepo>();
builder.Services.AddScoped<IUserProfileService, UserProfileService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddDbContext<EventLinkDBContext>(options =>
    options.UseSqlServer(
        EventLinkDBContext.GetConnectionString("DefaultConnection"))
);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
