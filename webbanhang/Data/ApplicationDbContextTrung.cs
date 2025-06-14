using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using webbanhang.Models;

namespace webbanhang.Data
{
    public class ApplicationDbContextTrung : DbContext, IAppDbContext
    {
        public ApplicationDbContextTrung(DbContextOptions<ApplicationDbContextTrung> options)
            : base(options) { }

        public DbSet<SanPham> SanPham { get; set; }
        public DbSet<DonHang> DonHang { get; set; }
        public DbSet<CTDonHang> CTDonHang { get; set; }
        public DbSet<KhachHang> KhachHang { get; set; }
        public DbSet<TaiKhoan> TaiKhoan { get; set; }
        public DbSet<LoaiSP> LoaiSP { get; set; }
        public DbSet<Kho> Kho { get; set; }
        public DbSet<KhuVuc> KhuVuc { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SanPham>()
    .HasOne(sp => sp.LoaiSP)
    .WithMany(ls => ls.SanPhams)
    .HasForeignKey(sp => sp.MaLoaiSP);

            modelBuilder.Entity<SanPham>()
                .HasOne(sp => sp.Kho)
                .WithMany(k => k.SanPhams)
                .HasForeignKey(sp => sp.MaKho);

            modelBuilder.Entity<DonHang>()
                .HasOne(dh => dh.KhachHang)
                .WithMany(kh => kh.DonHangs)
                .HasForeignKey(dh => dh.MaKH);

            modelBuilder.Entity<KhachHang>()
                .HasOne(kh => kh.KhuVuc)
                .WithMany(kv => kv.KhachHangs)
                .HasForeignKey(kh => kh.MaKhuVuc);

            modelBuilder.Entity<TaiKhoan>()
                .HasOne(tk => tk.KhachHang)
                .WithOne(kh => kh.TaiKhoan)
                .HasForeignKey<TaiKhoan>(tk => tk.MaKH);

            modelBuilder.Entity<CTDonHang>()
                .HasKey(ct => new { ct.MaDonHang, ct.MaSP });

            modelBuilder.Entity<CTDonHang>()
                .HasOne(ct => ct.DonHang)
                .WithMany(dh => dh.CTDonHangs)
                .HasForeignKey(ct => ct.MaDonHang);

            modelBuilder.Entity<CTDonHang>()
                .HasOne(ct => ct.SanPham)
                .WithMany(sp => sp.CTDonHangs)
                .HasForeignKey(ct => ct.MaSP);


        }

    }

}
