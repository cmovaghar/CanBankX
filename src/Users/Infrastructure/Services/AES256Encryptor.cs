using System.Security.Cryptography;
using Users.Domain.Services;

namespace Users.Infrastructure.Services
{
    // Tiré de https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.aes?view=net-8.0
    public class AES256Encryptor(IConfiguration config) : ISinEncryptor
    {
        private readonly IConfiguration _config = config;

        public string Encrypt(string plainText)
        {
            byte[] encrypted;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Convert.FromBase64String(_config["KEY"]!);
                aesAlg.IV = Convert.FromBase64String(_config["IV"]!);

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using MemoryStream msEncrypt = new();
                using (CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using StreamWriter swEncrypt = new(csEncrypt);
                    swEncrypt.Write(plainText);
                }

                encrypted = msEncrypt.ToArray();
            }

            return Convert.ToBase64String(encrypted);
        }

        public string Decrypt(string cipherText)
        {
            string plaintext = null;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Convert.FromBase64String(_config["KEY"]!);
                aesAlg.IV = Convert.FromBase64String(_config["IV"]!);

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using MemoryStream msDecrypt = new(Convert.FromBase64String(cipherText));
                using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
                using StreamReader srDecrypt = new(csDecrypt);
                plaintext = srDecrypt.ReadToEnd();
            }

            return plaintext;
        }
    }
}
