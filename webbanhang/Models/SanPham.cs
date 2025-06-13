using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webbanhang.Models
{
    public class SanPham
    {
        [Key]
        public string MaSP { get; set; }
        public string TenSP { get; set; }

        public string MaLoaiSP { get; set; }
        [ForeignKey("MaLoaiSP")]
        public LoaiSP LoaiSP { get; set; }

        public decimal GiaBan { get; set; }

        public string Mota { get; set; }

        public string HinhAnh { get; set; }

        public string MaKho { get; set; }
        [ForeignKey("MaKho")]
        public Kho Kho { get; set; }

        public ICollection<CTDonHang> CTDonHangs { get; set; }
    }

}
