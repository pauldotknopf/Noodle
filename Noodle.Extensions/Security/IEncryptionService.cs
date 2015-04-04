
namespace Noodle.Security
{
    public interface IEncryptionService
    {
        string CreateSaltKey(int size);
        string CreatePasswordHash(string password, string saltkey, EncryptionFormat passwordFormat = EncryptionFormat.SHA1);
        string EncryptText(string plainText, string encryptionPrivateKey = "");
        string DecryptText(string cipherText, string encryptionPrivateKey = "");
    }
}
