using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [ServiceFilter(typeof(AutherizeCurrentLoggedInUser))]
    [Route("api/user/{userId}/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IDatingRepository _datingRepository;
        private readonly IMapper _mapper;

        public MessagesController(IDatingRepository datingRepository, IMapper mapper)
        {
            _datingRepository = datingRepository;
            _mapper = mapper;
        }

        [HttpGet("{id}",Name ="GetMessage")]
        public async Task<IActionResult> GetMessage(int userId,int id)
        {
            var messageFromRepo = await _datingRepository.GetMessage(id);
            if (messageFromRepo == null)
            {
                return NotFound();
            }
            return Ok(messageFromRepo);
        }
        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId,MessageForCreationDto messageForCreationDto)
        {
            var sender = await _datingRepository.GetUser(userId);
            messageForCreationDto.SenderId = userId;
            var reciever = await _datingRepository.GetUser(messageForCreationDto.RecieverId);
            if (reciever == null)
            {
                return BadRequest("Could not found user");
            }
            var message = _mapper.Map<Message>(messageForCreationDto);
            _datingRepository.Add(message);
            if (await _datingRepository.SaveAll())
            {
                //var messagesFromRepo = await _datingRepository.GetMessageThread(userId, messageForCreationDto.RecieverId);
                //message = messagesFromRepo.FirstOrDefault(m => m.SenderId == userId && m.RecieverId == messageForCreationDto.RecieverId);
                var messageToReturn = _mapper.Map<MessageToReturnDto>(message);
                return CreatedAtRoute("GetMessage", new { userId = userId, id = message.Id }, messageToReturn);
            }
            throw new Exception("Creating message failed on save");
        }

        [HttpGet]
        public async Task<IActionResult> GetMessageForUser(int userId,[FromQuery]MessageParams messageParams)
        {
            messageParams.UserId = userId;
            var messagesFromRepo = await _datingRepository.GetMessagesForUser(messageParams);
            
            var messages = _mapper.Map<IEnumerable<MessageToReturnDto>>(messagesFromRepo);
            Response.AddPaginationHeader(messagesFromRepo.CurrentPage, messagesFromRepo.PageSize, messagesFromRepo.TotalCount, messagesFromRepo.TotalPages);
            return Ok(messages);
        }

        [HttpGet("thread/{recieverId}")]
        public async Task<IActionResult> GetMessageForUser(int userId,int recieverId)
        {
            var messagesFromRepo = await _datingRepository.GetMessageThread(userId,recieverId);
            var messagesThread = _mapper.Map<IEnumerable<MessageToReturnDto>>(messagesFromRepo);
            return Ok(messagesThread);
        }
        [HttpPost("{messageId}")]
        public async Task<IActionResult> DeleteMessage(int messageId,int userId)
        {
            var messageFromRepo = await _datingRepository.GetMessage(messageId);
            if(messageFromRepo.SenderId == userId)
            {
                messageFromRepo.Senderdeleted = true;
            }
            if (messageFromRepo.RecieverId == userId)
            {
                messageFromRepo.Recieverdeleted = true;
            }

            if(messageFromRepo.Senderdeleted && messageFromRepo.Recieverdeleted)
            {
                _datingRepository.Delete<Message>(messageFromRepo);
            }
            if(await _datingRepository.SaveAll())
            {
                return NoContent();
            }
            throw new Exception("Error in deleting the message");
        }

        [HttpPost("{messageId}/read")]
        public async Task<IActionResult> MarkMessageAsRead(int messageId,int userId)
        {
            var messageFromRepo = await _datingRepository.GetMessage(messageId);
            if (messageFromRepo.RecieverId != userId)
            {
                return Unauthorized();
            }
            messageFromRepo.IsRead = true;
            messageFromRepo.DateRead = DateTime.Now;
            await _datingRepository.SaveAll();

                return NoContent();
            
        }
    }
} 