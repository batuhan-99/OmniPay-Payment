using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OmniPay.API.Middleware;
using OmniPay.Application.Handlers;
using OmniPay.Application.Interfaces;
using OmniPay.Infrastructure.Gateways;
using OmniPay.Infrastructure.Persistence;
using OmniPay.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    keyValue => keyValue.Key,
                    keyValue => keyValue.Value!.Errors
                        .Select(error => string.IsNullOrWhiteSpace(error.ErrorMessage)
                            ? "Invalid value."
                            : error.ErrorMessage)
                        .ToArray());

            return new BadRequestObjectResult(new
            {
                message = "Validation failed.",
                errors
            });
        };
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("OmniPayDatabase")
    ?? throw new InvalidOperationException("Connection string 'OmniPayDatabase' is not configured.");

builder.Services.AddDbContext<PaymentDbContext>(options =>
    options.UseNpgsql(connectionString));

// Register Application & Infrastructure Services (Dependency Injection)
builder.Services.AddScoped<IPaymentRepository, EfCorePaymentRepository>();
builder.Services.AddScoped<IPaymentGateway, SimulatedPaymentGateway>();
builder.Services.AddScoped<CreatePaymentCommandHandler>();
builder.Services.AddScoped<GetPaymentByIdQueryHandler>();
builder.Services.AddScoped<ProcessPaymentCommandHandler>();
builder.Services.AddScoped<MarkPaymentAsSuccessfulCommandHandler>();
builder.Services.AddScoped<MarkPaymentAsFailedCommandHandler>();
builder.Services.AddScoped<CancelPaymentCommandHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
