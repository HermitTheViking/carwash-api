using Domain.Databse.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain
{
    public interface IUserRepository
    {
        Task<List<UserDbModel>> GetAllAsync();
        Task<UserDbModel> GetByIdAsync(string userId);
        Task<UserDbModel> GetByEmailAsync(string userEmail);

        Task<string> Add(UserDbModel userBbModel);
        void Remove(UserDbModel userBbModel);

        bool IsEmailAlreadyInUse(string userEmail);
    }
}