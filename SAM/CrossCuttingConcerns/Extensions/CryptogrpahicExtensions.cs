using System;
using System.Security.Cryptography;
using System.Text;

namespace SAM1.CrossCuttingConcerns.Extensions
{
    public static class CryptographicExtensions
    {
        /// <summary>
        /// This will generate a SHA1 512 hash for the plaintext password
        /// </summary>
        /// <param name="plainText">The password in plaintext</param>
        /// <returns></returns>
        public static string GenerateHash(this string plainText)
        {
            var cryptographicProvider = SHA512.Create();
            var plainTextBytes = Encoding.ASCII.GetBytes(plainText);
            var cipherBytes = cryptographicProvider.ComputeHash(plainTextBytes);

            return Convert.ToBase64String(cipherBytes);
        }
    }
}