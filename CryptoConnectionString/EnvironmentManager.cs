using System.Security.Cryptography;
using DotNetEnv;

namespace CryptoConnectionString
{
    public static class EnvironmentManager
    {
        public static (string key, string iv) GenerateOrLoadKeyIv(string envFilePath = ".env")
        {
            //Carrega o arquivo .env caso já tenha para carregar as variaveis de ambiente
            Env.Load();

            var key = Environment.GetEnvironmentVariable("CLIENTE_KEY");
            var IV = Environment.GetEnvironmentVariable("CLIENTE_IV");

            if (key == null || IV == null)
            {
                using var aes = Aes.Create();
                aes.GenerateKey();
                aes.GenerateIV();

                key = Convert.ToBase64String(aes.Key);
                IV = Convert.ToBase64String(aes.IV);

                UpdateEnvFile(envFilePath, key, IV);

                Environment.SetEnvironmentVariable("CLIENTE_KEY", key);
                Environment.SetEnvironmentVariable("CLIENTE_IV", IV);
            }

            return (key, IV);
        }

        private static void UpdateEnvFile(string envFilePath, string key, string IV)
        {
            if (!File.Exists(envFilePath))
            {
                File.Create(envFilePath).Dispose();
            }

            var envLines = File.ReadAllLines(envFilePath).ToList();

            var updatedKey = false;
            var updatedIV = false;

            for (int i = 0; i < envLines.Count; i++)
            {
                if (envLines[i].StartsWith("CLIENTE_KEY="))
                {
                    envLines[i] = $"CLIENTE_KEY={key}";
                    updatedKey = true;
                }
                else if (envLines[i].StartsWith("CLIENTE_IV="))
                {
                    envLines[i] = $"CLIENTE_IV={IV}";
                    updatedIV = true;
                }
            }

            if (!updatedKey)
                envLines.Add($"CLIENTE_KEY={key}");

            if (!updatedIV)
                envLines.Add($"CLIENTE_IV={IV}");

            File.WriteAllLines(envFilePath, envLines);
        }
    }
}
