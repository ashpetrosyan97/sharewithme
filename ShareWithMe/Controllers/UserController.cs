using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using SWM.Application.Users;
using SWM.Core.Users;
using ShareWithMe.Models;
using ShareWithMe.Common;
using SWM.Helpers;

namespace ShareWithMe.Users
{
    [Route("api/[controller]/[action]")]
    public class UserController : ControllerBase
    {
        private readonly string StorageDirectory;
        private readonly IUserManager _userManager;
        private readonly IWebHostEnvironment _env;
        private readonly Mapper mapper;

        public UserController(IUserManager userManager, IWebHostEnvironment enviroment, Mapper mapper)
        {
            _userManager = userManager;
            _env = enviroment;
            this.mapper = mapper;

            StorageDirectory = Path.Combine(_env.WebRootPath, AppConsts.StorageDirectory);
            bool exists = Directory.Exists(StorageDirectory);

            if (!exists)
                Directory.CreateDirectory(StorageDirectory);
            exists = Directory.Exists(Path.Combine(_env.WebRootPath, AppConsts.ProfileImagesDirectory));

            if (!exists)
                Directory.CreateDirectory(Path.Combine(_env.WebRootPath, AppConsts.ProfileImagesDirectory));
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> Create([FromForm] CreateUserDto input)
        {

            var user = mapper.Map<CreateUserDto, User>(input);
            user.FilesDirectory = Path.Combine(AppConsts.StorageDirectory, user.Username);
            Directory.CreateDirectory(Path.Combine(_env.WebRootPath, user.FilesDirectory));
            await _userManager.CreateAsync(user);
            return mapper.Map<User, UserDto>(user);
        }

        [Authorize]
        [HttpPut]
        public async Task<ActionResult<UserDto>> Update([FromForm]UpdateUserDto input)
        {
            var user = mapper.Map<UpdateUserDto, User>(input);
            await _userManager.UpdateAsync(user);
            return mapper.Map<User, UserDto>(user);
        }

        /* [Authorize]
         [HttpPut]
         public async Task<JsonResult> UpdateProfileImage([FromForm] UpdateProfileImageDto input)
         {
             var user = await _userManager.GetAsync(u => u.Id == input.Id);
             string filePath = Path.Combine(AppConsts.ProfileImagesDirectory, $"{user.Username}{Path.GetExtension(input.Img.FileName)}");
             using (var stream = new FileStream(Path.Combine(_env.WebRootPath, filePath), FileMode.Create))
             {
                 await input.Img.CopyToAsync(stream);
             }
             user.ProfileImage = filePath;
             await _userManager.UpdateAsync(user);
             return new JsonResult(new ResponseModel(message: "Success"));

         }

         [HttpGet]
         public async Task<JsonResult> Get(string username)
         {

             var userEntity = await _userManager.GetAsync(u => u.Username == username);
             if (userEntity == null)
             {
                 return new JsonResult(new ResponseModel(message: $"There is no user with username {username}", code: HttpStatusCode.BadRequest, success: false));
             }
             var user = mapper.Map<User, LimitedUserDto>.Map(userEntity);
             return new JsonResult(new ResponseModel(code: HttpStatusCode.OK, message: "Success", data: user));
         }

         [Authorize]
         [HttpGet]
         public async Task<JsonResult> GetAll()
         {
             long userId = long.Parse(User.Identity.Name);
             var entities = await _userManager.GetAll();
             List<UserDto> users = mapper.Map<User, UserDto>.MapList(entities.Where(u => u.Id != userId).ToList());
             return new JsonResult(new ResponseModel(message: "Success", data: users));
         }

         [Authorize]
         [HttpPost]
         public async Task<JsonResult> Delete(long id)
         {
             var user = await _userManager.GetAsync(u => u.Id == id);
             if (user == null)
             {
                 return new JsonResult(new ResponseModel(message: $"There is no user with id {id}", code: HttpStatusCode.BadRequest, success: false));
             }
             await _userManager.DeleteAsync(user);
             return new JsonResult(new ResponseModel(message: "User succesfully deleted"));
         }*/

    }
}