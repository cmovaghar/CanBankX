using MediatR;
using Users.Domain.Repositories;
using Users.Domain.Services;
using Users.Infrastructure.Persistence;
using Users.Infrastructure.Repositories;
using Users.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Users.Application.Commands.RegisterUser;
using Users.Infrastructure.Exceptions;
using Accounts.Domain.Services;
using Accounts.Infrastructure.Services;

namespace Users.Infrastructure.Config
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddServiceInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<UserDbContext>(options =>
                options.UseNpgsql(
                    configuration.GetConnectionString("UserDb"),
                    sql => sql.MigrationsAssembly(typeof(UserDbContext).Assembly.FullName)
                )
            );

            services.AddHealthChecks().AddNpgSql(configuration.GetConnectionString("UserDb")!);

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommand).Assembly));
            
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddTransient<RegisterUserCommandValidator>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
            services.AddScoped<ITwoFactorService, TwoFactorService>();
            services.AddScoped<ISinEncryptor, AES256Encryptor>();
            services.AddScoped<IJwtService, JwtService>();

            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();

            services.AddHttpClient<IAccountService, AccountService>(client =>
            {
                client.BaseAddress = new Uri(configuration["Services:Accounts"]!);
                client.DefaultRequestHeaders.Add(configuration["INTERNAL_HEADER"]!, configuration["INTERNAL_SECRET"]);
            });

            return services;
        }
    }
}
