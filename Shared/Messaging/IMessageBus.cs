using System.Threading.Tasks;

namespace Utility.Messaging
{
    public interface IMessageBus
    {
        void Send<T>(T command);
        Task SendAsync<T>(T command);
    }
}