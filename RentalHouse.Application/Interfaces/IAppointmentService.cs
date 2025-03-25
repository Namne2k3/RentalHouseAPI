using RentalHouse.Application.DTOs;

namespace RentalHouse.Application.Interfaces
{
    public interface IAppointmentService
    {

        Task<AppointmentDetailDto> GetAppointmentDetailAsync(int appointmentId);
        Task UpdateAppointmentStatusAsync(int appointmentId, string newStatus, string notes, int changedById);
    }
}
