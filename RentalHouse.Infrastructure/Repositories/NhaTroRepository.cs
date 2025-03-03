using Microsoft.EntityFrameworkCore;
using RentalHouse.Application.DTOs;
using RentalHouse.Application.DTOs.Conversions;
using RentalHouse.Application.Interfaces;
using RentalHouse.Application.Utils;
using RentalHouse.Domain.Entities.NhaTros;
using RentalHouse.Infrastructure.Data;
using RentalHouse.SharedLibrary.Logs;
using RentalHouse.SharedLibrary.Responses;
using System.Linq.Expressions;

namespace RentalHouse.Infrastructure.Repositories
{
    public class NhaTroRepository : INhaTroRepository
    {
        private readonly IRentalHouseDbContext _context;
        public NhaTroRepository(IRentalHouseDbContext context)
        {
            _context = context;
        }

        public async Task<Response> CreateAsync(NhaTro entity)
        {
            try
            {
                var nhatro = await GetByAsync(n => n.Address.Equals(entity.Address));
                if (nhatro is not null)
                {
                    return new Response(false, "Dữ liệu nhà trọ này đã tồn tại!");
                }

                var currentNhaTro = _context.NhaTros.Add(entity).Entity;
                await _context.SaveChangesAsync();

                if (currentNhaTro is not null && currentNhaTro.Id > 0)
                {
                    return new Response(true, "Dữ liệu nhà trọ đã được thêm vào!");
                }
                else
                {
                    return new Response(false, $"Có lỗi xảy ra khi thêm {entity.Title}!");
                }
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);

                return new Response(false, "Có lỗi xảy ra khi thêm dữ liệu!");
            }
        }

        public async Task<Response> DeleteAsync(NhaTro entity)
        {
            try
            {
                var nhatro = await FindByIdAsync(entity.Id);
                if (nhatro is null)
                {
                    return new Response(false, $"Không tìm thấy dữ liệu: {entity.Title}!");
                }
                _context.Entry(nhatro).State = EntityState.Detached;
                _context.NhaTros.Remove(entity);
                await _context.SaveChangesAsync();
                return new Response(true, $"{entity.Title} đã xóa thành công!");
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                return new Response(false, "Có lỗi xảy ra khi xóa dữ liệu!");
            }
        }

        public async Task<NhaTro> FindByIdAsync(int id)
        {
            try
            {
                var product = await _context.NhaTros.FindAsync(id);
                return product is not null ? product : null!;

            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);

                throw new Exception("Có lỗi xảy ra khi tìm kiếm dữ liệu!");
            }
        }

        public async Task<IEnumerable<NhaTro>> GetAllAsync()
        {
            try
            {
                var nhatros = await _context.NhaTros
                    .AsNoTracking()
                    .Take(20)
                    .Include(n => n.Images)
                    .ToListAsync();
                return nhatros is not null ? nhatros : null!;

            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);

                throw new InvalidOperationException("Xảy ra lỗi khi truy xuất dữ liệu!");
            }
        }
        public async Task<PagedResultDTO<NhaTroDTO>> GetAllAsyncWithFilters(
            int page = 1,
            int pageSize = 20,
            string? city = null,
            string? district = null,
            string? commune = null,
            string? street = null,
            string? address = null,
            decimal? price = null,
            decimal? area = null,
            int? bedRoomCount = null
        )
        {
            try
            {
                var query = _context.NhaTros
                    .AsNoTracking()
                    .Include(n => n.Images)
                    .Include(u => u.User)
                    .Where(n =>
                        (address != null &&
                            (n.Address != null && n.Address.ToLower().Contains(address.ToLower())) &&
                            (area == null || (n.Area != null && AreaParserUtil.ParseArea(n.Area) <= area.Value)) &&
                            (price == null || (n.Price != null && PriceParserUtil.ParsePrice(n.Price) <= price.Value)) &&
                            (bedRoomCount == null || (n.BedRoomCount != null && RoomParserUtil.ParseRoomCount(n.BedRoomCount) == bedRoomCount.Value))
                        )
                        ||
                        (address == null &&
                            (city == null || (n.Address != null && n.Address.ToLower().Contains(city.ToLower()))) &&
                            (district == null || (n.Address != null && n.Address.ToLower().Contains(district.ToLower()))) &&
                            (commune == null || (n.Address != null && n.Address.ToLower().Contains(commune.ToLower()))) &&
                            (street == null || (n.Address != null && n.Address.ToLower().Contains(street.ToLower()))) &&
                            (area == null || (n.Area != null && AreaParserUtil.ParseArea(n.Area) <= area.Value)) &&
                            (price == null || (n.Price != null && PriceParserUtil.ParsePrice(n.Price) <= price.Value)) &&
                            (bedRoomCount == null || (n.BedRoomCount != null && RoomParserUtil.ParseRoomCount(n.BedRoomCount) == bedRoomCount.Value))
                        )
                    );

                var totalItems = await query.CountAsync();

                var nhatros = await query
                    .OrderByDescending(n => n.Id)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var (_, list) = NhaTroConversion.FromEntity(null, nhatros);

                return new PagedResultDTO<NhaTroDTO>(
                    TotalItems: totalItems,
                    TotalPages: (int)Math.Ceiling((double)totalItems / pageSize),
                    Data: list ?? new List<NhaTroDTO>()
                );
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new InvalidOperationException("Xảy ra lỗi khi truy xuất dữ liệu!", ex);
            }
        }


        public async Task<NhaTro> GetByAsync(Expression<Func<NhaTro, bool>> predicate)
        {
            try
            {

                var nhatro = await _context.NhaTros.Where(predicate).FirstOrDefaultAsync()!;
                return nhatro is not null ? nhatro : null!;

            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);

                throw new InvalidOperationException("Xảy ra lỗi khi truy xuất dữ liệu!");
            }
        }

        public async Task<Response> UpdateAsync(NhaTro entity)
        {
            throw new NotImplementedException();
        }
    }
}
