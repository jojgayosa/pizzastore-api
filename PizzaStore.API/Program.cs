using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PizzaStore.API.Common;
using PizzaStore.Application.Common.Interfaces;
using PizzaStore.Application.Pizzas.Commands;
using PizzaStore.Application.Pizzas.Queries;
using PizzaStore.Application.Toppings.Commands;
using PizzaStore.Application.Toppings.Queries;
using PizzaStore.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
});

// Register FluentValidation validators
builder.Services.AddValidatorsFromAssemblyContaining<CreateToppingValidator>();

// Register global exception handler and ProblemDetails
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Configure EF Core with PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<PizzaStoreDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IApplicationDbContext>(
    provider => provider.GetRequiredService<PizzaStoreDbContext>());

// Topping handlers
builder.Services.AddScoped<CreateToppingHandler>();
builder.Services.AddScoped<UpdateToppingHandler>();
builder.Services.AddScoped<DeleteToppingHandler>();
builder.Services.AddScoped<GetToppingsHandler>();
builder.Services.AddScoped<GetToppingHandler>();


// Pizza handlers
builder.Services.AddScoped<CreatePizzaHandler>();
builder.Services.AddScoped<UpdatePizzaHandler>();
builder.Services.AddScoped<UpdatePizzaToppingsHandler>();
builder.Services.AddScoped<DeletePizzaHandler>();
builder.Services.AddScoped<GetPizzasHandler>();
builder.Services.AddScoped<GetPizzaHandler>();

//Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();