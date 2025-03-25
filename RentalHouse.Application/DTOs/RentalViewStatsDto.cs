namespace RentalHouse.Application.DTOs
{
    public class RentalViewStatsDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int ViewCount { get; set; }
        public DateTime? PostedDate { get; set; }
        public string Status { get; set; }
    }
}
