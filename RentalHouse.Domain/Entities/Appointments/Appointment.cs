using RentalHouse.Domain.Entities.Auth;
using RentalHouse.Domain.Entities.NhaTros;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentalHouse.Domain.Entities.Appointments
{
    public class Appointment
    {
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }

        [ForeignKey("NhaTro")]
        public int NhaTroId { get; set; }
        public NhaTro NhaTro { get; set; }

        [ForeignKey("Owner")]
        public int OwnerId { get; set; }
        public User Owner { get; set; }

        public DateTime AppointmentTime { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Pending"; // Mặc định là Pending

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        // Thêm trường để lưu lý do hủy (nếu có)
        [MaxLength(500)]
        public string? CancellationReason { get; set; }

        // Thêm trường để lưu ghi chú
        [MaxLength(500)]
        public string? Notes { get; set; }

        // Thêm trường để lưu thời gian xác nhận
        public DateTime? ConfirmedAt { get; set; }

        // Thêm trường để lưu người xác nhận
        [ForeignKey("ConfirmedBy")]
        public int? ConfirmedById { get; set; }
        public User? ConfirmedBy { get; set; }
    }

}
