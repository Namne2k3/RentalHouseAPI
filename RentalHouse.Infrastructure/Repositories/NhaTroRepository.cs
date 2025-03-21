﻿using Microsoft.EntityFrameworkCore;
using RentalHouse.Application.DTOs;
using RentalHouse.Application.DTOs.Conversions;
using RentalHouse.Application.Interfaces;
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

        public async Task<Response> CreateAsync(NhaTro entity, List<string> images)
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

                foreach (var imageUrl in images)
                {
                    var nhaTroImage = new NhaTroImage
                    {
                        ImageUrl = imageUrl,
                        NhaTroID = currentNhaTro.Id
                    };
                    _context.NhaTroImages.Add(nhaTroImage);
                }
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

        public Task<Response> CreateAsync(NhaTro entity)
        {
            throw new NotImplementedException();
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
                var product = await _context.NhaTros
                    .Include(n => n.Images)
                    .Include(n => n.User)
                    .FirstOrDefaultAsync(n => n.Id == id);
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
            decimal? price1 = null,
            decimal? price2 = null,
            decimal? area1 = null,
            decimal? area2 = null,
            int? bedRoomCount = null,
            int? userId = null
        )
        {
            try
            {
                // Query server-side: chỉ những điều kiện mà EF có thể dịch sang SQL
                var query = _context.NhaTros
                    .AsNoTracking()
                    .Include(n => n.Images)
                    .Include(u => u.User)
                    .Where(n =>
                        (address != null &&
                            (n.Address != null && n.Address.ToLower().Contains(address.ToLower())) &&
                            (userId == null || (n.UserId == userId))
                        )
                        ||
                        (address == null &&
                            (userId == null || (n.UserId == userId)) &&
                            (city == null || (n.Address != null && n.Address.ToLower().Contains(city.ToLower()))) &&
                            (district == null || (n.Address != null && n.Address.ToLower().Contains(district.ToLower()))) &&
                            (commune == null || (n.Address != null && n.Address.ToLower().Contains(commune.ToLower()))) &&
                            (street == null || (n.Address != null && n.Address.ToLower().Contains(street.ToLower())))
                        )
                    );

                // Apply additional filters directly in the query
                if (price1 != null && price2 != null)
                {
                    query = query.Where(n => n.Price >= price1 && n.Price <= price2);
                }

                if (area1 != null && area2 != null)
                {
                    query = query.Where(n => n.Area != null && n.Area >= area1.Value && n.Area <= area2.Value);
                }

                if (bedRoomCount != null)
                {
                    query = query.Where(n => n.BedRoomCount != null && n.BedRoomCount == bedRoomCount.Value);
                }

                // Tính tổng số mục dựa trên dữ liệu đã được filter
                var totalItems = await query.CountAsync();

                // Sắp xếp và phân trang trên tập dữ liệu đã filter
                var pagedData = await query
                    .OrderByDescending(n => n.Id)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var (_, list) = NhaTroConversion.FromEntity(null, pagedData);

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

        public async Task<IEnumerable<NhaTroDTO>> GetRelateNhaTrosAsync(int nhaTroId, int count)
        {
            try
            {
                var currentNhaTro = await _context.NhaTros.AsNoTracking().FirstOrDefaultAsync(n => n.Id == nhaTroId);

                if (currentNhaTro == null)
                {
                    return Enumerable.Empty<NhaTroDTO>();
                }

                var relatedNhaTros = await _context.NhaTros.AsNoTracking()
                    .Include(n => n.User)
                    .Include(n => n.Images)
                    .Where(n => n.Id != nhaTroId && n.Price >= currentNhaTro.Price * 0.8m && n.Price <= currentNhaTro.Price * 1.2m)
                    .Take(count)
                    .ToListAsync();

                var (_, listNhaTroDTOS) = NhaTroConversion.FromEntity(null, relatedNhaTros);

                return listNhaTroDTOS != null ? listNhaTroDTOS : Enumerable.Empty<NhaTroDTO>();


            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new InvalidOperationException("Xảy ra lỗi khi tìm dữ liệu nhà trọ liên quan!");
            }
        }

        public async Task<Response> UpdateAsync(NhaTro entity)
        {
            try
            {
                var existingNhaTro = await _context.NhaTros
                    .Include(n => n.Images)
                    .FirstOrDefaultAsync(n => n.Id == entity.Id);

                if (existingNhaTro == null)
                {
                    return new Response(false, "Không tìm thấy dữ liệu nhà trọ!");
                }

                // Cập nhật các thuộc tính của nhà trọ
                existingNhaTro.Title = entity.Title;
                existingNhaTro.Address = entity.Address;
                existingNhaTro.Description = entity.Description;
                existingNhaTro.DescriptionHtml = entity.DescriptionHtml;
                existingNhaTro.Url = entity.Url;
                existingNhaTro.Price = entity.Price;
                existingNhaTro.PriceExt = entity.PriceExt;
                existingNhaTro.Area = entity.Area;
                existingNhaTro.BedRoom = entity.BedRoom;
                existingNhaTro.PostedDate = entity.PostedDate;
                existingNhaTro.ExpiredDate = entity.ExpiredDate;
                existingNhaTro.Type = entity.Type;
                existingNhaTro.Code = entity.Code;
                existingNhaTro.BedRoomCount = entity.BedRoomCount;
                existingNhaTro.BathRoom = entity.BathRoom;
                existingNhaTro.Furniture = entity.Furniture;
                existingNhaTro.Latitude = entity.Latitude;
                existingNhaTro.Longitude = entity.Longitude;
                existingNhaTro.PriceBil = entity.PriceBil;
                existingNhaTro.PriceMil = entity.PriceMil;
                existingNhaTro.PriceVnd = entity.PriceVnd;
                existingNhaTro.AreaM2 = entity.AreaM2;
                existingNhaTro.PricePerM2 = entity.PricePerM2;

                // Cập nhật danh sách hình ảnh
                if (entity.Images != null && entity.Images.Any())
                {
                    // Xóa các hình ảnh cũ
                    _context.NhaTroImages.RemoveRange(existingNhaTro.Images);

                    // Thêm các hình ảnh mới
                    foreach (var image in entity.Images)
                    {
                        var nhaTroImage = new NhaTroImage
                        {
                            ImageUrl = image.ImageUrl,
                            NhaTroID = existingNhaTro.Id
                        };
                        _context.NhaTroImages.Add(nhaTroImage);
                    }
                }

                await _context.SaveChangesAsync();

                return new Response(true, "Dữ liệu nhà trọ đã được cập nhật thành công");
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new InvalidOperationException("Xảy ra lỗi khi tìm dữ liệu nhà trọ liên quan!");
            }
        }
    }
}
