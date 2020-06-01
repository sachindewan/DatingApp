using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DatingRepository : IDatingRepository
    {
        public DataContext _context { get; }
        public DatingRepository(DataContext context)
        {
            _context = context;
        }

        public void Add<T>(T entity) where T : class
        {
            _context.Add<T>(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove<T>(entity);
        }

        public async Task<User> GetUser(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(d=>d.Id==userId);
            return user;
        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var users =  _context.Users.OrderByDescending(o=>o.LastActive).AsQueryable();
            users = users.Where(u => u.Id != userParams.UserId);
            users = users.Where(u => u.Gender == userParams.Gender);
            if(userParams.MinAge!=18 || userParams.Maxage != 99)
            {
                var minDob = DateTime.Today.AddYears(-userParams.Maxage - 1);
                var maxAge = DateTime.Today.AddYears(-userParams.MinAge - 1);
                users = users.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxAge);
            }
            if (userParams.Likers)
            {
                var userLikers = await GetUserLikes(userParams.UserId, userParams.Likers);

                users = users.Where(x => userLikers.Contains(x.Id));
            }
            if (userParams.Likees)
            {
                var userLikees = await GetUserLikes(userParams.UserId, userParams.Likers);

                users = users.Where(x => userLikees.Contains(x.Id));
            }
            if (!string.IsNullOrEmpty(userParams.OrderBy))
            {
                switch (userParams.OrderBy)
                {
                    case "created": 
                        users=users.OrderByDescending(o => o.Created);
                        break;
                    default:
                        users = users.OrderByDescending(o => o.LastActive);
                        break;

                }
            }
            var pagedList = await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
            return pagedList;
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Photo> GetPhoto(int Id)
        {
            var photo = await _context.Photos.FirstOrDefaultAsync(u => u.Id == Id);
            return photo;
        }

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            var mainPhotFroUser = await _context.Photos.Where(user => user.UserId == userId).FirstOrDefaultAsync(p => p.IsMain);
            return mainPhotFroUser;
        }

        public async Task<Like> GetLike(int userId, int reciepentId)
        {
            var like =await _context.Likes.FirstOrDefaultAsync(x=>x.LikerId==userId && x.LikeeId ==reciepentId);
            return like;
        }

        private async Task<IEnumerable<int>> GetUserLikes(int id, bool likers)
        {
            var users = await _context.Users.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (likers)
            {
                return users.Likers.Where(x => x.LikeeId == id).Select(x => x.LikerId);
            }
            else
            {
                return users.Likees.Where(x => x.LikerId == id).Select(x => x.LikeeId);
            }
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams)
        {
            var messages = _context.Messages.AsQueryable();
            switch (messageParams.MessageContainer)
            {
                case "Inbox":
                    messages = messages.Where(m => m.RecieverId == messageParams.UserId && m.Recieverdeleted==false);
                    break;
                case "Outbox":
                    messages = messages.Where(m => m.SenderId == messageParams.UserId && m.Senderdeleted==false);
                    break;
                default:
                    messages = messages.Where(m => m.RecieverId == messageParams.UserId && m.Recieverdeleted==false && m.IsRead == false);
                    break;
            }
            messages = messages.OrderByDescending(d => d.MessageSent);
            return await PagedList<Message>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);

        }

        public async Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId)
        {
            var messages = await _context.Messages.Where(m => m.RecieverId == userId && m.SenderId == recipientId && m.Recieverdeleted==false || m.RecieverId == recipientId && m.SenderId == userId && m.Senderdeleted==false)
                .OrderByDescending(m => m.MessageSent)
                .ToListAsync();
            return messages;
        }
    }
}
