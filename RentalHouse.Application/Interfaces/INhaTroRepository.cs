using RentalHouse.Application.DTOs;
using RentalHouse.Domain.Entities.NhaTros;
using RentalHouse.SharedLibrary.Interfaces;

namespace RentalHouse.Application.Interfaces
{
    public interface INhaTroRepository : ICrudGenericInterface<NhaTro>, ICanGetAllWithoutParam<NhaTro>, ICanUpdate<NhaTro>
    {
        Task<PagedResultDTO<NhaTroDTO>> GetAllAsyncWithFilters(int page, int pageSize, string? city, string? district, string? commune, string? street, string? address, decimal? price,
                decimal? area,
                int? bedRoomCount);
    }
}
