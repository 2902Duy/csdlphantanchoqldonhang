using System.ComponentModel.DataAnnotations;

namespace webbanhang.Models
{
    public class KhuVuc
    {
        [Key]
        public string MaKhuVuc { get; set; }
        public string TenKhuVuc { get; set; }

        public string MoTa { get; set; }

        public ICollection<Kho> Khos { get; set; }
        public ICollection<KhachHang> KhachHangs { get; set; }
    }

}
