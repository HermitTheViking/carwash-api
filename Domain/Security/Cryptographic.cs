using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Domain.Security
{
    public class Cryptographic : ICryptographic
    {
        private readonly IConfiguration _config;

        private int SALT_BYTE_SIZE = 0;
        private int HASH_BYTE_SIZE = 0;
        private int PBKDF2_ITERATIONS = 0;
                
        private int ITERATION_INDEX = 0;
        private int SALT_INDEX = 0;
        private int PBKDF2_INDEX = 0;

        private static byte[] _saltBytes;
        private static byte[] _initVectorBytes;

        public Cryptographic(IConfiguration config)
        {
            _config = config;
            SALT_BYTE_SIZE = int.Parse(_config.GetSection("crypto").GetSection("SALT_BYTE_SIZE").Value);
            HASH_BYTE_SIZE = int.Parse(_config.GetSection("crypto").GetSection("HASH_BYTE_SIZE").Value);
            PBKDF2_ITERATIONS = int.Parse(_config.GetSection("crypto").GetSection("PBKDF2_ITERATIONS").Value);

            ITERATION_INDEX = int.Parse(_config.GetSection("crypto").GetSection("ITERATION_INDEX").Value);
            SALT_INDEX = int.Parse(_config.GetSection("crypto").GetSection("SALT_INDEX").Value);
            PBKDF2_INDEX = int.Parse(_config.GetSection("crypto").GetSection("PBKDF2_INDEX").Value);

            _saltBytes = Encoding.UTF8.GetBytes(_config.GetSection("crypto").GetSection("SALT").Value);
            _initVectorBytes = Encoding.UTF8.GetBytes(_config.GetSection("crypto").GetSection("INITVECTOR").Value);
        }

        public string CreateHash(string stringToHash)
        {
            RNGCryptoServiceProvider csprng = new RNGCryptoServiceProvider();
            byte[] salt = new byte[SALT_BYTE_SIZE];
            csprng.GetBytes(salt);

            byte[] hash = PBKDF2(stringToHash, salt, PBKDF2_ITERATIONS, HASH_BYTE_SIZE);
            return PBKDF2_ITERATIONS + ":" + Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
        }

        public bool ValidateHashString(string stringToTest, string correctHash)
        {
            char[] delimiter = { ':' };
            string[] split = correctHash.Split(delimiter);
            int iterations = int.Parse(split[ITERATION_INDEX]);
            byte[] salt = Convert.FromBase64String(split[SALT_INDEX]);
            byte[] hash = Convert.FromBase64String(split[PBKDF2_INDEX]);

            byte[] testHash = PBKDF2(stringToTest, salt, iterations, hash.Length);
            return SlowEquals(hash, testHash);
        }
     
        public string Encrypt(string plainText, string password)
        {
            return Convert.ToBase64String(EncryptToBytes(Encoding.UTF8.GetBytes(plainText), password));
        }

        public string Decrypt(string cipherText, string password)
        {
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText.Replace(' ', '+'));
            return Decrypt(cipherTextBytes, password).TrimEnd('\0');
        }
 
        private static byte[] EncryptToBytes(byte[] plainTextBytes, string password)
        {
            int keySize = 256;

            byte[] initialVectorBytes = _initVectorBytes;
            byte[] saltValueBytes = _saltBytes;
            byte[] keyBytes = new Rfc2898DeriveBytes(password, saltValueBytes).GetBytes(keySize / 8);

            using RijndaelManaged symmetricKey = new RijndaelManaged
            {
                Mode = CipherMode.CBC
            };

            using ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initialVectorBytes);
            using MemoryStream memStream = new MemoryStream();
            using CryptoStream cryptoStream = new CryptoStream(memStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();

            return memStream.ToArray();
        }

        private static string Decrypt(byte[] cipherTextBytes, string password)
        {
            int keySize = 256;

            byte[] initialVectorBytes = _initVectorBytes;
            byte[] saltValueBytes = _saltBytes;
            byte[] keyBytes = new Rfc2898DeriveBytes(password, saltValueBytes).GetBytes(keySize / 8);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            using RijndaelManaged symmetricKey = new RijndaelManaged
            {
                Mode = CipherMode.CBC
            };

            using ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initialVectorBytes);
            using MemoryStream memStream = new MemoryStream(cipherTextBytes);
            using CryptoStream cryptoStream = new CryptoStream(memStream, decryptor, CryptoStreamMode.Read);
            int byteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);

            return Encoding.UTF8.GetString(plainTextBytes, 0, byteCount);
        }

        private static bool SlowEquals(byte[] a, byte[] b)
        {
            uint diff = (uint)a.Length ^ (uint)b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++) { diff |= (uint)(a[i] ^ b[i]); }
            return diff == 0;
        }

        private static byte[] PBKDF2(string password, byte[] salt, int iterations, int outputBytes) => new Rfc2898DeriveBytes(password, salt)
        {
            IterationCount = iterations
        }.GetBytes(outputBytes);
    }
}
