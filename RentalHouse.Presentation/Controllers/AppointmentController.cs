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
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentRepository _appointmentRepository;

        public AppointmentController(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;

        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentDTO dto)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var response = await _appointmentRepository.CreateAppointmentAsync(dto, int.Parse(userId));
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpGet("GetUserAppointments")]
        [Authorize]
        public async Task<ActionResult<List<AppointmentDTO>>> GetUserAppointments()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var appointments = await _appointmentRepository.GetUserAppointmentsAsync(int.Parse(userId));
            var listDto = appointments.Select(p => p.ToAppointmentDTO());
            return listDto.Any() ? Ok(listDto) : BadRequest(listDto);
        }

        [HttpGet("GetOwnerAppointments")]
        [Authorize]
        public async Task<ActionResult<List<AppointmentDTO>>> GetOwnerAppointments()
        {
            string ownerId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var appointments = await _appointmentRepository.GetOwnerAppointmentsAsync(int.Parse(ownerId));
            var listDto = appointments.Select(p => p.ToAppointmentDTO());
            return listDto.Any() ? Ok(listDto) : BadRequest(listDto);
        }

        [HttpPut("{appointmentId}")]
        [Authorize]
        public async Task<ActionResult<Response>> UpdateAppointmentStatus(int appointmentId, [FromBody] string status)
        {
            var response = await _appointmentRepository.UpdateAppointmentStatusAsync(appointmentId, status);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }
    }

}
