using Microsoft.EntityFrameworkCore;
using Shared;
using System.Text.Json.Serialization;
using Users.Domain.Entities;
using Users.Domain.Services;
using Users.Infrastructure.Config;
using Users.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServiceInfrastructure(builder.Configuration);
builder.Services.AddSharedAuth(builder.Configuration);

builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSharedAuth();
app.MapControllers();
app.UseExceptionHandler();

app.MapHealthChecks("/health").WithMetadata(new PublicEndpointAttribute());

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
    await db.Database.MigrateAsync();
    await UserDbContextSeed.SeedAsync(db, hasher);
}

app.Run();