//using Microsoft.AspNetCore.Mvc;
//using webbanhang.Data;
//using webbanhang.Models;

//namespace webbanhang.Controllers
//{
//    public class GioHangController : Controller
//    {
//        private readonly ApplicationDbContext _context;
//        private const string CartKey = "GioHang";

//        public GioHangController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        private ShoppingCart GetCart()
//        {
//            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>(CartKey);
//            if (cart == null)
//            {
//                cart = new ShoppingCart();
//                HttpContext.Session.SetObjectAsJson(CartKey, cart);
//            }
//            return cart;
//        }

//        public IActionResult Index()
//        {
//            var cart = GetCart();
//            return View(cart);
//        }

//        public IActionResult ThemVaoGio(string maSP)
//        {
//            var sp = _context.SanPham.Find(maSP);
//            if (sp != null)
//            {
//                var cart = GetCart();
//                var item = cart.Items.FirstOrDefault(x => x.MaSP == maSP);
//                if (item == null)
//                {
//                    cart.Items.Add(new CartItem
//                    {
//                        MaSP = sp.MaSP,
//                        TenSP = sp.TenSP,
//                        GiaBan = sp.GiaBan,
//                        SoLuong = 1,
//                        HinhAnh = sp.HinhAnh
//                    });
//                }
//                else
//                {
//                    item.SoLuong++;
//                }
//                HttpContext.Session.SetObjectAsJson(CartKey, cart);
//            }
//            return RedirectToAction("Index", "GioHang");
//        }

//        public IActionResult XoaKhoiGio(string maSP)
//        {
//            var cart = GetCart();
//            cart.Items.RemoveAll(x => x.MaSP == maSP);
//            HttpContext.Session.SetObjectAsJson(CartKey, cart);
//            return RedirectToAction("Index");
//        }

//        public IActionResult ThanhToan()
//        {
//            var cart = GetCart();
//            if (!cart.Items.Any()) return RedirectToAction("Index");

//            // Tạm thời giả lập khách hàng
//            var kh = _context.KhachHang.FirstOrDefault();
//            if (kh == null) return Content("Không tìm thấy khách hàng");

//            var maDH = "DH" + DateTime.Now.Ticks;
//            var donHang = new DonHang
//            {
//                MaDonHang = maDH,
//                MaKH = kh.MaKH,
//                NgayDat = DateTime.Now,
//                TrangThai = "Đang xử lý"
//            };

//            _context.DonHang.Add(donHang);

//            foreach (var item in cart.Items)
//            {
//                var ct = new CTDonHang
//                {
//                    MaDonHang = maDH,
//                    MaSP = item.MaSP,
//                    SoLuong = item.SoLuong,
//                    DonGia = item.GiaBan
//                };
//                _context.CTDonHang.Add(ct);
//            }

//            _context.SaveChanges();
//            HttpContext.Session.Remove(CartKey); // clear cart
//            return View("ThongBaoDatHang");
//        }
//    }

//}
