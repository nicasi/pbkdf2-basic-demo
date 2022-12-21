using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace pbkdf_basic_demo
{
    /**
     * PBKDF algorithm 
     */
    public class Crypto
    {
        public byte[] GenerateKeyFromPassword(string password, byte[]? salt)
        {
            // This generates the key:
            byte[] key = Rfc2898DeriveBytes.Pbkdf2(password, salt, 1000000, HashAlgorithmName.SHA512, 32);

            return key;
        }
        
        public (byte[] key, byte[] salt) GenerateKeyFromPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(20);

            // This generates the key:
            byte[] key = Rfc2898DeriveBytes.Pbkdf2(password, salt, 1000000, HashAlgorithmName.SHA512, 32);

            return (key, salt);
        }

        public (byte[] cipher_output, byte[] k, byte[] IV) Encrypt(byte[] plain_input)
        {
            Aes aes = Aes.Create();
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(plain_input, 0, plain_input.Length);
                    cs.Close();
                    return (ms.ToArray(), aes.Key, aes.IV);
                }
            }
        }

        public (byte[] cipher_output, byte[] k, byte[] IV) Encrypt(byte[] plain_input, byte[] key)
        {
            Aes aes = Aes.Create();
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(key, aes.IV), CryptoStreamMode.Write))
                {
                    cs.Write(plain_input, 0, plain_input.Length);
                    cs.Close();
                    return (ms.ToArray(), key, aes.IV);
                }
            }
        }

        public byte[] Decrypt(byte[] input, byte[] key, byte[] IV)
        {
            Aes aes = Aes.Create();
            aes.Key = key;
            aes.IV = IV;
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(key, IV), CryptoStreamMode.Write))
                {
                    cs.Write(input, 0, input.Length);
                    cs.Close();
                    return ms.ToArray();
                }
            }
        }
    }
}
