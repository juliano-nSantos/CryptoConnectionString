namespace CryptoConnectionString
{
    public interface IConnectionStringEncryptor
    {
        /// <summary>
        /// Realiza a criptografia do valor da connection string
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        string Encrypt(string plainText);
        /// <summary>
        /// Realiza a descriptografia da connection string a ser utilizada
        /// </summary>
        /// <param name="encryptedText"></param>
        /// <returns></returns>
        string Decrypt(string encryptedText);
        bool TryDecrypt(string encryptedText, out string decryptedText);
        /// <summary>
        /// Realiza a criptografia de todos nós dentro da chave ConnectionStrings e substitui pelo valor criptografado
        /// </summary>
        /// <param name="environment"></param>
        void EncryptAppSettings(string? environment = null);
    }
}
