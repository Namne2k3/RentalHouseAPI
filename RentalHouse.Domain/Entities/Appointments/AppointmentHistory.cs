using RentalHouse.Domain.Entities.Auth;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentalHouse.Domain.Entities.Appointments
{
    public class AppointmentHistory
    {
        public int Id { get; set; }

        [ForeignKey("Appointment")]
        public int AppointmentId { get; set; }
        public Appointment? Appointment { get; set; }

        [Required]
        [MaxLength(20)]
        public string? Status { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        [ForeignKey("ChangedBy")]
        public int ChangedById { get; set; }
        public User? ChangedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
