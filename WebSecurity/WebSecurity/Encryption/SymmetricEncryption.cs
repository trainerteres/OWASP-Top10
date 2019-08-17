using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace WebSecurity.Encryption
{
    public class SymmetricEncryption
    {
        // Method to encrypte a string data and save it in a specific file using an AES algorithm  
        public string EncryptText(SymmetricAlgorithm aesAlgorithm, string text)
        {
            // Create an encryptor from the AES algorithm instance and pass the aes algorithm key and inialiaztion vector to generate a new random sequence each time for the same text  
            ICryptoTransform encryptor = aesAlgorithm.CreateEncryptor(aesAlgorithm.Key, aesAlgorithm.IV);

            // Create a memory stream to save the encrypted data in it  
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter writer = new StreamWriter(cs))
                    {
                        // Write the text in the stream writer   
                        writer.Write(text);
                    }
                }

                // Get the result as a byte array from the memory stream   
                byte[] encryptedDataBuffer = ms.ToArray();

                // Write the data to a file   

                UnicodeEncoding byteConverter = new UnicodeEncoding();

                return Convert.ToBase64String(encryptedDataBuffer);
            }
        }

        // Method to decrypt a data from a specific file and return the result as a string   
        public string DecryptData(SymmetricAlgorithm aesAlgorithm, string text)
        {
            // Create a decryptor from the aes algorithm   
            ICryptoTransform decryptor = aesAlgorithm.CreateDecryptor(aesAlgorithm.Key, aesAlgorithm.IV);

            // Read the encrypted bytes from the file   
            UnicodeEncoding byteConverter = new UnicodeEncoding();
            byte[] encryptedDataBuffer = Convert.FromBase64String(text);

            // Create a memorystream to write the decrypted data in it   
            using (MemoryStream ms = new MemoryStream(encryptedDataBuffer))
            {
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader reader = new StreamReader(cs))
                    {
                        // Reutrn all the data from the streamreader   
                        return reader.ReadToEnd();
                    }
                }
            }
        }
    }
}