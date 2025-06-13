using Microsoft.EntityFrameworkCore;
using webbanhang.Models; 
public interface IAppDbContext
{
    DbSet<KhachHang> KhachHang { get; }
    DbSet<TaiKhoan> TaiKhoan { get; }
    DbSet<SanPham> SanPham { get; }
    DbSet<DonHang> DonHang { get; }
    DbSet<CTDonHang> CTDonHang { get; }

    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
