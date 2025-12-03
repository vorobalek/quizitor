using System.Security.Cryptography;
using System.Text;
using Quizitor.Bots.Configuration;

namespace Quizitor.Bots.Services.Crypto;

internal sealed class EncryptionService : IEncryptionService
{
    private const int SaltSize = 16;
    private const int Iterations = 100_000;

    public string? TryEncryptString(string plainText)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(plainText)) return null;

            var password = AppConfiguration.CryptoPassword;
            var salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            GenerateKeyAndIv(password, salt, out var key, out var iv);

            byte[] encryptedBytes;
            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using var ms = new MemoryStream();
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    var plainBytes = Encoding.UTF8.GetBytes(plainText);
                    cs.Write(plainBytes, 0, plainBytes.Length);
                }

                encryptedBytes = ms.ToArray();
            }

            var resultBytes = new byte[salt.Length + encryptedBytes.Length];
            Buffer.BlockCopy(salt, 0, resultBytes, 0, salt.Length);
            Buffer.BlockCopy(encryptedBytes, 0, resultBytes, salt.Length, encryptedBytes.Length);
            return Convert.ToBase64String(resultBytes);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public string? TryDecryptString(string cipherText)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(cipherText))
                throw new ArgumentNullException(nameof(cipherText));

            var password = AppConfiguration.CryptoPassword;
            var fullCipherBytes = Convert.FromBase64String(cipherText);
            var salt = new byte[SaltSize];
            Buffer.BlockCopy(fullCipherBytes, 0, salt, 0, salt.Length);

            var encryptedBytes = new byte[fullCipherBytes.Length - salt.Length];
            Buffer.BlockCopy(fullCipherBytes, salt.Length, encryptedBytes, 0, encryptedBytes.Length);

            GenerateKeyAndIv(password, salt, out var key, out var iv);

            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
            {
                cs.Write(encryptedBytes, 0, encryptedBytes.Length);
            }

            var decryptedBytes = ms.ToArray();
            return Encoding.UTF8.GetString(decryptedBytes);
        }
        catch (Exception)
        {
            return null;
        }
    }

    private static void GenerateKeyAndIv(string password, byte[] salt, out byte[] key, out byte[] iv)
    {
        key = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, 32);
        iv = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, 16);
    }
}