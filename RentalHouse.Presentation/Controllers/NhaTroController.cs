using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentalHouse.Application.DTOs;
using RentalHouse.Application.DTOs.Conversions;
using RentalHouse.Application.Interfaces;
using RentalHouse.SharedLibrary.Responses;
using System.Security.Claims;

namespace RentalHouse.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[EnableCors("AllowOrigin")]
    public class NhaTroController : ControllerBase
    {
        private readonly INhaTroRepository _repository;
        private readonly INhaTroService _service;
        public NhaTroController(INhaTroRepository repository, INhaTroService service)
        {
            _repository = repository;
            _service = service;
        }

        [HttpGet("GetNhaTros")]
        public async Task<ActionResult<IEnumerable<NhaTroDTO>>> GetNhaTros()
        {
            // cách để lấy ra thông tin được mã hóa trong token
            //var userId = User.FindFirst(ClaimTypes.Email)?.Value;
            var nhatros = await _repository.GetAllAsync();
            if (!nhatros.Any())
            {
                return NotFound("Không tìm thấy nhà trọ trong cơ sở dữ liệu!");
            }

            var (_, list) = NhaTroConversion.FromEntity(null, nhatros);
            return list!.Any() ? Ok(list) : NotFound("Không tìm thấy nhà trọ!");
        }

        [HttpGet("GetNhaTrosWithFilters")]
        public async Task<ActionResult<PagedResultDTO<NhaTroDTO>>> GetNhaTrosWithFilters([FromQuery] FilterNhaTroDTO filterNhaTroDTO)
        {
            // cách để lấy ra thông tin được mã hóa trong token
            //var userId = User.FindFirst(ClaimTypes.Email)?.Value;
            var nhatros = await _repository.GetAllAsyncWithFilters(
                filterNhaTroDTO.page != 0 ? filterNhaTroDTO.page : 1,
                filterNhaTroDTO.pageSize != 0 ? filterNhaTroDTO.pageSize : 20,
                filterNhaTroDTO.city!,
                filterNhaTroDTO.district!,
                filterNhaTroDTO.commune!,
                filterNhaTroDTO.street!,
                filterNhaTroDTO.address!,
                filterNhaTroDTO.price1!,
                filterNhaTroDTO.price2!,
                filterNhaTroDTO.area!,
                filterNhaTroDTO.bedRoomCount!
            );

            if (!nhatros.Data!.Any())
            {
                return NotFound("Không tìm thấy nhà trọ!");
            }

            return nhatros.Data!.Any() ? Ok(nhatros) : NotFound("Không tìm thấy nhà trọ!");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response>> CreateNhaTro(NhaTroDTO nhaTroDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var getNhaTro = NhaTroConversion.ToEntity(nhaTroDTO); ;
            var response = await _repository.CreateAsync(getNhaTro);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpDelete]
        [Authorize]
        public async Task<ActionResult<Response>> DeleteNhaTro(NhaTroDTO nhaTroDTO)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int userId = int.Parse(userIdClaim!);

            if (!await _service.IsOwner(nhaTroDTO.Id, userId))
            {
                return Forbid("Bạn không có quyền xóa thông tin nhà trọ này!");
            }

            var nhatro = NhaTroConversion.ToEntity(nhaTroDTO);
            var response = await _repository.DeleteAsync(nhatro);

            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }
    }
}
