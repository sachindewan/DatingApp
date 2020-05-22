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
            var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(d=>d.Id==userId);
            return user;
        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var users =  _context.Users.Include(p => p.Photos).OrderByDescending(o=>o.LastActive).AsQueryable();
            users = users.Where(u => u.Id != userParams.UserId);
            users = users.Where(u => u.Gender == userParams.Gender);
            if(userParams.MinAge!=18 || userParams.Maxage != 99)
            {
                var minDob = DateTime.Today.AddYears(-userParams.Maxage - 1);
                var maxAge = DateTime.Today.AddYears(-userParams.MinAge - 1);
                users = users.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxAge);
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

        public Task<Photo> GetMainPhotoForUser(int userId)
        {
            var mainPhotFroUser = _context.Photos.Where(user => user.UserId == userId).FirstOrDefaultAsync(p => p.IsMain);
            return mainPhotFroUser;
        }
    }
}
