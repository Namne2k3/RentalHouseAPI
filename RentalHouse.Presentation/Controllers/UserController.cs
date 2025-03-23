using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentalHouse.Application.DTOs;
using RentalHouse.Application.Interfaces;
using RentalHouse.SharedLibrary.Responses;
using System.Security.Claims;

namespace RentalHouse.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[EnableCors("AllowOrigin")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _repository;
        public UserController(IUserRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("getAllUsers")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ResponseWithDataList>> GetAllUsers()
        {
            var users = await _repository.GetAllUsers();
            return users.Any() ? Ok(new ResponseWithDataList()
            {
                IsSuccess = true,
                Message = "Danh sách người dùng",
                Data = users
            }) : NotFound(new ResponseWithDataList()
            {
                IsSuccess = false,
                Message = "Không có người dùng nào",
                Data = null!
            });
        }

        [HttpPut("updateUser")]
        [Authorize]
        public async Task<ActionResult<Response>> UpdateUser(ChangeUserDTO changeUserDTO)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userId = int.Parse(userIdClaim!);

            if (userId != changeUserDTO.Id)
            {
                return Forbid("Không thể cập nhật thông tin của tài khoản khác!");
            }

            var result = await _repository.UpdateUser(changeUserDTO);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPut("changePassword")]
        [Authorize]
        public async Task<ActionResult<Response>> ChangePassword([FromBody] ChangePasswordDTO changePasswordDTO)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userId = int.Parse(userIdClaim!);

            if (userId < 1)
            {
                return Forbid("Chưa đăng nhập!");
            }

            var result = await _repository.ChangePassword(userId, changePasswordDTO.newPassword, changePasswordDTO.currentPassword);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
