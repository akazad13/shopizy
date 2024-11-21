using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;
using Shopizy.Api;
using Shopizy.Application;
using Shopizy.Infrastructure;
using Shopizy.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPresentation().AddApplication().AddInfrastructure(builder.Configuration);

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
        try
        {
            var errorFeature = context.Features.Get<IExceptionHandlerFeature>();

            if (errorFeature != null)
            {
                var exception = errorFeature.Error;

                if (exception is not ValidationException validationException)
                {
                    throw exception;
                }

                var errors = validationException.Errors.Select(err => err.ErrorMessage);

                var ret = new { message = "Validation errors occured!", errors = errors };

                context.Response.StatusCode = 400;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(JsonConvert.SerializeObject(ret), Encoding.UTF8);
            }
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(
                JsonConvert.SerializeObject(
                    new
                    {
                        message = "Error occured!",
                        errors = new string[] { ex!.InnerException!.Message },
                    }
                ),
                Encoding.UTF8
            );
        }
    });
});

app.UseCors("_myAllowSpecificOrigins");
app.UseHttpsRedirection().UseAuthorization();

app.MapControllers();

using (IServiceScope scope = app.Services.CreateScope())
{
    var initialiser = scope.ServiceProvider.GetRequiredService<DbMigrationsHelper>();
    await initialiser.MigrateAsync();
}

await app.RunAsync();
