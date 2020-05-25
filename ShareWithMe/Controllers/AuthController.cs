using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using SWM.Core.Users;
using SWM.Core.Files;
using SWM.Application;
using SWM.Application.Users;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.AspNetCore.SignalR;
using ShareWithMe.Hubs;
using ShareWithMe.Models;
using ShareWithMe.Common;
using Microsoft.Extensions.Logging;

namespace ShareWithMe.Controllers
{
    [Route("api/[controller]/[action]")]
    public class AuthController : ControllerBase
    {
        private IConfiguration _configuration { get; }
        private readonly IUserManager _userManager;
        private readonly IHostingEnvironment _env;
       // private readonly SmtpClient _smtpClient;
        private readonly IFileManager _fileManager;
        private readonly IHubContext<ProgressHub, IProgressHub> _hub;
        ILogger<AuthController> _logger;
        public AuthController(IUserManager userManager, IConfiguration config, IFileManager fileManager, IHostingEnvironment env,  IHubContext<ProgressHub, IProgressHub> hub, ILogger<AuthController> logger/*, SmtpClient smtpClient*/)
        {
            _logger = logger;
            _hub = hub;
            _fileManager = fileManager;
           // _smtpClient = smtpClient;
            _env = env;
            _userManager = userManager;
            _configuration = config;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<JsonResult> Login([FromForm]AuthenticationModel login)
        {
            _logger.LogInformation("test");
            var user = await _userManager.GetAsync(x => x.Username == login.UserName.ToLower());
            if (user == null)
            {
                return new JsonResult(new ResponseModel(errors: new List<string> { $"There is no registered user with username {login.UserName}" },
                    success: false,
                    code: HttpStatusCode.BadRequest,
                    message: "Authentication failed!"
                    ));
            }

            if (!Password.Verify(string.IsNullOrEmpty(login.Password) ? "" : login.Password, user.Password))
            {
                return new JsonResult(new ResponseModel(
                    errors: new List<string> { "Wrong username or password" },
                    success: false,
                    code: HttpStatusCode.BadRequest,
                    message: "Authentication failed!"
                    ));
            }

            user.LastLoginTime = DateTime.Now;
            await _userManager.UpdateAsync(user);

            var tokenString = GenerateJSONWebToken(user.Id);
            var userDetails = CustomMapper<UserEntity, UserDto>.Map(user);
            return new JsonResult(new ResponseModel(
                    code: HttpStatusCode.OK,
                    message: "Success",
                    data: new
                    {
                        user = userDetails,
                        expires = (DateTime.Now.AddDays(1) - DateTime.Now).TotalSeconds,
                        accessToken = tokenString,
                    }));
        }

        [HttpGet]
        [Authorize]
        public async Task<JsonResult> GetCurrentUserDetails()
        {
            var header = Request.Headers["Authorization"];
            if (header.ToString().StartsWith("Bearer "))
            {
                var credValue = header.ToString().Substring("Bearer ".Length).Trim();
                long id = GetAuthentiactedUserId(credValue);
                var User = await _userManager.GetAsync(u => u.Id == id);
                if (User == null)
                {
                    return new JsonResult(new ResponseModel(errors: new List<string> { $"There is no registered user with id {id}" },
                        success: false,
                        code: HttpStatusCode.BadRequest,
                        message: "Authentication failed!"
                        ));
                }
                var userDetails = CustomMapper<UserEntity, UserDto>.Map(User);
                return new JsonResult(new ResponseModel(code: HttpStatusCode.OK, message: "Success", data: new { user = userDetails }));
            }

            return new JsonResult(new ResponseModel(errors: new List<string> { $"Wrong authentication type detected.Use Bearer token" },
                        success: false,
                        code: HttpStatusCode.BadRequest,
                        message: "Authentication failed!"
                        )); ;
        }

      /* [HttpPost]
        public async Task<JsonResult> ResetPassword([FromForm]ResetPasswordDto input)
        {
            var user = await _userManager.GetAsync(u => u.Username == input.Username && u.Email == input.Email);
            if (user == null)
            {
                return new JsonResult(new ResponseModel(message: $"There is no user with email {user.Email}", code: HttpStatusCode.BadRequest, success: false));
            }
            try
            {
                string psw = Password.Generate();

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
                user.Password = Password.Hash(psw);
                await _userManager.UpdateAsync(user);
                return new JsonResult(new ResponseModel(message: "Success"));
            }
            catch (Exception)
            {
                return new JsonResult(new ResponseModel(message: "Something went wrong", code: HttpStatusCode.BadRequest, success: false));
            }

        }
        */

        private string GenerateJSONWebToken(long id)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Authentication:SecretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var claimsData = new[] { new Claim(ClaimTypes.Name, id.ToString()) };

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
