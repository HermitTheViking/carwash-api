namespace Domain.Security
{
    public interface ICryptographic
    {
        string CreateHash(string stringToHash);
        bool ValidateHashString(string stringToTest, string correctHash);
        string Encrypt(string plainText, string password);
        string Decrypt(string cipherText, string password);
    }
}