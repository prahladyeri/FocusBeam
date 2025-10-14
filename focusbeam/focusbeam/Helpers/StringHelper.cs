/**
 * StringHelper.cs
 * 
 * @author Prahlad Yeri <prahladyeri@yahoo.com>
 * @license MIT
 */
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace focusbeam.Helpers
{
    internal static class StringHelper
    {
        internal static string Encrypt(string plainText, string password, string salt)
        {
            using (Aes aes = Aes.Create())
            {
                byte[] saltBytes = Encoding.UTF8.GetBytes(salt);
                var key = new Rfc2898DeriveBytes(password, saltBytes, 10000);
                aes.Key = key.GetBytes(32);
                aes.IV = key.GetBytes(16);

                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (var sw = new StreamWriter(cs))
                        sw.Write(plainText);
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        internal static string Decrypt(string cipherText, string password, string salt)
        {
            using (Aes aes = Aes.Create())
            {
                byte[] saltBytes = Encoding.UTF8.GetBytes(salt);
                var key = new Rfc2898DeriveBytes(password, saltBytes, 10000);
                aes.Key = key.GetBytes(32);
                aes.IV = key.GetBytes(16);

                byte[] buffer = Convert.FromBase64String(cipherText);

                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (var ms = new MemoryStream(buffer))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }
    }
}
