using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Utility.Messaging
{
    public class FakeBus : IMessageBus
    {
        private readonly Dictionary<Type, ICommandHandler> _commandHandlers;
        private readonly Dictionary<Type, ICommandHandler> _asyncCommandHandlers;

        public FakeBus(IEnumerable<ICommandHandler> commandHandlers)
        {
            _commandHandlers = new Dictionary<Type, ICommandHandler>();
            _asyncCommandHandlers = new Dictionary<Type, ICommandHandler>();

            foreach (var item in commandHandlers)
            {
                foreach (var genericCommandHandlerIntf in item.GetType().GetInterfaces().Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICommandHandler<>)))
                {
                    var commandType = genericCommandHandlerIntf.GetGenericArguments()[0].UnderlyingSystemType;
                    _commandHandlers.Add(commandType, item);
                }

                foreach (var genericAsyncCommandHandlerIntf in item.GetType().GetInterfaces().Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IAsyncCommandHandler<>)))
                {
                    var commandType = genericAsyncCommandHandlerIntf.GetGenericArguments()[0].UnderlyingSystemType;
                    _asyncCommandHandlers.Add(commandType, item);
                }
            }
        }

        public void Send<T>(T command)
        {
            if (_commandHandlers.TryGetValue(typeof(T), out ICommandHandler commandHandler))
            {
                var handler = (ICommandHandler<T>)commandHandler;
                handler.Handle(command);
            }
            else if (_asyncCommandHandlers.TryGetValue(typeof(T), out commandHandler))
            {
                var asyncHandler = (IAsyncCommandHandler<T>)commandHandler;
                asyncHandler.HandleAsync(command).Wait();
            }
            else
            {
                throw new InvalidOperationException(string.Format("No handler registered for command of type {0}", typeof(T)));
            }
        }

        public Task SendAsync<T>(T command)
        {
            if (_asyncCommandHandlers.TryGetValue(typeof(T), out ICommandHandler commandHandler))
            {
                var asyncHandler = (IAsyncCommandHandler<T>)commandHandler;
                return asyncHandler.HandleAsync(command);
            }
            else if (_commandHandlers.TryGetValue(typeof(T), out commandHandler))
            {
                var handler = (ICommandHandler<T>)commandHandler;
                return Task.Run(() => handler.Handle(command));
            }
            else
            {
                throw new InvalidOperationException(string.Format("No handler registered for command of type {0}", typeof(T)));
            }
        }
    }
}