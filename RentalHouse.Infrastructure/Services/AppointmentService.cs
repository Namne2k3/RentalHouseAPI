using Microsoft.EntityFrameworkCore;
using RentalHouse.Application.DTOs;
using RentalHouse.Application.Interfaces;
using RentalHouse.Infrastructure.Data;

namespace RentalHouse.Infrastructure.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IRentalHouseDbContext _context;
        private readonly IAppointmentHistoryService _historyService;

        public AppointmentService(IRentalHouseDbContext context, IAppointmentHistoryService historyService)
        {
            _context = context;
            _historyService = historyService;
        }

        public async Task UpdateAppointmentStatusAsync(int appointmentId, string newStatus, string notes, int changedById)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment == null)
                throw new Exception("Không tìm thấy lịch hẹn");

            // Cập nhật trạng thái
            appointment.Status = newStatus;
            appointment.UpdatedAt = DateTime.UtcNow;

            // Thêm vào lịch sử
            await _historyService.AddHistoryAsync(appointmentId, newStatus, notes, changedById);

            await _context.SaveChangesAsync();
        }

        public async Task<AppointmentDetailDto> GetAppointmentDetailAsync(int appointmentId)
        {

            var appointment = await _context.Appointments
                .Include(a => a.NhaTro)
                .Include(a => a.User)
                .Include(a => a.Owner)
                .FirstOrDefaultAsync(a => a.Id == appointmentId);

            if (appointment == null)
                throw new Exception("Không tìm thấy lịch hẹn");

            var history = await _historyService.GetAppointmentHistoryAsync(appointmentId);

            return new AppointmentDetailDto
            {
                Id = appointment.Id,
                NhaTroTitle = appointment.NhaTro.Title,
                UserName = appointment.User.FullName,
                OwnerName = appointment.Owner.FullName,
                AppointmentTime = appointment.AppointmentTime,
                Status = appointment.Status,
                CreatedAt = appointment.CreatedAt,
                CompletedAt = appointment.CompletedAt,
                Notes = appointment.Notes,
                History = history
            };
        }
    }
}
