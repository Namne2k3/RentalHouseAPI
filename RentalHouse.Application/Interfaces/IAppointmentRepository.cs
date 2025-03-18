using RentalHouse.Application.DTOs;
using RentalHouse.Domain.Entities.Appointments;
using RentalHouse.SharedLibrary.Responses;

namespace RentalHouse.Application.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<Response> CreateAppointmentAsync(CreateAppointmentDTO dto, int userId);
        Task<IEnumerable<Appointment>> GetUserAppointmentsAsync(int userId);
        Task<IEnumerable<Appointment>> GetOwnerAppointmentsAsync(int ownerId);
        Task<Response> UpdateAppointmentStatusAsync(int appointmentId, string status);
    }

}
