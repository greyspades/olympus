using System.Text.Json;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using User.Interface;
using Video.Model;
using Credentials.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Newtonsoft.Json.Linq;

namespace User.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IUserRepository _repo;
        readonly Guid guid = Guid.NewGuid();
        public UserController(IConfiguration config, IUserRepository repo)
        {
            _config = config;
            _repo = repo;
        }

        [HttpPost("e360")]
        public async Task<ActionResult> Login(AdminDto payload) {
            try {
            var data = await _repo.AdminAuth(payload);
            if(data != null) {
                var claims = new[]
            {
                new Claim(ClaimTypes.Sid, payload.Id),
                // Add additional claims as needed
            };

            // Create a JWT token
            var token = new JwtSecurityToken(
                issuer: "your_issuer",
                audience: "your_audience",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1), // Token expiration time
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your_secret_key_ludex")), SecurityAlgorithms.HmacSha256)
            );

               return Ok(new {
                    code = 200,
                    message = "Signed in successfully",
                    data.data,
                    token = new JwtSecurityTokenHandler().WriteToken(token)
               });
            } else {
                var response = new {
                    code = 500,
                    status = false,
                    message = "Unnable to complete your request"
                };

                return StatusCode(500, response);
            }
                
            } catch(Exception e) {
                Console.WriteLine(e.Message);
                using StreamWriter outputFile = new("logs.txt", true);

                await outputFile.WriteAsync(e.Message);

                var response = new {
                    code = 500,
                    status = false,
                    message = "Unnable to complete your request"
                };

                return StatusCode(500, response);
            }
        }
    }

    
}
