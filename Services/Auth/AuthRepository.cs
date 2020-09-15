using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using Dotnet_Core_Web_API.Data;
using Dotnet_Core_Web_API.Dto.UserDto;
using Dotnet_Core_Web_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;

namespace Dotnet_Core_Web_API.Services.Auth
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _context;
        public AuthRepository(AppDbContext context)
        {
            this._context = context;
        }
        public async Task<ServiceResponse<string>> Login(LoginDto loginUser)
        {
            ServiceResponse<string> response = new ServiceResponse<string>();
            User user = await this._context.Users.FirstOrDefaultAsync(u => u.Username.ToLower() == loginUser.Username.ToLower());
            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found";
            }
            else if (!this.VerifyPassword(loginUser.Password, user.PasswordHash, user.PasswordSalt))
            {
                response.Success = false;
                response.Message = "Wrong password.";
            }
            else
                response.Data = this.CreateToken(user);
            return response;
        }

        public async Task<ServiceResponse<int>> Register(RegisterDto registerUser)
        {
            ServiceResponse<int> response = new ServiceResponse<int>();
            if (!await this.UserExists(registerUser.Username))
            {
                this.CreatePasswordHash(registerUser.Password, out byte[] passwordHash, out byte[] passwordSalt);
                User user = new User { Username = registerUser.Username };
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                await this._context.Users.AddAsync(user);
                await this._context.SaveChangesAsync();
                response.Data = user.Id;
            }
            else
            {
                response.Success = false;
                response.Message = "User already exists";
            }
            return response;
        }

        public async Task<bool> UserExists(string username)
        {
            if (await this._context.Users.AnyAsync(u => u.Username.ToLower() == username.ToLower()))
                return true;
            return false;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hamc = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computeHash = hamc.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < passwordHash.Length; i++)
                    if (passwordHash[i] != computeHash[i])
                        return false;
                return true;
            }
        }

        private string CreateToken(User user)
        {
            dynamic config = JObject.Parse(File.ReadAllText("myConfig.json"));
            string configToken = config.AppSettings.Token;
            List<Claim> claims = new List<Claim>{
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name,user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };
            SymmetricSecurityKey key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(configToken));
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            SecurityToken token = handler.CreateToken(descriptor);
            return handler.WriteToken(token);
        }
    }
}