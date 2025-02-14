namespace RentalHouse.SharedLibrary.Response
{
    public record Response(bool IsSuccess = false, string Message = null!);
}
