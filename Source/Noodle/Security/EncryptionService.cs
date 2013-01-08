using System;
using System.Security.Cryptography;
using System.Web.Configuration;
using System.Web.Security;

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

        public string CreatePasswordHash(string password, string saltkey, FormsAuthPasswordFormat passwordFormat = FormsAuthPasswordFormat.SHA1)
        {
            string saltAndPassword = String.Concat(password, saltkey);
            string hashedPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(saltAndPassword, passwordFormat.ToString());
            return hashedPassword;
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
