using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;

        public AdminController(DataContext dataContext, UserManager<User> userManager)
        {
            this._context = dataContext;
            _userManager = userManager;
        }

        [Authorize(Policy = SD.AdminOnly)]
        [HttpGet("userWithRoles")]
        public async Task<IActionResult> GetUserWithRoles()
        {
            var userWithRoles = await _context.Users.OrderBy(x => x.UserName).Select(user => new
            {
                Id = user.Id,
                UserName = user.UserName,
                Roles = (from userRoles in user.UserRoles join role in _context.Roles on userRoles.RoleId equals role.Id select role.Name).ToList()
            }).ToListAsync();

            return Ok(userWithRoles);

        }

        [Authorize(Policy = SD.PhotoModerate)]
        [HttpPost("editRoles/{username}")]
        public async Task<IActionResult> EditRoles(string username, RoleEditDto editedRoles)
        {
            var resultUser = await _userManager.FindByNameAsync(username);

            if (resultUser == null) return BadRequest("user does not exist");

            var existedRoles = await _userManager.GetRolesAsync(resultUser);
            var selectedRoles = editedRoles.RolesName;
            selectedRoles = selectedRoles ?? new string[] { };

            var result = await _userManager.AddToRolesAsync(resultUser, selectedRoles.Except(existedRoles));
            if (result.Succeeded)
            {
                result = await _userManager.RemoveFromRolesAsync(resultUser, existedRoles.Except(selectedRoles));
                if (!result.Succeeded) return BadRequest("failed to remove the roles");

                return Ok(await _userManager.GetRolesAsync(resultUser));
                     
            }

            return BadRequest("failed to add roles");
            
        }

    }
}