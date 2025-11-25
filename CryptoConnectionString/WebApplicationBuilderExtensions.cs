using Microsoft.AspNetCore.Builder;

namespace CryptoConnectionString
{
    public static class WebApplicationBuilderExtensions
    {        
        public static WebApplicationBuilder UseCryptoConnectionString(this WebApplicationBuilder builder, string envFilePath = ".env")
        {
            EnvironmentManager.GenerateOrLoadKeyIv(envFilePath);

            var encryptor = new ConnectionStringEncryptor();

            encryptor.EncryptAppSettings(builder.Environment.EnvironmentName);

            return builder;
        }
    }
}
