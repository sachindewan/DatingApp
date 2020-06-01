using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.Data
{
    public class Seed
    {
        public static void SeedUser(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            if (!userManager.Users.Any())
            {
                var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");
                var users = JsonConvert.DeserializeObject<List<User>>(userData);
                //adding role in Role table
                var roles = new List<Role>(){
                    new Role(){ Name = SD.Admin},
                    new Role(){ Name = SD.Member},
                    new Role(){ Name = SD.Moderator},
                };

                foreach (var role in roles)
                {
                    roleManager.CreateAsync(role).GetAwaiter().GetResult();
                }
                foreach (var user in users)
                {
                    user.UserName = user.UserName.ToLower();
                    var result = userManager.CreateAsync(user,"password").GetAwaiter().GetResult();
                    if (result.Succeeded)
                    {
                        userManager.AddToRoleAsync(user, SD.Member).GetAwaiter().GetResult();
                    }
                }

                var adminUser = new User()
                {
                    UserName = "Admin",
                };

                var resultAdmin = userManager.CreateAsync(adminUser,"Admin@123").GetAwaiter().GetResult();
                if (resultAdmin.Succeeded)
                {
                    var admin = userManager.FindByNameAsync(adminUser.UserName).GetAwaiter().GetResult();
                    userManager.AddToRolesAsync(admin, new[] { SD.Admin,SD.Moderator }).GetAwaiter().GetResult();
                }
            }
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
