namespace webbanhang.Models
{
    public class CartItem
    {
        public string MaSP { get; set; }

        public string TenSP { get; set; }

        public string HinhAnh { get; set; }

        public int SoLuong { get; set; }

        public decimal GiaBan { get; set; }

        public decimal ThanhTien => SoLuong * GiaBan;
    }

}
