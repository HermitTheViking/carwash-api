namespace Utility.Security
{
    public interface IStringHash
    {
        string CreateHash(string password);
        bool ValidatePassword(string password, string correctHash);
    }
}