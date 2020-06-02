using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DatingApp.API.Controllers
{
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudnary;

        public PhotosController(IDatingRepository repo, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            this._repo = repo;
            this._mapper = mapper;
            this._cloudinaryConfig = cloudinaryConfig;

            Account account = new Account()
            {
                ApiKey = _cloudinaryConfig.Value.ApiKey,
                ApiSecret = _cloudinaryConfig.Value.ApiSecret,
                Cloud = _cloudinaryConfig.Value.CloudName
            };

            _cloudnary = new Cloudinary(account);
        }

        [HttpGet("{id}",Name ="GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromRepo = await _repo.GetPhoto(id);

            var photo = _mapper.Map<PhotoForResultDto>(photoFromRepo);
            return Ok(photo);
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, [FromForm]PhotoForCreationDto photoForCreationDto)
        {
            if (userId != int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
            var userFromRepo = await _repo.GetUser(userId);

            var file = photoForCreationDto.File;

            var uploadResult = new ImageUploadResult();

            if (file?.Length > 0)
            {
                using(var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.FileName, stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face"),
                    };
                    uploadResult = _cloudnary.Upload(uploadParams);
                }
            }

            photoForCreationDto.Url = uploadResult.Uri.ToString();
            photoForCreationDto.PublicId = uploadResult.PublicId;

            var photo = _mapper.Map<Photo>(photoForCreationDto);

            if (!userFromRepo.Photos.Any(u => u.IsMain))
            {
                photo.IsMain = true;
            }

            userFromRepo.Photos.Add(photo);

            if (await _repo.SaveAll())
            {
                var photoForReturn = _mapper.Map<PhotoForResultDto>(photo);
                return CreatedAtRoute("GetPhoto",new { userId= photo.UserId, id=photo.Id}, photoForReturn);
            }

            return BadRequest("Could not add the photo");
        }
        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int userId, int id)
        {
            if (userId != int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
            var userFromRepo = await _repo.GetUser(userId);
            if(!userFromRepo.Photos.Any(p=>p.Id == id))
            {
                return Unauthorized();
            }

            var photoFromRepo = await _repo.GetPhoto(id);

            if (photoFromRepo.IsMain)
            {
                return BadRequest("this is already the main photo");
            }

            var mainPhotoForUser = await _repo.GetMainPhotoForUser(userId);
            mainPhotoForUser.IsMain = false;

            photoFromRepo.IsMain = true;

            if (await _repo.SaveAll())
            {
                return NoContent();
            }

            return BadRequest("Could not set photo to main");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int userId, int id)
        {
            if (userId != int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
            var userFromRepo = await _repo.GetUser(userId);
            if (!userFromRepo.Photos.Any(p => p.Id == id))
            {
                return Unauthorized();
            }

            var photoFromRepo = await _repo.GetPhoto(id);

            if (photoFromRepo.IsMain)
            {
                return BadRequest("Main photo could not be deleted");
            }

            if (photoFromRepo.PublicId != null)
            {
                var deletParams = new DeletionParams(photoFromRepo.PublicId);

                var _result = _cloudnary.Destroy(deletParams);
                if (_result.Result == "ok")
                {
                    _repo.Delete(photoFromRepo);
                }
            }
            
            if(photoFromRepo.PublicId == null)
            {
                _repo.Delete(photoFromRepo);
            }

            if (await _repo.SaveAll())
            {
                return Ok();
            }

            return BadRequest("Failed to delete the photo");
        }
        [Authorize(Policy =SD.PhotoModerate)]
        [HttpGet("getUnApprovedPhoto")]
        public async Task<IActionResult> GetAllUnApprovedPhoto(int userId)
        {
            if (userId != int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
            var userFromRepo = await _repo.GetUsers();

            if (userFromRepo == null)
            {
                return NotFound();
            }
            List<Photo> photos = new List<Photo>();
            userFromRepo.ToList().ForEach(element =>
            {
                photos.AddRange(element.Photos.Where(p => !p.IsApproved).ToList());
            });
            var photosToReturn = _mapper.Map<IEnumerable<PhotoForDetailedDto>>(photos);
            return Ok(photosToReturn);
        }

        [HttpGet("approve/{Id}")]
        [Authorize(Policy = SD.PhotoModerate)]
        public async Task<IActionResult> Approve(int userId , int Id)
        {
            var photo = await _repo.GetPhoto(Id);
            if (photo == null) return BadRequest();

            photo.IsApproved = true;
            if (await _repo.SaveAll())
            {
                return Ok();
            }
            return BadRequest("failed to approve the photo");
        }
    }

}