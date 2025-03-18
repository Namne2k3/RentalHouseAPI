namespace RentalHouse.Application.DTOs
{
    public record AppointmentDTO(
        int id,
        int userId,
        string fullName,
        string phoneNumber,
        string email,
        string address,
        string title,
        string status,
        DateTime createdAt,
        DateTime? updatedAt
    );
}
