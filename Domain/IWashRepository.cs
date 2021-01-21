using Domain.Databse.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain
{
    public interface IWashRepository
    {
        Task<List<WashDbModel>> GetAllAsync();
        Task<WashDbModel> GetRecentByWashIdAsync(string washId);
        Task<List<WashDbModel>> GetAllByUserIdAsync(string userId);
        Task<WashDbModel> GetRecentByUserIdAsync(string userId);

        Task<string> Add(WashDbModel washDbModel);
        void Remove(WashDbModel washDbModel);
        void Update(string washId, Dictionary<string, object> updates);

        bool IsWashOnGoingForUser(string userId);

        void StartWashThread(int duration, string threadName);
    }
}