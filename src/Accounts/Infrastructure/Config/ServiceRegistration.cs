using Accounts.Application.Commands.OpenAccount;
using Accounts.Domain.Repositories;
using Accounts.Infrastructure.Exceptions;
using Accounts.Infrastructure.Persistence;
using Accounts.Infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Accounts.Infrastructure.Config
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddServiceInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AccountDbContext>(options =>
                options.UseNpgsql(
                    configuration.GetConnectionString("AccountDb"),
                    sql => sql.MigrationsAssembly(typeof(AccountDbContext).Assembly.FullName)
                )
            );

            services.AddHealthChecks().AddNpgSql(configuration.GetConnectionString("AccountDb")!);

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(OpenAccountCommand).Assembly));

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddTransient<OpenAccountCommandValidator>();

            services.AddScoped<IAccountRepository, AccountRepository>();

            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();

            return services;
        }
    }
}
