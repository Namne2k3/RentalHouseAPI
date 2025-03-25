using RentalHouse.Application.DTOs;

namespace RentalHouse.Application.Interfaces
{
    public interface IAppointmentHistoryService
    {
        Task AddHistoryAsync(int appointmentId, string status, string notes, int changedById);
        Task<IEnumerable<AppointmentHistoryDto>> GetAppointmentHistoryAsync(int appointmentId);
    }
}
