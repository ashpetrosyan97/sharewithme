using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using ShareWithMe.Hubs;
using ShareWithMe.Models;
using SWM.Application.Users;
using SWM.Core.Users;
using SWM.Helpers;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ShareWithMe.Controllers
{
    [Route("api/[controller]/[action]")]
    public class AuthController : ControllerBase
    {
        private IConfiguration _configuration { get; }
        private readonly IUserManager _userManager;
        private readonly IWebHostEnvironment _env;
        private readonly SmtpClient _smtpClient;
        //private readonly IFileManager _fileManager;
        private readonly Mapper mapper;
        private readonly IHubContext<ProgressHub, IProgressHub> _hub;
        public AuthController(IUserManager userManager, IConfiguration config, /*IFileManager fileManager,*/ IWebHostEnvironment env, IHubContext<ProgressHub, IProgressHub> hub, SmtpClient smtpClient, Mapper map)
        {
            _hub = hub;
            mapper = map;
            //  _fileManager = fileManager;
            _smtpClient = smtpClient;
            _env = env;
            _userManager = userManager;
            _configuration = config;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<dynamic>> Login([FromForm]AuthenticationModel input)
        {
            var userEntity = await _userManager.GetAsync(x => x.Username == input.UserName.ToLower());
            if (userEntity == null)
                throw new CoreException($"There is no registered user with username {input.UserName}");

            var success = new PasswordHasher<User>().VerifyHashedPassword(userEntity, userEntity.PasswordHash, input.Password) == PasswordVerificationResult.Success;

            if (!success)
                throw new CoreException("Password is incorrect.", HttpStatusCode.BadRequest);


            userEntity.LastLoginTime = DateTime.Now;
            await _userManager.UpdateAsync(userEntity);

            var tokenString = GenerateJSONWebToken(userEntity.Id);
            var userDetails = mapper.Map<User, UserDto>(userEntity);
            return new { user = userDetails, expires = TimeSpan.FromDays(1).TotalMilliseconds, accessToken = tokenString };
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetCurrentUserDetails()
        {
            var header = Request.Headers["Authorization"];
            if (header.ToString().StartsWith("Bearer "))
            {
                var credValue = header.ToString().Substring("Bearer ".Length).Trim();
                long id = GetAuthentiactedUserId(credValue);
                var User = await _userManager.GetAsync(u => u.Id == id);
                if (User == null)
                    throw new CoreException($"There is no registered user with id {id}");
                return mapper.Map<User, UserDto>(User);
            }

            throw new CoreException($"Wrong authentication type detected.Use Bearer token");
        }

        [HttpPost]
        public async Task<ActionResult> ResetPassword([FromForm]ResetPasswordDto input)
        {
            var user = await _userManager.GetAsync(u => u.Username == input.Username && u.Email == input.Email);
            if (user == null)
            {
                return new JsonResult(new ResponseModel(message: $"There is no user with email {user.Email}", code: HttpStatusCode.BadRequest, success: false));
            }

            string psw = "random pswd";

            var msg = new MimeMessage
            {
                Subject = "Password reset",
                Body = new TextPart("plain")
                {
                    Text = $"Dear {user.Name} your password has been reseted.{Environment.NewLine} Your new password is {psw}"
                }
            };
            msg.From.Add(new MailboxAddress("Online Storage", "noreply@onlinestorage.com"));
            msg.To.Add(new MailboxAddress(user.Name, user.Email));
            await _smtpClient.SendAsync(msg);
            user.PasswordHash = new PasswordHasher<User>().HashPassword(user, psw);
            await _userManager.UpdateAsync(user);
            return Ok();
        }


        private string GenerateJSONWebToken(long id)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Authentication:SecretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var claimsData = new[] { new Claim(ClaimTypes.Name, $"{id}" )};

            var token = new JwtSecurityToken(
                    issuer: _configuration["Authentication:Issuer"],
                    audience: _configuration["Authentication:Audience"],
                    expires: DateTime.Now.AddDays(1),
                    claims: claimsData,
                    signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        protected long GetAuthentiactedUserId(string token)
        {
            string secret = _configuration["Authentication:SecretKey"];
            var key = Encoding.ASCII.GetBytes(secret);
            var handler = new JwtSecurityTokenHandler();
            var tokenSecure = handler.ReadToken(token) as SecurityToken;
            var validations = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };
            var claims = handler.ValidateToken(token, validations, out tokenSecure);
            return long.Parse(claims.Identity.Name);
        }

    }
}
