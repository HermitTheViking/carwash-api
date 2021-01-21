using System;

namespace Domain.Security
{
    public class PasswordGenerator
    {
        private static readonly Random Random = new Random();

        public string GeneratePassword(int passwordLength, bool strongPassword)
        {
            int seed = Random.Next(1, int.MaxValue);
            const string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            const string specialChars = @"!#$%&'()*+,-./:;=?@[\]_"; // note: Do not include < and > as this can cause characters to be hidden when viewed in HTML mail client.

            var chars = new char[passwordLength];
            var rd = new Random(seed);

            for (var i = 0; i < passwordLength; i++)
            {
                // If we are to use special characters
                if (strongPassword && i % Random.Next(3, passwordLength) == 0) { chars[i] = specialChars[rd.Next(0, specialChars.Length)]; }
                else { chars[i] = allowedChars[rd.Next(0, allowedChars.Length)]; }
            }

            return new string(chars);
        }
    }
}