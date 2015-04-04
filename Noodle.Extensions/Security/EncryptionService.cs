using System;
using System.Security.Cryptography;
namespace Noodle.Security
{
    public class EncryptionService : IEncryptionService
    {
        public string CreateSaltKey(int size)
        {
            // Generate a cryptographic random number
            var rng = new RNGCryptoServiceProvider();
            var buff = new byte[size];
            rng.GetBytes(buff);

            // Return a Base64 string representation of the random number
            return Convert.ToBase64String(buff);
        }

        public string CreatePasswordHash(string password, string saltkey, EncryptionFormat passwordFormat = EncryptionFormat.SHA1)
        {
            // TODO: Removed dependency on system.web, so fix this
            //var saltAndPassword = String.Concat(password, saltkey);
            //var hashedPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(saltAndPassword, passwordFormat.ToString());
            //return hashedPassword;
            return password;
        }

        public string EncryptText(string plainText, string encryptionPrivateKey = "")
        {
            //TODO:Encrypt
            return plainText;
        }

        public string DecryptText(string cipherText, string encryptionPrivateKey = "")
        {
            //TODO:Decrypt
            return cipherText;
        }
    }
}
