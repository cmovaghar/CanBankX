using Microsoft.EntityFrameworkCore;
using Transactions.Application.Commands;
using Transactions.Domain.Repositories;
using Transactions.Infrastructure.Persistence;
using Transactions.Infrastructure.Repositories;

namespace Transactions.Infrastructure.Config
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddServiceInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<TransactionDbContext>(options =>
                options.UseNpgsql(
                    configuration.GetConnectionString("TransactionDb"),
                    sql => sql.MigrationsAssembly(typeof(TransactionDbContext).Assembly.FullName)
                )
            );

            services.AddHealthChecks().AddNpgSql(configuration.GetConnectionString("TransactionDb")!);

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(InitiateTransactionCommand).Assembly));

            //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            //services.AddTransient<RegisterUserCommandValidator>();

            services.AddScoped<ITransactionRepository, TransactionRepository>();

            //services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();

            return services;
        }
    }
}
