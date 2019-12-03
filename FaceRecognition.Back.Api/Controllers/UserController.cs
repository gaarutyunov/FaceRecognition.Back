using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FaceRecognition.Back.Api.Attributes;
using FaceRecognition.Back.Api.Dtos;
using FaceRecognition.Back.Api.Interfaces;
using FaceRecognition.Back.Api.Models;
using FaceRecognition.Back.Api.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FaceRecognition.Back.Api.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;

        public UserController(IUserService userService, IMapper mapper, IOptions<AppSettings> appSettings)
        {
            _userService = userService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        [AllowAnonymous]
        [ValidateModel]
        [HttpPost]
        public async Task<ActionResult> Register([FromBody] CreateUserDto dto)
        {
            var user = await _userService.Register(dto);

            return Ok(user);
        }

        [AllowAnonymous]
        [ValidateModel]
        [HttpPost]
        public async Task<ActionResult<TokenResponse>> Login([FromBody] LoginUserDto dto)
        {
            var user = await _userService.Login(dto);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new TokenResponse
            {
                Token = tokenHandler.WriteToken(token)
            });
        }
    }
}