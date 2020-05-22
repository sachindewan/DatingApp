using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IDatingRepository _datingRepository;
        private readonly IMapper _mapper;

        public UserController(IDatingRepository datingRepository, IMapper mapper)
        {
            _datingRepository = datingRepository;
            _mapper = mapper;
        }
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers([FromQuery]UserParams userParams)
        {
            var userId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userFromRepo = await _datingRepository.GetUser(userId);
            userParams.UserId = userId;
            if (string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = userFromRepo.Gender == "male" ? "female" : "male";
            }
            var users = await _datingRepository.GetUsers(userParams);
            var userToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);
            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
            return Ok(userToReturn);
        }
        [HttpGet("users/{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _datingRepository.GetUser(id);
            var userToReturn = _mapper.Map<UserForDetailsDto>(user);
            return Ok(userToReturn);
        }
        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto userForupdateDto)
        {
            if (id != int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
            var userFromRepo = await _datingRepository.GetUser(id);
            _mapper.Map(userForupdateDto, userFromRepo);
            if (await _datingRepository.SaveAll())
                return NoContent();
            throw new Exception($"Updating user {id} failed on save");

        }
    }
}