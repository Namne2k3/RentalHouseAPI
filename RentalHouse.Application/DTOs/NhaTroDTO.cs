namespace RentalHouse.Application.DTOs
{
    public record NhaTroDTO(
        int Id,
        string Title,
        string Address,
        string? Description,
        string? DescriptionHtml,
        string? Url,
        string? Price,
        string? PriceExt,
        string? Area,
        string? BedRoom,
        DateTime? PostedDate,
        DateTime? ExpiredDate,
        string? Type,
        string? Code,
        string? BedRoomCount,
        string? Furniture,
        float? Latitude,
        float? Longitude,
        float? PriceBil,
        float? PriceMil,
        float? PriceVnd,
        float? AreaM2,
        float? PricePerM2,
        List<string> ImageUrls,
        int UserId,
        string fullName,
        string phoneNumber,
        string email
    );

}
