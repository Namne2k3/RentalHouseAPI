namespace RentalHouse.Application.DTOs
{
    public class AppointmentDetailDto
    {
        public int Id { get; set; }
        public string? NhaTroTitle { get; set; }
        public string? UserName { get; set; }
        public string? OwnerName { get; set; }
        public DateTime AppointmentTime { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? Notes { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public IEnumerable<AppointmentHistoryDto>? History { get; set; }
    }
}
