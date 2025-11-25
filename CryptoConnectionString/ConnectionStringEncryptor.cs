using System.Text;
using Newtonsoft.Json.Linq;

namespace CryptoConnectionString
{
    public class ConnectionStringEncryptor : IConnectionStringEncryptor
    {
        private static readonly byte[] _encryptedPrefixBytes = Encoding.UTF8.GetBytes("!ENC!");

        /// <summary>
        /// Realiza a criptografia do valor da connection string
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return plainText;
            if (IsEncrypted(plainText)) return plainText;

            var encryptedBytes = AesEncryptionHelper.Encrypt(Encoding.UTF8.GetBytes(plainText));

            var protectedValueWithPrefix = new List<byte>(_encryptedPrefixBytes);
            protectedValueWithPrefix.AddRange(encryptedBytes);

            return Convert.ToBase64String(protectedValueWithPrefix.ToArray());
        }

        /// <summary>
        /// Realiza a descriptografia da connection string a ser utilizada
        /// </summary>
        /// <param name="encryptedText"></param>
        /// <returns></returns>
        public string Decrypt(string encryptedText)
        {
            if (string.IsNullOrEmpty(encryptedText)) return encryptedText;
            if (!IsEncrypted(encryptedText)) return encryptedText;

            var decodedBytes = Convert.FromBase64String(encryptedText);
            var encyptedBytes = decodedBytes.AsSpan(_encryptedPrefixBytes.Length).ToArray();
            Buffer.BlockCopy(decodedBytes, _encryptedPrefixBytes.Length, encyptedBytes, 0, encyptedBytes.Length);

            var decryptedBytes = AesEncryptionHelper.Decrypt(encyptedBytes);

            return Encoding.UTF8.GetString(decryptedBytes);
        }

        public bool TryDecrypt(string encryptedText, out string decryptedText)
        {
            decryptedText = null;
            try
            {
                decryptedText = Decrypt(encryptedText);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool IsEncrypted(string text)
        {
            if (string.IsNullOrEmpty(text)) return false;

            try
            {
                var decodedBytes = Convert.FromBase64String(text);

                return decodedBytes.Length >= _encryptedPrefixBytes.Length &&
                    decodedBytes.AsSpan(0, _encryptedPrefixBytes.Length).SequenceEqual(_encryptedPrefixBytes);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Realiza a criptografia de todos nós dentro da chave ConnectionStrings e substitui pelo valor criptografado
        /// </summary>
        /// <param name="environment"></param>
        public void EncryptAppSettings(string? environment = null)
        {
            var configPath = string.IsNullOrEmpty(environment)
                ? "appsettings.json"
                : $"appsettings.{environment}.json";

            if (File.Exists(configPath))
            {
                EncryptConfigFile(configPath);
            }
        }

        private void EncryptConfigFile(string filePath)
        {
            var json = JObject.Parse(File.ReadAllText(filePath));
            var connectionStrings = json["ConnectionStrings"];

            if (connectionStrings != null)
            {
                foreach (var property in connectionStrings.Children<JProperty>())
                {
                    if (!IsEncrypted(property.Value.ToString()))
                    {
                        var encryptedValue = Encrypt(property.Value.ToString());
                        property.Value = encryptedValue;
                    }
                }

                File.WriteAllText(filePath, json.ToString());
            }
        }
    }
}
