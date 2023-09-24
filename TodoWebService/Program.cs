using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using TodoWebService;
using TodoWebService.BackgroundServices;
using TodoWebService.Data;
using TodoWebService.Models.DTOs.Validations;
using TodoWebService.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

var configuration = new ConfigurationBuilder()
			.AddJsonFile("appsettings.json")
			.Build();

Log.Logger = new LoggerConfiguration()
	.ReadFrom.Configuration(configuration)
	.CreateLogger();

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IEmailService, SmtpEmailService>();
builder.Services.AddHostedService<TodoNotificationService>();

builder.Services.AddDbContext<TodoDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("TodoDbConnectionString"));
});

builder.Services.AddAuthenticationAndAuthorization(builder.Configuration);

builder.Services.AddSwagger();

builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
