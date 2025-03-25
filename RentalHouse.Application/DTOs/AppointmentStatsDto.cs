namespace RentalHouse.Application.DTOs
{
    public class AppointmentStatsDto
    {
        public int TotalAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public int PendingAppointments { get; set; }
        public int CancelledAppointments { get; set; }
    }
}
