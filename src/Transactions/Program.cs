using Microsoft.EntityFrameworkCore;
using Prometheus;
using Shared;
using System.Text.Json.Serialization;
using Transactions.Infrastructure.Config;
using Transactions.Infrastructure.Persistence;

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
app.UseHttpMetrics();

app.MapMetrics();
app.MapHealthChecks("/health").WithMetadata(new PublicEndpointAttribute());

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TransactionDbContext>();
    await db.Database.MigrateAsync();
}

app.Run();
