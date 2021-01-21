using Domain.Commands;
using Domain.Databse.Models;
using Domain.ErrorHandling;
using Domain.Events;
using Domain.Events.Wash;
using Domain.Messaging;
using Domain.Time;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Domain.CommandHandlers
{
    public class WashCommandHandler :
        ICommandHandler,
        ICommandHandler<CreateWashCommand>,
        ICommandHandler<UpdateWashCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEventStore _eventStore;
        private readonly WashEventFactory _washEventFactory;
        private readonly ITimeService _timeService;
        private readonly IConfiguration _config;
        private readonly ILogger<WashCommandHandler> _logger;

        public WashCommandHandler(IUnitOfWork unitOfWork,
                                  IEventStore eventStore,
                                  WashEventFactory washEventFactory,
                                  ITimeService timeService,
                                  IConfiguration config,
                                  ILogger<WashCommandHandler> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
            _washEventFactory = washEventFactory ?? throw new ArgumentNullException(nameof(washEventFactory));
            _timeService = timeService ?? throw new ArgumentNullException(nameof(timeService));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Handle(CreateWashCommand command)
        {
            if (command == null) { return; }
            if (!_unitOfWork.Wash.IsWashOnGoingForUser(command.UserId)) { throw ExceptionFactory.WashIsOnGoingException(); }

            var newDbModel = new WashDbModel()
            {
                Done = false,
                Duration = int.Parse(_config.GetSection("WashTypes").GetSection(command.Type.ToString()).Value),
                StartTime = _timeService.UtcNow,
                Type = command.Type,
                UserId = command.UserId
            };

            newDbModel.Id = _unitOfWork.Wash.Add(newDbModel).Result;

            _eventStore.AddEvents(_washEventFactory.GetWashCreatedEvent(newDbModel));
        }

        public void Handle(UpdateWashCommand command)
        {
            WashDbModel washDbModel = _unitOfWork.Wash.GetRecentByUserIdAsync(command.UserId).Result;

            if (washDbModel == null) { return; }

            if (washDbModel.Done) { throw ExceptionFactory.NoWashIsOnGoingException(); }

            washDbModel.Done = command.Abort;

            if (command.Abort)
            {
                _unitOfWork.Wash.Update(washDbModel.Id, new Dictionary<string, object>() { { "Done", command.Abort } });
            }
            else
            {
                DateTime startTime = _timeService.UtcNow;
                _unitOfWork.Wash.StartWashThread(washDbModel.Duration, $"{washDbModel.Id}-{washDbModel.Type}");
                _unitOfWork.Wash.Update(washDbModel.Id, new Dictionary<string, object>() { { "StartTime", startTime } });
            }

            _eventStore.AddEvents(_washEventFactory.GetWashUpdatedEvent(washDbModel));
        }
    }
}