using System;
using API.DTO;
using API.Entities;
using API.Interfaces;

namespace API.Extensions;

public static class AppUserExtension
{
   public static UserDTO ToUserDTO(this AppUser user, ITokenService tokenService)
   {
      
      return new UserDTO
      {
         Id = user.Id,
         DisplayName = user.DisplayName,
         Email = user.Email,
         ImageUrl = string.Empty,
         Token = tokenService.CreateToken(user)
        };
   }

}
