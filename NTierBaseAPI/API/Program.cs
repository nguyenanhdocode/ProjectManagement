using DataAccess;
using Application;
using API.Middlewares;
using FluentValidation.AspNetCore;
using FluentValidation;
using Application.Validators;
using API.Filters;
using API;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining(typeof(IValidationsMarker));

builder.Services.AddControllers(config => config.Filters.Add(typeof(ValidateModelAttribute)));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.EnableAnnotations();
});
builder.Services.AddAPIService(builder.Configuration);
builder.Services.AddApplicationService(builder.Configuration, builder.Environment);
builder.Services.AddDataAccessServices(builder.Configuration);
builder.Services.AddCors(p => p.AddPolicy("Default", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("Default");

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.MapControllers();

app.Run();
