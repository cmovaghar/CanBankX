using MediatR;
using Users.Application.Commands;
using Users.Application.Validators;
using Users.Domain.Repositories;
using Users.Domain.Services;
using Users.Infrastructure.Persistence;
using Users.Infrastructure.Repositories;
using Users.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Users.Infrastructure.Config
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddUserServiceInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<UserDbContext>(options =>
                options.UseNpgsql(
                    configuration.GetConnectionString("UserDb"),
                    sql => sql.MigrationsAssembly(typeof(UserDbContext).Assembly.FullName)));
            
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommand).Assembly));

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            //services.AddValidatorsFromAssemblyContaining<RegisterUserCommandValidator>();
            services.AddTransient<RegisterUserCommandValidator>();

            services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
            services.AddScoped<ITwoFactorService, TwoFactorService>();

            return services;
        }
    }
}
