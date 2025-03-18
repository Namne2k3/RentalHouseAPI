using RentalHouse.Domain.Entities.Appointments;

namespace RentalHouse.Application.DTOs.Conversions
{
    public static class AppointmentConversion
    {
        public static AppointmentDTO ToAppointmentDTO(this Appointment appointment)
        {
            return new AppointmentDTO(
                id: appointment.Id,
                userId: appointment.UserId,
                fullName: appointment.User.FullName,
                phoneNumber: appointment.User.PhoneNumber!,
                email: appointment.User.Email,
                address: appointment.NhaTro.Address,
                title: appointment.NhaTro.Title,
                status: appointment.Status,
                createdAt: appointment.CreatedAt,
                updatedAt: appointment.UpdatedAt
            );
        }
    }
}
