using Api.Mappers;
using Api.Models;
using Domain;
using Domain.Commands;
using Domain.Databse.Models;
using Domain.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Api.Controllers
{
    [ApiController]
    public class WashController : ControllerBase
    {
        private readonly IMessageBus _messageBus;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper<WashDbModel, WashDto> _washMapper;
        private readonly ILogger<WashController> _logger;

        public WashController(
            IMessageBus messageBus,
            IUnitOfWork unitOfWork,
            IMapper<WashDbModel, WashDto> washMapper,
            ILogger<WashController> logger)
        {
            _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _washMapper = washMapper ?? throw new ArgumentNullException(nameof(washMapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [Route("api/washes")]
        [AllowAnonymous]
        public IActionResult GetAllWashes()
        {
            List<WashDbModel> list = _unitOfWork.Wash.GetAllAsync().Result;

            if (list.Count > 0) { Ok("No wash request have been made yet"); }

            var result = new List<WashDto>();

            foreach (WashDbModel item in list) { result.Add(_washMapper.Map(item)); }

            return Ok(result);
        }

        [HttpPost]
        [Route("api/washes")]
        [AllowAnonymous]
        public IActionResult CreateNewWash(CreateWashDto createDto)
        {
            string userId = _unitOfWork.Users.GetByEmailAsync(createDto.Email).Result.Id;

            if (userId == null) { return BadRequest("User was not found"); }

            _messageBus.Send(new CreateWashCommand
            {
                Type = createDto.Type,
                UserId = userId,
                StartNow = createDto.StartNow
            });

            return Ok(createDto.StartNow ? "Wash created and started" : "Wash created");
        }

        [HttpPut]
        [Route("api/washes")]
        [AllowAnonymous]
        public IActionResult UpdateWash(UpdateWashDto updateDto)
        {
            string userId = _unitOfWork.Users.GetByEmailAsync(updateDto.Email).Result.Id;

            if (userId == null) { return BadRequest("User was not found"); }

            _messageBus.Send(new UpdateWashCommand
            {
                UserId = userId,
                StartNow = updateDto.StartNow,
                Abort = updateDto.Abort
            });

            return Ok("Wash updated");
        }
    }
}
