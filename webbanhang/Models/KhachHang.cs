using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webbanhang.Models
{
    public class KhachHang
    {
        [Key]
        public string MaKH { get; set; }

        public string HoTen { get; set; }

        public string DiaChi { get; set; }

        public string DienThoai { get; set; }

        public string Email { get; set; }

        public string MaKhuVuc { get; set; }

        public KhuVuc KhuVuc { get; set; }

        public ICollection<DonHang> DonHangs { get; set; }

        public TaiKhoan TaiKhoan { get; set; }
    }

}
