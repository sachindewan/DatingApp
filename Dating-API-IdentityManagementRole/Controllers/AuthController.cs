using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public IConfiguration _configuration { get; }
        public AuthController(IConfiguration configuration, IMapper mapper,UserManager<User> userManager,RoleManager<Role> roleManager)
        {
            _configuration = configuration;
            this._mapper = mapper;
            this._userManager = userManager;
            this._roleManager = roleManager;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            //validate request

            userForRegisterDto.UserName = userForRegisterDto.UserName.ToLower();

            if (await _userManager.FindByNameAsync(userForRegisterDto.UserName) ==null)
                return BadRequest("User already register");

            var userToCreate = _mapper.Map<User>(userForRegisterDto);

            await _userManager.CreateAsync(userToCreate, userForRegisterDto.Password);
            await _userManager.AddToRoleAsync(userToCreate, "Member");

           // var createdUser = await _authRepository.Register(userToCreate, userForRegisterDto.Password);

            var userToReturn = _mapper.Map<UserForRegisterDto>(userToCreate);
            return CreatedAtRoute("GetUser",new { controller = "User" , id= userToCreate.Id},userToReturn);
         
        }

        //[HttpPost("login")]
        //public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        //{
        //    var userFromRepo = await _authRepository.Login(userForLoginDto.UserName.ToLower(), userForLoginDto.Password);
        //    if (userFromRepo == null) return Unauthorized();

        //    var claims = new Claim[]
        //    {
        //        new Claim(ClaimTypes.NameIdentifier,userFromRepo.Id.ToString()),
        //        new Claim(ClaimTypes.Name,userFromRepo.UserName)
        //    };

        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:token").Value));

        //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        //    var tokenDescriptor = new SecurityTokenDescriptor()
        //    {
        //        Subject = new ClaimsIdentity(claims),
        //        Expires = DateTime.Now.AddDays(1),
        //        SigningCredentials = creds
        //    };

        //    var tokenHandler = new JwtSecurityTokenHandler();

        //    var token = tokenHandler.CreateToken(tokenDescriptor);

        //    var user = _mapper.Map<UserForListDto>(userFromRepo);

        //    return Ok(new
        //    {
        //        token = tokenHandler.WriteToken(token),
        //        user
        //    });
        //}
    }
}