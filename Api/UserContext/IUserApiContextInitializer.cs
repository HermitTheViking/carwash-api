namespace Api.UserContext
{
    public interface IUserApiContextInitializer
    {
        void SetCurrentUserId(string userId);
    }
}