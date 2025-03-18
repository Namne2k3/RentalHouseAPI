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

        public AppointmentRepository(IRentalHouseDbContext context)
        {
            _context = context;
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

        public async Task<IEnumerable<Appointment>> GetUserAppointmentsAsync(int userId)
        {
            return await _context.Appointments
                .Where(a => a.UserId == userId)
                .Include(a => a.User)
                .Include(a => a.NhaTro)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetOwnerAppointmentsAsync(int ownerId)
        {
            return await _context.Appointments
                .Where(a => a.OwnerId == ownerId)
                .Include(a => a.User)
                .Include(a => a.NhaTro)
                .ToListAsync();
        }

        public async Task<Response> UpdateAppointmentStatusAsync(int appointmentId, string status)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment == null)
                return new Response(false, "Lịch hẹn không tồn tại!");

            appointment.Status = status;
            appointment.UpdatedAt = DateTime.UtcNow;

            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();

            return new Response(true, "Cập nhật trạng thái thành công!");
        }
    }

}
