using Domain.Databse.Models;
using Api.Models;

namespace Api.Mappers
{
    public class UserMapper : IMapper<UserDbModel, UserDto>
    {
        public UserDto Map(UserDbModel source)
        {
            return new UserDto
            {
                Id = source.Id,
                Email = source.Email
            };
        }
    }
}
