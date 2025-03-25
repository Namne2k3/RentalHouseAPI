using Microsoft.EntityFrameworkCore;
using RentalHouse.Application.DTOs;
using RentalHouse.Application.Interfaces;
using RentalHouse.Domain.Entities.Appointments;
using RentalHouse.Infrastructure.Data;

namespace RentalHouse.Infrastructure.Services
{
    public class AppointmentHistoryService : IAppointmentHistoryService
    {
        private readonly IRentalHouseDbContext _context;

        public AppointmentHistoryService(IRentalHouseDbContext context)
        {
            _context = context;
        }

        public async Task AddHistoryAsync(int appointmentId, string status, string notes, int changedById)
        {
            var history = new AppointmentHistory
            {
                AppointmentId = appointmentId,
                Status = status,
                Notes = notes,
                ChangedById = changedById,
                CreatedAt = DateTime.UtcNow
            };

            _context.AppointmentHistories.Add(history);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<AppointmentHistoryDto>> GetAppointmentHistoryAsync(int appointmentId)
        {
            return await _context.AppointmentHistories
                .Where(h => h.AppointmentId == appointmentId)
                .Include(h => h.ChangedBy)
                .OrderByDescending(h => h.CreatedAt)
                .Select(h => new AppointmentHistoryDto
                {
                    Id = h.Id,
                    Status = h.Status,
                    Notes = h.Notes,
                    ChangedBy = h.ChangedBy!.FullName,
                    CreatedAt = h.CreatedAt
                })
                .ToListAsync();
        }
    }
}
