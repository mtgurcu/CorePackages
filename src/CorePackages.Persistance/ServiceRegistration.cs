using CorePackages.Persistance.Interfaces;
using CorePackages.Persistance.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace CorePackages.Persistance
{
    public static class ServiceRegistration
    {
        public static void AddPersistanceServiceRegistration(this IServiceCollection services)
        {
            services.AddScoped<IOutboxRepository, OutboxRepository>();
        }
    }
}
