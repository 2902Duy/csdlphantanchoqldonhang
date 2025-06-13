using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webbanhang.Models
{
    public class Kho
    {
        [Key]
        public string MaKho { get; set; }
        public string TenKho { get; set; }

        public string DiaChi { get; set; }
        public string MaKhuVuc { get; set; }
        [ForeignKey("MaKhuVuc")]
        public KhuVuc KhuVuc { get; set; }

        public ICollection<SanPham> SanPhams { get; set; }
    }


}
