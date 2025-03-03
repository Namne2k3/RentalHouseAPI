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
        decimal? price1,
        decimal? price2,
        decimal? area,
        int? bedRoomCount
    );
}
