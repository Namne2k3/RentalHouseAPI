namespace RentalHouse.Application.DTOs
{
    public record FilterNhaTroDTO(
        int page,
        int pageSize,
        string? city,
        string? district,
        string? commune,
        string? street,
        string? address,
        decimal? price,
        decimal? area,
        int? bedRoomCount
    );
}
