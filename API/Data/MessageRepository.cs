using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using API.Data;
using AutoMapper.QueryableExtensions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public MessageRepository(DataContext context,IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public void AddMessage(Message message)
        {
           _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Message> GetMessage(int Id)
        {
            return await _context.Messages.Include(u=>u.Sender).Include(u=>u.Recipient).FirstOrDefaultAsync(m=>m.Id==Id);
        }

        public async Task<PagedList<MessageDto>> GetMessages(MessageParams messageParams)
        {
            var query= _context.Messages.OrderByDescending(m=>m.DateSend).AsQueryable();

            query = messageParams.Container switch
            {
                "Outbox"=> query.Where(m=>m.SenderUserName==messageParams.UserName && m.SenderDeleted==false),
                "Inbox"=> query.Where(m=>m.RecipientUserName==messageParams.UserName && m.RecipientDeleted==false),
                _ => query.Where(m=>m.RecipientUserName==messageParams.UserName && m.DateRead==null && m.RecipientDeleted==false)
            };

            return await PagedList<MessageDto>.CreateAsync(query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider),messageParams.PageNumber,messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string senderUserName, string recipientUserName)
        {
            var messages= await _context.Messages.Include(messages=>messages.Sender).ThenInclude(UserDto=>UserDto.Photos)
                .Include(messages=>messages.Recipient).ThenInclude(UserDto=>UserDto.Photos)
                .Where(messages=>messages.SenderUserName==senderUserName && messages.RecipientUserName==recipientUserName && messages.SenderDeleted==false ||
                messages.SenderUserName==recipientUserName && messages.RecipientUserName==senderUserName && messages.RecipientDeleted==false).OrderBy(m=>m.DateSend).ToListAsync();

            var UnReadMessages= messages.Where(message=>message.DateRead==null && message.RecipientUserName==senderUserName).ToList();

            if(UnReadMessages.Any())
            {

            foreach(var Message in UnReadMessages)
            {
                Message.DateRead=DateTime.Now;
            }
             await SaveAllAsync();
            }

           

           return _mapper.Map<IEnumerable<MessageDto>>(messages);

        }

        public async Task<bool> SaveAllAsync()
        {
            return  await _context.SaveChangesAsync() > 0;  
        }
    }
}