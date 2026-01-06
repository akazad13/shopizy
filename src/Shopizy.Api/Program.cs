using System.Text;
using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;
using Shopizy.Api;
using Shopizy.Application;
using Shopizy.Infrastructure;
using Shopizy.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddPresentation()
    .AddApplication(builder.Configuration)
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseInfrastructure();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger().UseSwaggerUI();
}

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var errorFeature = context.Features.Get<IExceptionHandlerFeature>();

        if (errorFeature != null)
        {
            var exception = errorFeature.Error;

            await context.Response.WriteAsync(
                JsonConvert.SerializeObject(
                    new { message = "Error occured!", errors = new string[] { exception.Message } }
                ),
                Encoding.UTF8
            );
        }
    });
});

app.UseCors("_myAllowSpecificOrigins");
if (!app.Environment.IsEnvironment("Testing"))
{
    app.UseHttpsRedirection();
}

app.UseAuthentication()
   .UseAuthorization();

app.MapControllers();

if (!builder.Configuration.GetValue<bool>("UsePostgreSql"))
{
    using (IServiceScope scope = app.Services.CreateScope())
    {
        var initialiser = scope.ServiceProvider.GetRequiredService<DbMigrationsHelper>();
        await initialiser.MigrateAsync();
    }
}

await app.RunAsync();
