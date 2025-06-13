using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webbanhang.Models
{
    public class DonHang
    {
        [Key]
        public string MaDonHang { get; set; }

        public string MaKH { get; set; }
        [ForeignKey("MaKH")]
        public KhachHang KhachHang { get; set; }

        public DateTime NgayDat { get; set; }

        public string TrangThai { get; set; }

        public ICollection<CTDonHang> CTDonHangs { get; set; }
    }

}
