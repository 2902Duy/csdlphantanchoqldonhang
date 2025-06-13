using System.ComponentModel.DataAnnotations;

namespace webbanhang.Models
{
    public class LoaiSP
    {
        [Key]
        public string MaLoaiSP { get; set; }
        public string TenLoaiSP { get; set; }
        public ICollection<SanPham> SanPhams { get; set; }
    }

}
