using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using API.Extensions;
using API.Entities;
using AutoMapper;
using API.Helpers;


namespace API.Controllers
{
    public class MessagesController:BaseApicontroller
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;

        public MessagesController(IUserRepository userRepository,IMessageRepository messageRepository,IMapper mapper)
        {
            _userRepository = userRepository;
            _messageRepository = messageRepository;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult> CreateMessage(CreateMessageDto messageParams)
        {
            if(messageParams.RecipientUserName.ToLower()==User.GetUserName()) return BadRequest("You cannot send message to yourself");

            var recipientUser= await _userRepository.GetUserByUserNameAsync(messageParams.RecipientUserName);
            if(recipientUser==null) return NotFound();

            var senderUser= await _userRepository.GetUserByUserNameAsync(User.GetUserName());
            
            var message= new Message{
                Content=messageParams.Content,
                Recipient=recipientUser,
                RecipientUserName=recipientUser.UserName,
                Sender=senderUser,
                SenderUserName=senderUser.UserName,
            };

            _messageRepository.AddMessage(message);
           
            if(await _messageRepository.SaveAllAsync()) return Ok(_mapper.Map<MessageDto>(message));

            return BadRequest("Failed to send message");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessages([FromQuery]MessageParams messageParams)
        {
            messageParams.UserName=User.GetUserName();
            var pagedListMessageDtoResult= await _messageRepository.GetMessages(messageParams);
            Response.AddPaginationHeader(pagedListMessageDtoResult.PageNumber,pagedListMessageDtoResult.ItemsPerPage,pagedListMessageDtoResult.TotalItems,pagedListMessageDtoResult.TotalPages);

            return pagedListMessageDtoResult;

        }

        [HttpGet("thread/{userName}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string userName)
        {
            var message= await _messageRepository.GetMessageThread(User.GetUserName(),userName);

            return Ok(message);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var message= await _messageRepository.GetMessage(id);
            var username=User.GetUserName();

            if(message.Sender.UserName != username && message.Recipient.UserName != username)
            {
                return Unauthorized();
            }

            if(message.Sender.UserName==username) message.SenderDeleted=true;
            if(message.Recipient.UserName==username) message.RecipientDeleted=true;

            if(await _messageRepository.SaveAllAsync()) return Ok();

            return BadRequest("Failed to delete the message");
        }
    }
}