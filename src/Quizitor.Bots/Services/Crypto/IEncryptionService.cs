namespace Quizitor.Bots.Services.Crypto;

internal interface IEncryptionService
{
    string? TryEncryptString(string plainText);
    string? TryDecryptString(string cipherText);
}