using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interface;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _contex;
        private readonly IMapper _mapper;
        public MessageRepository(DataContext contex, IMapper mapper)
        {
            _mapper = mapper;
            _contex = contex;
        }

        public void AddMessage(Message message)
        {
            _contex.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _contex.Messages.Remove(message);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _contex.Messages.Include(u => u.Sender).ThenInclude(p => p.Photos)
            .Include(u => u.Recipient).ThenInclude(p => p.Photos).SingleOrDefaultAsync(x=> x.Id == id);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = _contex.Messages.OrderByDescending(m => m.MessageSent).AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(u => u.Recipient.UserName == messageParams.Username && u.RecipientDeleted == false),
                "Outbox" => query.Where(u => u.Sender.UserName == messageParams.Username && u.SenderDeleted == false),
                _ => query.Where(u => u.Recipient.UserName == messageParams.Username && u.DateRead == null && u.RecipientDeleted == false)
            };

            var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);
            return await PagedList<MessageDto>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);


        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername)
        {
            var messages = await _contex.Messages
            .Include(u => u.Sender).ThenInclude(p => p.Photos)
            .Include(u => u.Recipient).ThenInclude(p => p.Photos)
            .Where(m => m.Recipient.UserName == currentUsername && m.RecipientDeleted == false
            && m.Sender.UserName == recipientUsername || m.Recipient.UserName == recipientUsername
            && m.Sender.UserName == currentUsername && m.SenderDeleted == false)
            .OrderBy(m => m.MessageSent).ToListAsync();

            var unreadMessages = messages.Where(m => m.DateRead == null && m.Recipient.UserName == currentUsername).ToList();

            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.Now;
                }
                await _contex.SaveChangesAsync();
            }
            var mmm = _mapper.Map<IEnumerable<MessageDto>>(messages);
            return mmm;

        }

        public async Task<bool> SaveAllAsync()
        {
            return await _contex.SaveChangesAsync() > 0;
        }
    }
}