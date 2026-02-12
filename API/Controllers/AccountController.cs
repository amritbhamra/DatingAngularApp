using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using API.Entities;
using API.Data;
using API.DTO;
using Microsoft.EntityFrameworkCore;
using API.Properties;
using API.Interfaces;
using API.Extensions;

namespace API.Controllers
{
        public class AccountController(AppDbContext context, ITokenService tokenService) : BaseAPIController
    {
            [HttpPost("register")]
            public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
            {
                if (await EmailExists(registerDTO.Email)) return BadRequest("Email is already in use");
                using var hmac = new HMACSHA512();
                var user = new AppUser
                {
                    DisplayName = registerDTO.DisplayName,
                    Email = registerDTO.Email,
                    PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(registerDTO.Password)),
                    PasswordSalt = hmac.Key
                };

                // Save the user to the database (this part is not implemented yet)
                context.Users.Add(user);
                await context.SaveChangesAsync();

                var UserDTO = user.ToUserDTO(tokenService);
                return Ok(UserDTO);
            }

             [HttpGet("EmailExists")]
            private async Task<bool>EmailExists(string email)
            {
                 return await context.Users.AnyAsync(x => x.Email.ToLower() == email.ToLower());
            }
             
    
             [HttpPost("login")]
             public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
             {
                AppUser? user = await context.Users.SingleOrDefaultAsync(x => x.Email.ToLower() == loginDTO.Email.ToLower());
                // bool userExists = context.Users.Any(x => x.Email.ToLower() == loginDTO.Email.ToLower() && x.P);
                //  if (await EmailExists(loginDTO.Email)) return BadRequest("Email is already in use");
               
               if (user == null) return Unauthorized("Invalid email");

                    using var hmac = new HMACSHA512(user.PasswordSalt);
                    var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(loginDTO.Password));
                    for (int i = 0; i < computedHash.Length; i++)
                    {
                        if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
                    }   

                var UserDTO = user.ToUserDTO(tokenService);
                return Ok(UserDTO);

             }
    }
}
