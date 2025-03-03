using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentalHouse.Application.DTOs;
using RentalHouse.Application.Interfaces;
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

        [HttpPut]
        [Authorize]
        public async Task<ActionResult> UpdateUser(ChangeUserDTO changeUserDTO)
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
    }
}
