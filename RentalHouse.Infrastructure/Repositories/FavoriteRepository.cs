using Microsoft.EntityFrameworkCore;
using RentalHouse.Application.DTOs;
using RentalHouse.Application.Interfaces;
using RentalHouse.Domain.Entities.Favorites;
using RentalHouse.Infrastructure.Data;
using RentalHouse.SharedLibrary.Interfaces;
using RentalHouse.SharedLibrary.Logs;
using RentalHouse.SharedLibrary.Responses;
using System.Linq.Expressions;

namespace RentalHouse.Infrastructure.Repositories
{
    public class FavoriteRepository : IFavoriteRepository
    {
        private readonly IRentalHouseDbContext _context;
        public FavoriteRepository(IRentalHouseDbContext context)
        {
            _context = context;
        }
        public async Task<FavoriteResponse> CreateAsync(Favorite entity)
        {
            try
            {

                // Kiểm tra xem người dùng có tồn tại hay không
                var userExists = await _context.Users.AnyAsync(u => u.Id == entity.UserId);
                if (!userExists)
                {
                    return new FavoriteResponse(0, false, "Người dùng không tồn tại!");
                }

                var fav = await GetByAsync(f => f.UserId == entity.UserId && f.NhaTroId == entity.NhaTroId);
                if (fav is not null)
                {
                    return new FavoriteResponse(0, false, "Bạn đã lưu thông tin nhà trọ này trước đó!");
                }

                var currentFav = _context.Favorites.Add(entity).Entity;
                await _context.SaveChangesAsync();
                if (currentFav is not null && currentFav.Id > 0)
                {
                    return new FavoriteResponse(currentFav.Id, true, "Đã lưu thông tin nhà trọ!");
                }
                else
                {
                    return new FavoriteResponse(0, false, "Có lỗi xảy ra khi thêm thông tin nhà trọ!");
                }
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new InvalidOperationException("Có lỗi xảy ra khi thêm thông tin nhà trọ!");
            }
        }

        public async Task<Response> DeleteAsync(Favorite entity)
        {
            try
            {
                var userExists = await _context.Users.AnyAsync(u => u.Id == entity.UserId);
                if (!userExists)
                {
                    return new Response(false, "Người dùng không tồn tại!");
                }


                var fav = await GetByAsync(u => u.UserId == entity.UserId && u.NhaTroId == entity.NhaTroId);
                if (fav is null)
                {
                    return new Response(false, $"Không tìm thấy thông tin nhà trọ đã lưu!");
                }
                _context.Entry(fav).State = EntityState.Detached;
                _context.Favorites.Remove(fav);
                await _context.SaveChangesAsync();
                return new Response(true, $"Đã bỏ lưu thông tin nhà trọ: {fav.NhaTro!.Title}!");
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new InvalidOperationException("Có lỗi xảy ra khi bỏ lưu thông tin nhà trọ!");
            }
        }

        public async Task<Favorite> FindByIdAsync(int id)
        {
            try
            {
                var getFav = await _context.Favorites
                    .Include(f => f.NhaTro)
                    .Include(f => f.User)
                    .Where(f => f.Id == id)
                    .FirstOrDefaultAsync()!;

                return getFav is not null ? getFav : null!;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new InvalidOperationException("Có lỗi xảy ra khi tìm kiếm thông tin nhà trọ!");
            }
        }
        public async Task<IEnumerable<Favorite>> GetFavoritesByUserIdAsync(int userId)
        {
            try
            {
                var favs = await _context.Favorites
                    .AsNoTracking()
                    .Include(f => f.NhaTro)
                    .Include(f => f.User)
                    .Where(f => f.UserId == userId)
                    .OrderByDescending(f => f.DateSaved)
                    .ToListAsync();

                return favs is not null ? favs : null!;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new InvalidOperationException("Có lỗi xảy ra khi truy xuất thông tin nhà trọ đã lưu!");
            }
        }

        public async Task<Favorite> GetByAsync(Expression<Func<Favorite, bool>> predicate)
        {
            try
            {
                var fav = await _context.Favorites
                    .Include(f => f.NhaTro)
                    .Include(f => f.User)
                    .Where(predicate)
                    .FirstOrDefaultAsync()!;

                return fav is not null ? fav : null!;

            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);

                throw new InvalidOperationException("Xảy ra lỗi khi truy xuất thông tin nhà trọ!");
            }
        }

        Task<Response> ICrudGenericInterface<Favorite>.CreateAsync(Favorite entity)
        {
            throw new NotImplementedException();
        }
    }
}
