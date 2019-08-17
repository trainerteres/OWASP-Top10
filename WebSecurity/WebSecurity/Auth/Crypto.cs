using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace WebSecurity.Auth
{
    public static class Crypto
    {
        private const int PBKDF2IterCount = 1000; // default for Rfc2898DeriveBytes
        private const int PBKDF2SubkeyLength = 256 / 8; // 256 bits
        private const int SaltSize = 128 / 8; // 128 bits

        /* =======================
        * HASHED PASSWORD FORMATS
        * =======================
        *
        * Version 0:
        * PBKDF2 with HMAC-SHA1, 128-bit salt, 256-bit subkey, 1000 iterations.
        * (See also: SDL crypto guidelines v5.1, Part III)
        * Format: { 0x00, salt, subkey }
        */

        public static string HashPassword(string password)
        {
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }

            // Produce a version 0 (see comment above) text hash.
            byte[] salt;
            byte[] hash;
            using (var deriveBytes = new Rfc2898DeriveBytes(password, SaltSize, PBKDF2IterCount))
            {
                salt = deriveBytes.Salt;
                hash = deriveBytes.GetBytes(PBKDF2SubkeyLength);
            }

            var hashBytes = new byte[1 + SaltSize + PBKDF2SubkeyLength];
            Buffer.BlockCopy(salt, 0, hashBytes, 1, SaltSize);
            Buffer.BlockCopy(hash, 0, hashBytes, 1 + SaltSize, PBKDF2SubkeyLength);
            return Convert.ToBase64String(hashBytes);
        }

        public static bool Verify(string password, string hashedPassword)
        {
            var inputPasswordHash = HashPassword(password);

            var base64Hash = hashedPassword;

            var hashBytes = Convert.FromBase64String(base64Hash);

            //using (var deriveBytes = new Rfc2898DeriveBytes(password, SaltSize, PBKDF2IterCount))
            //{
            //    salt = deriveBytes.Salt;
            //    hash = deriveBytes.GetBytes(PBKDF2SubkeyLength);
            //}

            var salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);


            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, PBKDF2IterCount);
            byte[] hash = pbkdf2.GetBytes(PBKDF2SubkeyLength);

            for (var i = 0; i < PBKDF2SubkeyLength; i++)
            {
                if(hash[i+SaltSize] != hash[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}