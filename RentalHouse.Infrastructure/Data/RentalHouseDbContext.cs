﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using RentalHouse.Domain.Entities.Addresses.Districts;
using RentalHouse.Domain.Entities.Addresses.Provinces;
using RentalHouse.Domain.Entities.Addresses.Wards;
using RentalHouse.Domain.Entities.Appointments;
using RentalHouse.Domain.Entities.Auth;
using RentalHouse.Domain.Entities.Favorites;
using RentalHouse.Domain.Entities.NhaTros;

namespace RentalHouse.Infrastructure.Data
{
    public class RentalHouseDbContext : DbContext, IRentalHouseDbContext
    {
        public RentalHouseDbContext(DbContextOptions<RentalHouseDbContext> options) : base(options) { }
        public DbSet<NhaTro> NhaTros { get; set; }
        public DbSet<NhaTroImage> NhaTroImages { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Ward> Wards { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NhaTro>()
                .HasOne(n => n.User)
                .WithMany(u => u.Nhatros)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Ngăn chặn xóa user làm mất nhà trọ

            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.User)
                .WithMany(u => u.Favorites)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Chỉ xóa favorite khi user bị xóa

            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.NhaTro)
                .WithMany()
                .HasForeignKey(f => f.NhaTroId)
                .OnDelete(DeleteBehavior.Cascade); // Chỉ xóa favorite khi nhà trọ bị xóa

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.User)
                .WithMany(u => u.Appointments)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.NoAction); // 👈 Fix lỗi bằng cách ngăn cascade delete
        }


        EntityEntry IRentalHouseDbContext.Entry(object entity)
        {
            return base.Entry(entity);
        }
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
