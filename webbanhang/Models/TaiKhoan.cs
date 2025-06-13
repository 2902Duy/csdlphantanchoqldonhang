using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webbanhang.Models
{
    public class TaiKhoan
    {
        [Key]
        public string MaTK { get; set; }

        public string TenTaiKhoan { get; set; }

        public string MatKhau { get; set; }

        public string MaKH { get; set; }
        [ForeignKey("MaKH")]
        public KhachHang KhachHang { get; set; }
    }

}
