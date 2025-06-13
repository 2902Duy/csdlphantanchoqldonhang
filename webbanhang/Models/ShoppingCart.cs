namespace webbanhang.Models
{
    public class ShoppingCart
    {
        public List<CartItem> Items { get; set; } = new List<CartItem>();

        public decimal TongTien => Items.Sum(i => i.ThanhTien);
    }

}
