using Shopizy.Api;
using Shopizy.Application;
using Shopizy.Infrastructure;
using Shopizy.Infrastructure.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddPresentation().AddApplication().AddInfrastructure(builder.Configuration);

WebApplication app = builder.Build();

app.UseInfrastructure();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    _ = app.UseSwagger().UseSwaggerUI();
}

app.UseCors("_myAllowSpecificOrigins");
app.UseHttpsRedirection().UseAuthorization();

app.MapControllers();

using (IServiceScope scope = app.Services.CreateScope())
{
    DbMigrationsHelper initialiser = scope.ServiceProvider.GetRequiredService<DbMigrationsHelper>();
    await initialiser.MigrateAsync();
}

await app.RunAsync();
