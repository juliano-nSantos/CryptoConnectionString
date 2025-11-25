using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoConnectionString
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Gera a cliente_key, cliente_iv, caso não tenha o arquivo .env gera um arquivo e faz a DI
        /// </summary>
        /// <param name="services"></param>
        /// <param name="envFilePath"></param>
        /// <returns></returns>
        public static IServiceCollection AddCryptoConnectionString(this IServiceCollection services, string envFilePath = ".env")
        {
            EnvironmentManager.GenerateOrLoadKeyIv(envFilePath);

            services.AddSingleton<IConnectionStringEncryptor, ConnectionStringEncryptor>();

            return services;
        }

        /// <summary>
        /// Gera a cliente_key, cliente_iv, caso não tenha o arquivo .env gera um arquivo, faz a DI e criptografa as connections strings e atualiza o appsettings.json
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="envFilePath"></param>
        /// <returns></returns>
        public static IServiceCollection AddCryptoConnectionString(this IServiceCollection services, IConfiguration configuration, string envFilePath = ".env")
        {
            services.AddCryptoConnectionString(envFilePath);

            var encryptor = new ConnectionStringEncryptor();

            var environment = configuration.GetValue<string>("Environment") ??
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ??
                "Production";

            encryptor.EncryptAppSettings(environment);

            return services;
        }
    }
}
