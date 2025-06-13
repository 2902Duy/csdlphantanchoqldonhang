using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webbanhang.Models
{
    public class CTDonHang
    {
        [Key, Column(Order = 0)]
        public string MaDonHang { get; set; }

        [Key, Column(Order = 1)]
        public string MaSP { get; set; }

        public int SoLuong { get; set; }

        public decimal DonGia { get; set; }

        [ForeignKey("MaDonHang")]
        public DonHang DonHang { get; set; }

        [ForeignKey("MaSP")]
        public SanPham SanPham { get; set; }
    }

}
