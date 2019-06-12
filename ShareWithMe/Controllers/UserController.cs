using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using SWM.Application.Users;
using SWM.Core.Users;
using ShareWithMe.Models;
using ShareWithMe.Common;

namespace ShareWithMe.Users
{
    [Route("api/[controller]/[action]")]

    public class UserController : ControllerBase
    {
        private readonly string StorageDirectory;
        private readonly IUserManager _userManager;
        private readonly IHostingEnvironment _env;

        public UserController(IUserManager userManager, IHostingEnvironment enviroment)
        {
            _userManager = userManager;
            _env = enviroment;

            StorageDirectory = Path.Combine(_env.WebRootPath, AppConsts.StorageDirectory);
            bool exists = Directory.Exists(StorageDirectory);

            if (!exists)
                Directory.CreateDirectory(StorageDirectory);
            exists = Directory.Exists(Path.Combine(_env.WebRootPath, AppConsts.ProfileImagesDirectory));

            if (!exists)
                Directory.CreateDirectory(Path.Combine(_env.WebRootPath, AppConsts.ProfileImagesDirectory));
        }

        [HttpPost]
        public async Task<JsonResult> Create([FromForm] CreateUserDto input)
        {
            if (ModelState.IsValid)
            {
                var user = CustomMapper<CreateUserDto, UserEntity>.Map(input);
                try
                {
                    user.Password = Password.Hash(user.Password);
                    user.Username = user.Username.ToLower();
                    user.Email = user.Email.ToLower();
                    user.FilesDirectory = Path.Combine(AppConsts.StorageDirectory, user.Username);
                    Directory.CreateDirectory(Path.Combine(_env.WebRootPath, user.FilesDirectory));
                    await _userManager.CreateAsync(user);
                    return new JsonResult(new ResponseModel("Success"));
                }
                catch (Exception ex)
                {
                    SqlException exp = ex.InnerException as SqlException;
                    List<string> errmsg = new List<string>();
                    if (exp.Number == 2601 || exp.Number == 2627)
                    {
                        string constraint = exp.Message.Substring(exp.Message.LastIndexOf('_') + 1);
                        errmsg.Add($"{constraint.Split('\'').First()} already exists");
                    }
                    return new JsonResult(new ResponseModel(code: HttpStatusCode.BadRequest, errors: errmsg, success: false));
                }
            }
            else
            {
                List<string> errorMsg = new List<string>();
                foreach (var key in ModelState.Values)
                {
                    foreach (var err in key.Errors)
                    {
                        errorMsg.Add(err.ErrorMessage);
                    }
                }
                return new JsonResult(new ResponseModel(code: HttpStatusCode.PartialContent, success: false, errors: errorMsg));
            }
        }

        [Authorize]
        [HttpPut]
        public async Task<JsonResult> Update([FromForm]UpdateUserDto input)
        {
            if (ModelState.IsValid)
            {
                var user = CustomMapper<UpdateUserDto, UserEntity>.Map(input);
                user.Password = Password.Hash(user.Password);
                user.Username = user.Username.ToLower();
                user.Email = user.Email.ToLower();
                try
                {
                    await _userManager.UpdateAsync(user);
                    return new JsonResult(new ResponseModel("Success"));
                }
                catch (Exception ex)
                {
                    SqlException exp = ex.InnerException as SqlException;
                    List<string> errmsg = new List<string>();
                    if (exp.Number == 2601 || exp.Number == 2627)
                    {
                        string constraint = exp.Message.Substring(exp.Message.LastIndexOf('_') + 1);
                        errmsg.Add($"{constraint.Split('\'').First()} already exists");
                    }
                    return new JsonResult(new ResponseModel(code: HttpStatusCode.BadRequest, errors: errmsg, success: false));
                }
            }
            else
            {
                List<string> errorMsg = new List<string>();
                foreach (var key in ModelState.Values)
                {
                    foreach (var err in key.Errors)
                    {
                        errorMsg.Add(err.ErrorMessage);
                    }
                }
                return new JsonResult(new ResponseModel(code: HttpStatusCode.PartialContent, success: false, errors: errorMsg));
            }
        }

        [Authorize]
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
            var user = CustomMapper<UserEntity, LimitedUserDto>.Map(userEntity);
            return new JsonResult(new ResponseModel(code: HttpStatusCode.OK, message: "Success", data: user));
        }

        [Authorize]
        [HttpGet]
        public async Task<JsonResult> GetAll()
        {
            long userId = long.Parse(User.Identity.Name);
            var entities = await _userManager.GetAll();
            List<UserDto> users = CustomMapper<UserEntity, UserDto>.MapList(entities.Where(u => u.Id != userId).ToList());
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
        }

    }
}