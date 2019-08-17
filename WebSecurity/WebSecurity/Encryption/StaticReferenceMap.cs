using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace WebSecurity.Encryption
{
    public static class StaticReferenceMap
    {
        public const int KeySize = 128; //bits
        public const int IvSize = 16; //bytes
        public const int OutputByteSize = KeySize / 8;
        private static readonly byte[] Key;

        static StaticReferenceMap()
        {
            
            Key = GetRandomValue() ;//pull 128 bit key in
        }

        /// <summary>
        /// Generates an encrypted value using symmetric encryption.
        /// This is utilizing speed over strength due to the limit of security through obscurity
        /// </summary>
        /// <typeparam name="T">Primitive types only</typeparam>
        /// <param name="value">direct value to be encrypted</param>
        /// <returns>Encrypted value</returns>
        public static string GetIndirectReferenceMap<T>(this T value)
        {
            //Get a converter to convert value to string
            var converter = TypeDescriptor.GetConverter(typeof(T));
            if (!converter.CanConvertTo(typeof(string)))
            {
                throw new ApplicationException("Can't convert value to string");
            }

            //Convert value direct value to string
            var directReferenceStr = converter.ConvertToString(value);

            //encode using UT8
            var directReferenceByteArray = Encoding.UTF8.GetBytes(directReferenceStr);

            //Encrypt and return URL safe Token string which is the indirect reference value
            var urlSafeToken = EncryptDirectReferenceValue<T>(directReferenceByteArray);
            return urlSafeToken;
        }

        /// <summary>
        /// Give a encrypted indirect value, will decrypt the value and
        /// return the direct reference value
        /// </summary>
        /// <param name="indirectReference">encrypted string</param>
        /// <returns>direct value</returns>
        public static string GetDirectReferenceMap(this string indirectReference)
        {
            var indirectReferenceByteArray =
                 HttpServerUtility.UrlTokenDecode(indirectReference);
            return DecryptIndirectReferenceValue(indirectReferenceByteArray);
        }

        private static string EncryptDirectReferenceValue<T>(byte[] directReferenceByteArray)
        {
            //IV needs to be a 16 byte cryptographic stength random value
            var iv = GetRandomValue();

            //We will store both the encrypted value and the IV used - IV is not a secret
            var indirectReferenceByteArray = new byte[OutputByteSize + IvSize];
            using (SymmetricAlgorithm algorithm = GetAlgorithm())
            {
                var encryptedByteArray =
                    GetEncrptedByteArray(algorithm, iv, directReferenceByteArray);

                Buffer.BlockCopy(
                    encryptedByteArray, 0, indirectReferenceByteArray, 0, OutputByteSize);
                Buffer.BlockCopy(iv, 0, indirectReferenceByteArray, OutputByteSize, IvSize);
            }
            return HttpServerUtility.UrlTokenEncode(indirectReferenceByteArray);
        }

        private static string DecryptIndirectReferenceValue(
            byte[] indirectReferenceByteArray)
        {
            byte[] decryptedByteArray;
            using (SymmetricAlgorithm algorithm = GetAlgorithm())
            {
                var encryptedByteArray = new byte[OutputByteSize];
                var iv = new byte[IvSize];

                //separate off the actual encrypted value and the IV from the byte array
                Buffer.BlockCopy(
                    indirectReferenceByteArray,
                    0,
                    encryptedByteArray,
                    0,
                    OutputByteSize);

                Buffer.BlockCopy(
                    indirectReferenceByteArray,
                    encryptedByteArray.Length,
                    iv,
                    0,
                    IvSize);

                //decrypt the byte array using the IV that was stored with the value
                decryptedByteArray = GetDecryptedByteArray(algorithm, iv, encryptedByteArray);
            }
            //decode the UTF8 encoded byte array
            return Encoding.UTF8.GetString(decryptedByteArray);
        }

        private static byte[] GetDecryptedByteArray(
             SymmetricAlgorithm algorithm, byte[] iv, byte[] valueToBeDecrypted)
        {
            var decryptor = algorithm.CreateDecryptor(Key, iv);
            return decryptor.TransformFinalBlock(
                valueToBeDecrypted, 0, valueToBeDecrypted.Length);
        }

        private static byte[] GetEncrptedByteArray(
            SymmetricAlgorithm algorithm, byte[] iv, byte[] valueToBeEncrypted)
        {
            var encryptor = algorithm.CreateEncryptor(Key, iv);
            return encryptor.TransformFinalBlock(
                valueToBeEncrypted, 0, valueToBeEncrypted.Length);
        }

        private static AesManaged GetAlgorithm()
        {
            var aesManaged = new AesManaged
            {
                KeySize = KeySize,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7
            };
            return aesManaged;
        }

        private static byte[] GetRandomValue()
        {
            var csprng = new RNGCryptoServiceProvider();
            var buffer = new Byte[16];

            //generate the random indirect value
            csprng.GetBytes(buffer);
            return buffer;
        }
    }
}
