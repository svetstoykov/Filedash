using Filedash.Domain.IoC;
using Filedash.Infrastructure.IoC;
using Filedash.Web.IoC;
using Filedash.Web.Middleware;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services
    .AddDomainServices()
    .AddInfrastructureServices(builder.Configuration)
    .AddWebServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");
app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseRouting();
app.UseEndpoints(e =>
{
    e.MapControllers();
});

app.Run();

public partial class Program { }
