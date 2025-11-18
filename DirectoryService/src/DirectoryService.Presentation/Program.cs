using DirectoryService.Application.DI;
using DirectoryService.Infrastructure;
using DirectoryService.Presentation.Middlewares;
using Microsoft.AspNetCore.HttpLogging;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

builder.Services.AddControllers();
builder.Services.AddHttpLogging(opts =>
{
    opts.LoggingFields = HttpLoggingFields.RequestProperties |
                         HttpLoggingFields.RequestQuery |
                         HttpLoggingFields.RequestBody |
                         HttpLoggingFields.ResponseBody |
                         HttpLoggingFields.ResponseStatusCode;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services
    .AddInfrastructure()
    .AddApplication();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpLogging();
app.UseExceptionMiddleware();
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

namespace DirectoryService.Presentation
{
    public class Program;
}