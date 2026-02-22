using System.Text;
using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;
using Shopizy.Api;
using Shopizy.Application;
using Shopizy.Infrastructure;
using Shopizy.Infrastructure.Services;
using Shopizy.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddPresentation()
    .AddApplication(builder.Configuration)
    .AddInfrastructure(builder.Configuration)
    .AddEndpoints(typeof(Program).Assembly);

var app = builder.Build();

app.UseInfrastructure();
app.MapEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger().UseSwaggerUI();
}

app.UseExceptionHandler();

app.UseCors("_myAllowSpecificOrigins");
if (!app.Environment.IsEnvironment("Testing"))
{
    app.UseHttpsRedirection();
}

app.UseAuthentication()
   .UseAuthorization();

if (!builder.Configuration.GetValue<bool>("UsePostgreSql"))
{
    using (IServiceScope scope = app.Services.CreateScope())
    {
        var initialiser = scope.ServiceProvider.GetRequiredService<DbMigrationsHelper>();
        await initialiser.MigrateAsync();
    }
}

await app.RunAsync();
