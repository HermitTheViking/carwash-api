using System;
using System.Security.Cryptography;

namespace Domain.Security
{
    public class StringHash : IStringHash
    {
        public const int SALT_BYTE_SIZE = 24;
        public const int HASH_BYTE_SIZE = 24;
        public const int PBKDF2_ITERATIONS = 1000;

        public const int ITERATION_INDEX = 0;
        public const int SALT_INDEX = 1;
        public const int PBKDF2_INDEX = 2;

        public string CreateHash(string password)
        {
            RNGCryptoServiceProvider csprng = new RNGCryptoServiceProvider();
            byte[] salt = new byte[SALT_BYTE_SIZE];
            csprng.GetBytes(salt);

            byte[] hash = PBKDF2(password, salt, PBKDF2_ITERATIONS, HASH_BYTE_SIZE);
            return PBKDF2_ITERATIONS + ":" + Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
        }

        public bool ValidatePassword(string password, string correctHash)
        {
            char[] delimiter = { ':' };
            string[] split = correctHash.Split(delimiter);
            int iterations = int.Parse(split[ITERATION_INDEX]);
            byte[] salt = Convert.FromBase64String(split[SALT_INDEX]);
            byte[] hash = Convert.FromBase64String(split[PBKDF2_INDEX]);

            byte[] testHash = PBKDF2(password, salt, iterations, hash.Length);
            return SlowEquals(hash, testHash);
        }

        private static bool SlowEquals(byte[] a, byte[] b)
        {
            uint diff = (uint)a.Length ^ (uint)b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++) { diff |= (uint)(a[i] ^ b[i]); }
            return diff == 0;
        }

        private static byte[] PBKDF2(string password, byte[] salt, int iterations, int outputBytes)
        {
            return new Rfc2898DeriveBytes(password, salt)
            {
                IterationCount = iterations
            }.GetBytes(outputBytes);
        }
    }
}
