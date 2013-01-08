using System.Web.Configuration;

namespace Noodle.Security
{
    public interface IEncryptionService
    {
        string CreateSaltKey(int size);
        string CreatePasswordHash(string password, string saltkey, FormsAuthPasswordFormat passwordFormat = FormsAuthPasswordFormat.SHA1);
        string EncryptText(string plainText, string encryptionPrivateKey = "");
        string DecryptText(string cipherText, string encryptionPrivateKey = "");
    }
}
