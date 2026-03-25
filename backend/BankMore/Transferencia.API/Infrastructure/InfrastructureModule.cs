using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Transferencia.API.Domain.Interfaces;
using Transferencia.API.Infrastructure.Data;
using Transferencia.API.Infrastructure.Repositories;
using Transferencia.API.Infrastructure.Services;

namespace Transferencia.API.Infrastructure
{
    public static class InfrastructureModule
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration
            )
        {
            services
                .AddData(configuration)
                .AddMediator(configuration)
                .AddRepositories(configuration)
                .AddAuth(configuration);

            return services;
        }

        private static IServiceCollection AddData(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<DbConnectionFactory>();

            return services;
        }

        private static IServiceCollection AddMediator(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<ContaCorrenteClient>(client =>
            {
                client.BaseAddress = new Uri(configuration["Services:ContaCorrente"]);
            });
            services.AddScoped<ITransferenciaRepository, TransferenciaRepository>();
            services.AddScoped<IIdempotenciaRepository, IdempotenciaRepository>();

            return services;
        }

        private static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
                    };
                });

            return services;
        }
    }
}
