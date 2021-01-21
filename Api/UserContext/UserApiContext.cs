using Domain.Events;

namespace Api.UserContext
{
    public class UserApiContext : ICurrentContext, IUserApiContextInitializer
    {
        public string UserId { get; private set; }

        public void SetCurrentUserId(string userId)
        {
            UserId = userId;
        }
    }
}