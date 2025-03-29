using RentalHouse.Domain.Entities;
using RentalHouse.Domain.Entities.NhaTros;

namespace RentalHouse.Application.DTOs
{
    public class ReportDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? NhaTroId { get; set; }
        public string? ReportType { get; set; }
        public string? Description { get; set; }
        public List<string> Images { get; set; } = new();
        public ApprovalStatus Status { get; set; }
        public NhaTro? NhaTro { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

}
