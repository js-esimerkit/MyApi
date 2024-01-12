using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyApi.Controllers
{
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
            private readonly IConfiguration _configuration;

        public LoginController(Database db, IConfiguration configuration)
        {
            Db = db;
            _configuration=configuration;
        }


        // POST api/login
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] User body)
        {
            Console.WriteLine(body.username);
            Console.WriteLine(body.password);
            await Db.Connection.OpenAsync();
            var query = new Login(Db);
            var result = await query.GetPassword(body.username);

            if (result is null || !BCrypt.Net.BCrypt.Verify(body.password, result))
            {
                // authentication failed
                return new OkObjectResult(false);
            }
            else
            {
                // authentication successful
                var token = GenerateToken(body.username);
                return new OkObjectResult(token);
            }

        }

        private object GenerateToken(string user)
        {
            Console.WriteLine(user);
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("Jwt:Key")));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,user)

            };
            var token = new JwtSecurityToken(_configuration.GetValue<string>("Jwt:Issuer"),
                _configuration.GetValue<string>("Jwt:Audience"),
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials);


            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public Database Db { get; }
    }
}
