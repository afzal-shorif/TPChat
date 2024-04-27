
using CimpleChat.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CimpleChat.Test
{
    public static class Helper
    {
        private static IServiceProvider Provider()
        {
            var services = new ServiceCollection();
            services.AddScoped<IGetNextId,  GetNextId>();

            return services.BuildServiceProvider();
        }

        public static T GetRequiredService<T>()
        {
            var provider = Provider();

            return provider.GetRequiredService<T>();
        }
    }
}
