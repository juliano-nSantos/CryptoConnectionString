using System.Security.Cryptography;
using System.Text;

namespace CryptoConnectionString
{
    public static class AesEncryptionHelper
    {

        private static readonly Lazy<(byte[] key, byte[] iv)> _cryptoParams = new Lazy<(byte[] key, byte[] iv)>(() =>
        {
            var key = Environment.GetEnvironmentVariable("CLIENTE_KEY");
            var iv = Environment.GetEnvironmentVariable("CLIENTE_IV");

            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(iv))
            {
                throw new InvalidOperationException(
                    "CLIENTE_KEY e CLIENTE_IV environment variables are required. " +
                    "Call EnvironmentManager.GenerateOrLoadKeyIv() first.");
            }

            return (Convert.FromBase64String(key), Convert.FromBase64String(iv));
        });

        private static byte[] key = _cryptoParams.Value.key;
        private static byte[] iv = _cryptoParams.Value.iv;

        public static string Encrypt(string plainText)
        {
            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;

            var encryptor = aes.CreateEncryptor();

            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] cypherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            return Convert.ToBase64String(cypherBytes);
        }

        public static byte[] Encrypt(byte[] value)
        {
            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;

            var encryptor = aes.CreateEncryptor();           
            
            byte[] plainBytes = value.Concat(key.Concat(iv)).ToArray();

            byte[] cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            return cipherBytes;
        }

        public static string Decrypt(string cypherText)
        {
            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            var decryptor = aes.CreateDecryptor();

            byte[] cypherBytes = Convert.FromBase64String(cypherText);
            byte[] plainText = decryptor.TransformFinalBlock(cypherBytes, 0, cypherBytes.Length);

            return Encoding.UTF8.GetString(plainText);
        }

        public static byte[] Decrypt(byte[] value)
        {
            try
            {
                using var aes = Aes.Create();
                aes.Key = key;
                aes.IV = iv;

                var decryptor = aes.CreateDecryptor();

                var scope = key.Concat(iv).ToArray();
                //var encryptedValues = value.Concat(scope).ToArray();

                byte[] decriptedBytes = decryptor.TransformFinalBlock(value, 0, value.Length);

                byte[] cypherBytes = new byte[decriptedBytes.Length - scope.Length];

                Buffer.BlockCopy(decriptedBytes, 0, cypherBytes, 0, cypherBytes.Length);

                return cypherBytes;
            }
            catch (Exception)
            {
                throw;
            }            
        }
    }
}
