using Microsoft.EntityFrameworkCore;
using RentalHouse.Application.DTOs;
using RentalHouse.Application.Interfaces;
using RentalHouse.Domain.Entities.Appointments;
using RentalHouse.Infrastructure.Data;
using RentalHouse.SharedLibrary.Responses;

namespace RentalHouse.Infrastructure.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly IRentalHouseDbContext _context;
        private readonly IAppointmentHistoryService _historyService;

        public AppointmentRepository(IRentalHouseDbContext context, IAppointmentHistoryService historyService)
        {
            _context = context;
            _historyService = historyService;
        }

        public async Task<Response> CreateAppointmentAsync(CreateAppointmentDTO dto, int userId)
        {
            var nhatro = await _context.NhaTros.FindAsync(dto.NhaTroId);
            if (nhatro == null)
                return new Response(false, "Nhà trọ không tồn tại!");

            var appointment = new Appointment
            {
                UserId = userId,
                NhaTroId = dto.NhaTroId,
                OwnerId = nhatro.UserId, // Lấy chủ nhà trọ
                AppointmentTime = dto.AppointmentTime
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            return new Response(true, "Đã đặt lịch thành công!");
        }

        public async Task<IEnumerable<AppointmentDetailDto>> GetUserAppointmentsAsync(int userId)
        {
            return await _context.Appointments
                .Where(a => a.UserId == userId)
                .Include(a => a.NhaTro)
                .Include(a => a.Owner)
                .Select(a => new AppointmentDetailDto
                {
                    Id = a.Id,
                    NhaTroTitle = a.NhaTro.Title,
                    UserName = a.User.FullName,
                    OwnerName = a.Owner.FullName,
                    PhoneNumber = a.User.PhoneNumber,
                    Email = a.User.Email,
                    Address = a.NhaTro.Address,
                    AppointmentTime = a.AppointmentTime,
                    Status = a.Status,
                    CreatedAt = a.CreatedAt,
                    CompletedAt = a.CompletedAt,
                    Notes = a.Notes
                })
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<AppointmentDetailDto>> GetOwnerAppointmentsAsync(int ownerId)
        {
            return await _context.Appointments
                .Where(a => a.OwnerId == ownerId)
                .Include(a => a.NhaTro)
                .Include(a => a.User)
                .Select(a => new AppointmentDetailDto
                {
                    Id = a.Id,
                    NhaTroTitle = a.NhaTro.Title,
                    UserName = a.User.FullName,
                    OwnerName = a.Owner.FullName,
                    PhoneNumber = a.User.PhoneNumber,
                    Email = a.User.Email,
                    Address = a.NhaTro.Address,
                    AppointmentTime = a.AppointmentTime,
                    Status = a.Status,
                    CreatedAt = a.CreatedAt,
                    CompletedAt = a.CompletedAt,
                    Notes = a.Notes
                })
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<Response> UpdateAppointmentStatusAsync(int appointmentId, string status, string notes, int changerId)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment == null)
                return new Response(false, "Lịch hẹn không tồn tại!");

            appointment.Status = status;
            appointment.UpdatedAt = DateTime.UtcNow;

            _context.Appointments.Update(appointment);

            await _historyService.AddHistoryAsync(appointmentId, status, notes, changerId);

            await _context.SaveChangesAsync();

            return new Response(true, "Cập nhật trạng thái thành công!");
        }

        public async Task<AppointmentStatsDto> GetAppointmentStatsAsync(int userId, bool isOwner)
        {
            var query = isOwner
                ? _context.Appointments.Where(a => a.OwnerId == userId)
                : _context.Appointments.Where(a => a.UserId == userId);

            var appointmentStats = new AppointmentStatsDto
            {
                TotalAppointments = await query.CountAsync(),
                CompletedAppointments = await query.CountAsync(a => a.Status == "Completed"),
                PendingAppointments = await query.CountAsync(a => a.Status == "Pending"),
                CancelledAppointments = await query.CountAsync(a => a.Status == "Cancelled")
            };

            return appointmentStats;
        }

        public async Task<IEnumerable<AppointmentTimeStatsDto>> GetPopularAppointmentTimesAsync()
        {
            var now = DateTime.UtcNow;
            var thirtyDaysAgo = now.AddDays(-30);
            var appoiments = await _context.Appointments
                .Where(a => a.CreatedAt >= thirtyDaysAgo && a.Status != "Cancelled")
                .GroupBy(a => new { a.AppointmentTime.Date, a.AppointmentTime.Hour })
                .Select(g => new AppointmentTimeStatsDto
                {
                    TimeSlot = new DateTime(g.Key.Date.Year, g.Key.Date.Month, g.Key.Date.Day, g.Key.Hour, 0, 0),
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .ToListAsync();

            return appoiments;
        }
    }

}
