using Api.Mappers;
using Api.Models;
using Domain;
using Domain.Databse.Models;
using Domain.ErrorHandling;
using Domain.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Api.Controllers
{
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMessageBus _messageBus;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper<UserDbModel, UserDto> _userMapper;
        private readonly ILogger<UsersController> _logger;

        public UsersController(
            IMessageBus messageBus,
            IUnitOfWork unitOfWork,
            IMapper<UserDbModel, UserDto> userMapper,
            ILogger<UsersController> logger)
        {
            _messageBus = messageBus ?? throw new System.ArgumentNullException(nameof(messageBus));
            _unitOfWork = unitOfWork ?? throw new System.ArgumentNullException(nameof(unitOfWork));
            _userMapper = userMapper ?? throw new System.ArgumentNullException(nameof(userMapper));
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [Route("api/users")]
        [Authorize]
        public IActionResult GetUsers()
        {
            List<UserDbModel> list = _unitOfWork.Users.GetAllAsync().Result;

            if (list.Count > 0) { Ok("No user request have been made yet"); }

            var result = new List<UserDto>();

            foreach (UserDbModel item in list) { result.Add(_userMapper.Map(item)); }

            return Ok(result);
        }

        [HttpGet]
        [Route("api/users/{email}")]
        [Authorize]
        public IActionResult GetUserByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) { throw ExceptionFactory.UserWithEmailNotFoundException(email); }

            UserDbModel dbModel = GetUserByEmailOrThrowException(email);

            return Ok(_userMapper.Map(dbModel));
        }

        private UserDbModel GetUserByEmailOrThrowException(string email)
        {
            UserDbModel dbModel = _unitOfWork.Users.GetByEmailAsync(email).Result;

            if (dbModel == null) { throw ExceptionFactory.UserWithEmailNotFoundException(email); }

            return dbModel;
        }
    }
}
