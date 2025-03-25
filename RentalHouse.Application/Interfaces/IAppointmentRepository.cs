using RentalHouse.Application.DTOs;
using RentalHouse.SharedLibrary.Responses;

namespace RentalHouse.Application.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<Response> CreateAppointmentAsync(CreateAppointmentDTO dto, int userId);
        Task<IEnumerable<AppointmentDetailDto>> GetUserAppointmentsAsync(int userId);
        Task<IEnumerable<AppointmentDetailDto>> GetOwnerAppointmentsAsync(int ownerId);
        Task<Response> UpdateAppointmentStatusAsync(int appointmentId, string newStatus, string notes, int changedById);

        Task<AppointmentStatsDto> GetAppointmentStatsAsync(int userId, bool isOwner);
        Task<IEnumerable<AppointmentTimeStatsDto>> GetPopularAppointmentTimesAsync();
    }

}
