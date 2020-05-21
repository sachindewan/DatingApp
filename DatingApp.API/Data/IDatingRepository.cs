using DatingApp.API.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.API
{
    public interface IDatingRepository
    {
        void Add<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;

        Task<bool> SaveAll();

        Task<IEnumerable<User>> GetUsers();
        Task<User> GetUser(int userId);
        Task<Photo> GetPhoto(int Id);

        Task<Photo> GetMainPhotoForUser(int userId);

    }
}
