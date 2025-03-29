using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RentalHouse.Application.DTOs;
using RentalHouse.Application.Interfaces;
using RentalHouse.Domain.Entities;
using RentalHouse.Domain.Entities.Reports;
using RentalHouse.Infrastructure.Data;
using RentalHouse.SharedLibrary.Responses;

public class ReportRepository : IReportRepository
{
    private readonly IRentalHouseDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public ReportRepository(IRentalHouseDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    // 📌 Phương thức lấy tất cả các báo cáo
    public async Task<IEnumerable<ReportDto>> GetAllReportsAsync()
    {
        return await _context.Reports
            .Include(r => r.Images)
            .Include(r => r.User)
            .Include(r => r.NhaTro)
            .Select(r => new ReportDto
            {
                Id = r.Id,
                UserId = r.User.Id,
                ReportType = r.ReportType,
                Description = r.Description,
                Images = r.Images.Select(img => img.ImageUrl).ToList(), // Chỉ lấy URL ảnh
                Status = r.Status,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt,
                NhaTro = r.NhaTro
            })
            .ToListAsync();
    }


    // 📌 Lấy báo cáo theo ID (kèm danh sách ảnh)
    public async Task<Report> GetReportByIdAsync(int id)
    {
        return await _context.Reports
            .Include(r => r.Images) // Lấy danh sách ảnh
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    // 📌 Xử lý upload nhiều ảnh lên server
    public async Task<List<string>> UploadEvidenceFilesAsync(List<IFormFile> files)
    {
        List<string> evidenceUrls = new List<string>();

        if (files != null && files.Count > 0)
        {
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads/reports");
            Directory.CreateDirectory(uploadsFolder); // Đảm bảo thư mục tồn tại

            foreach (var file in files)
            {
                string fileName = $"{Guid.NewGuid()}_{file.FileName}";
                string filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                string fileUrl = $"/uploads/reports/{fileName}";
                evidenceUrls.Add(fileUrl);
            }
        }

        return evidenceUrls;
    }

    // 📌 Tạo báo cáo kèm danh sách ảnh
    public async Task<Response> CreateReportAsync(CreateReportDto reportDto, List<string> imageUrls)
    {
        // Upload file ảnh trước

        var report = new Report
        {
            UserId = reportDto.UserId,
            NhaTroId = reportDto.NhaTroId,
            ReportType = reportDto.ReportType,
            Description = reportDto.Description,
            Status = ApprovalStatus.Pending,
        };

        var currentReport = _context.Reports.Add(report).Entity;
        await _context.SaveChangesAsync();

        // Nếu có ảnh thì lưu vào bảng ReportImage
        foreach (var imageUrl in imageUrls)
        {
            var reportImage = new ReportImage
            {
                ImageUrl = imageUrl,
                ReportId = currentReport.Id
            };
            _context.ReportImages.Add(reportImage);
        }
        await _context.SaveChangesAsync();

        if (currentReport is not null && currentReport.Id > 0)
        {
            return new Response(true, "Khiếu nại đã được tạo! Cám ơn bạn đã phản h");
        }
        else
        {
            return new Response(false, $"Có lỗi xảy ra khi thêm khiếu nại!");
        }
    }

    // 📌 Cập nhật trạng thái của báo cáo
    public async Task<bool> UpdateReportStatusAsync(int id, UpdateReportDto updateDto)
    {
        var report = await _context.Reports.FindAsync(id);
        if (report == null) return false;

        // Cập nhật trạng thái dựa trên dữ liệu từ UpdateReportDto
        switch (updateDto.Status)
        {
            case 0:
                report.Status = ApprovalStatus.Pending;
                break;
            case 1:
                report.Status = ApprovalStatus.Approved;
                break;
            case 2:
                report.Status = ApprovalStatus.Rejected;
                break;
            default:
                return false; // Trường hợp trạng thái không hợp lệ
        }

        _context.Reports.Update(report);
        await _context.SaveChangesAsync();
        return true;
    }
}
