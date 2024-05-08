using Filedash.Domain.IoC;
using Filedash.Infrastructure.IoC;
using Filedash.StartUp.IoC;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services
    .AddWebServices()
    .AddDomainServices()
    .AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();