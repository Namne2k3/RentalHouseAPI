﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RentalHouse.Application.DTOs;
using RentalHouse.Application.Interfaces;
using RentalHouse.Domain.Entities.Auth;
using RentalHouse.Infrastructure.Data;
using RentalHouse.SharedLibrary.Responses;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RentalHouse.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IRentalHouseDbContext _context;
        private readonly IConfiguration _config;

        public UserRepository(IRentalHouseDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        private async Task<User> GetUserByEmail(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            return user is not null ? user : null!;
        }
        public async Task<GetUserDTO> GetUser(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            return user is not null
                ? new GetUserDTO(
                    user.Id,
                    user.FullName,
                    user.Email,
                    user.PhoneNumber!
                )
                : null!;
        }

        public async Task<LoginResponse> Login(LoginDTO loginDTO)
        {
            var getUser = await GetUserByEmail(loginDTO.Email);

            var getUserDTO = new GetUserDTO(
                Email: getUser.Email,
                PhoneNumber: getUser.PhoneNumber!,
                Id: getUser.Id,
                FullName: getUser.FullName
            );

            if (getUser is null)
            {
                return new LoginResponse(null!, false, "Sai thông tin email hoặc email chưa được đăng ký!");
            }

            bool verifyPassword = BCrypt.Net.BCrypt.Verify(loginDTO.Password, getUser.Password);
            if (!verifyPassword)
            {
                return new LoginResponse(null!, false, "Sai mật khẩu!");
            }
            string token = GenerateToken(getUser);
            return new LoginResponse(getUserDTO, true, token);
        }

        private string GenerateToken(User appUser)
        {
            var key = Encoding.UTF8.GetBytes(_config.GetSection("Authentication:Key").Value!);
            var securityKey = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new (ClaimTypes.Name, appUser.FullName!),
                new (ClaimTypes.NameIdentifier, appUser.Id.ToString()!),
                new (ClaimTypes.Email, appUser.Email!),
            };

            if (!string.IsNullOrEmpty(appUser.Role.ToString()) || !Equals("string", appUser.Role.ToString()))
            {
                claims.Add(new(ClaimTypes.Role, appUser.Role.ToString()!));
            }

            var token = new JwtSecurityToken(
                issuer: _config["Authentication:Issuer"],
                audience: _config["Authentication:Audience"],
                claims: claims,
                expires: null,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<Response> Register(UserDTO userDTO)
        {
            var getUser = await GetUserByEmail(userDTO.Email);
            if (getUser is not null)
            {
                return new Response(false, "Không thể sử dụng Email này!");
            }

            var result = _context.Users.Add(
                new User()
                {
                    FullName = userDTO.FullName,
                    Email = userDTO.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(userDTO.Password),
                    PhoneNumber = userDTO.PhoneNumber
                }
            ).Entity;

            await _context.SaveChangesAsync();
            return result.Id > 0
                ? new Response(true, "Tài khoản đã được đăng ký!")
                : new Response(false, "Sai thông tin đăng ký tài khoản!");
        }

        public async Task<Response> UpdateUser(ChangeUserDTO changeUserDTO)
        {
            var getUser = await GetUserByEmail(changeUserDTO.Email);
            if (getUser is null)
            {
                return new Response(false, "Không tìm thấy tài khoản!");
            }

            getUser.FullName = changeUserDTO.FullName;
            getUser.Email = changeUserDTO.Email;
            getUser.PhoneNumber = changeUserDTO.PhoneNumber;

            await _context.SaveChangesAsync();

            return new Response(true, "Cập nhật thông tin tài khoản thành công!");
        }

        public async Task<Response> ChangePassword(int userId, string newPassword, string currentPassword)
        {
            var getUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (getUser is null)
            {
                return new Response(false, "Không tìm thấy tài khoản!");
            }

            bool verifyPassword = BCrypt.Net.BCrypt.Verify(currentPassword, getUser.Password);
            if (!verifyPassword)
            {
                return new Response(false, "Sai mật khẩu!");
            }

            _context.Entry(getUser).State = EntityState.Modified;
            getUser.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _context.SaveChangesAsync();

            return new Response(true, "Cập nhật mật khẩu tài khoản thành công!");
        }
    }
}
