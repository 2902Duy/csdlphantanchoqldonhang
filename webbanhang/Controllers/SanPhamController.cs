using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text.Json;
using webbanhang.Data;
using webbanhang.Models;

namespace webbanhang.Controllers
{
    public class SanPhamController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ApplicationDbContextNam _contextNam;
        private readonly ApplicationDbContextBac _contextBac;
        private readonly ApplicationDbContextTrung _contextTrung;

        public SanPhamController(ApplicationDbContext context, ApplicationDbContextNam contextNam, ApplicationDbContextBac contextBac, ApplicationDbContextTrung contextTrung)
        {
            _context = context;
            _contextNam = contextNam;
            _contextBac = contextBac;
            _contextTrung = contextTrung;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            var query = _context.SanPham.Include(sp => sp.LoaiSP).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(sp => sp.TenSP.Contains(searchString));
            }

            var sanPhams = await query.ToListAsync();
            return View(sanPhams);
        }

        [HttpGet]
        public IActionResult AddToCart(string maSP)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var userName = User.Identity.Name;
            var cartKey = $"cart_{userName}";
            var cartJson = HttpContext.Session.GetString(cartKey);

            var cart = string.IsNullOrEmpty(cartJson)
                ? new Dictionary<string, int>()
                : JsonSerializer.Deserialize<Dictionary<string, int>>(cartJson);

            if (cart.ContainsKey(maSP))
                cart[maSP]++;
            else
                cart[maSP] = 1;

            HttpContext.Session.SetString(cartKey, JsonSerializer.Serialize(cart));

            return RedirectToAction("Index");
        }


        [Authorize]
        public IActionResult Cart()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var userName = User.Identity.Name;
            var cartKey = $"cart_{userName}";
            var cartJson = HttpContext.Session.GetString(cartKey);

            var cart = string.IsNullOrEmpty(cartJson)
                ? new Dictionary<string, int>()
                : JsonSerializer.Deserialize<Dictionary<string, int>>(cartJson);

            var maSPList = cart.Keys.ToList();

            var products = _context.SanPham.Where(p => maSPList.Contains(p.MaSP)).ToList();

            // Truyền thêm số lượng vào ViewModel hoặc ViewBag
            ViewBag.CartQuantities = cart;

            return View(products);
        }


        public IActionResult RemoveFromCart(string maSP)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var userName = User.Identity.Name;
            var cartKey = $"cart_{userName}";
            var cartJson = HttpContext.Session.GetString(cartKey);

            var cart = string.IsNullOrEmpty(cartJson)
                ? new Dictionary<string, int>()
                : JsonSerializer.Deserialize<Dictionary<string, int>>(cartJson);

            if (cart.ContainsKey(maSP))
            {
                cart.Remove(maSP);
                HttpContext.Session.SetString(cartKey, JsonSerializer.Serialize(cart));
            }

            return RedirectToAction("Cart");
        }


        [HttpPost]
        public IActionResult UpdateQuantityAjax(string maSP, int quantity)
        {
            var userName = User.Identity?.Name;
            if (string.IsNullOrEmpty(userName))
                return Json(new { success = false, message = "Chưa đăng nhập" });

            var cartKey = $"cart_{userName}";
            var cartJson = HttpContext.Session.GetString(cartKey);
            var cart = string.IsNullOrEmpty(cartJson)
                ? new Dictionary<string, int>()
                : JsonSerializer.Deserialize<Dictionary<string, int>>(cartJson);

            if (cart.ContainsKey(maSP))
                cart[maSP] = quantity;

            HttpContext.Session.SetString(cartKey, JsonSerializer.Serialize(cart));
            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Checkout(List<string> selectedItems)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var userName = User.Identity.Name;

            // Lấy mã KH từ claims (nếu bạn có lưu claim "MaKH")
            var maKH = User.FindFirst("MaKH")?.Value;
            var maKhuVuc = User.FindFirst("MaKhuVuc")?.Value;

            if (string.IsNullOrEmpty(maKH) || string.IsNullOrEmpty(maKhuVuc))
                return Unauthorized();

            IAppDbContext db = maKhuVuc == "MB" ? (IAppDbContext)_contextBac 
                :maKhuVuc == "MT" ? (IAppDbContext)_contextTrung
                : _contextNam;

            var khachHang = db.KhachHang
                .Include(k => k.TaiKhoan)
                .FirstOrDefault(kh => kh.MaKH == maKH);

            if (khachHang == null)
                return Unauthorized();

            var cartKey = $"cart_{userName}";
            var cartJson = HttpContext.Session.GetString(cartKey);
            if (string.IsNullOrEmpty(cartJson)) return RedirectToAction("Cart");

            var cart = JsonSerializer.Deserialize<Dictionary<string, int>>(cartJson);
            var products = db.SanPham
                .Where(p => selectedItems.Contains(p.MaSP))
                .ToList();

            var prefixDH = "DH" + khachHang.MaKhuVuc;
            var lastOrder = db.DonHang
                .Where(dh => dh.MaDonHang.StartsWith(prefixDH))
                .OrderByDescending(dh => dh.MaDonHang)
                .Select(dh => dh.MaDonHang)
                .FirstOrDefault();

            int nextOrderNumber = 1;
            if (!string.IsNullOrEmpty(lastOrder) && lastOrder.Length > 4)
            {
                if (int.TryParse(lastOrder.Substring(4), out int number))
                {
                    nextOrderNumber = number + 1;
                }
            }

            string maDonHang = prefixDH + nextOrderNumber.ToString("D2");

            var donHang = new DonHang
            {
                MaDonHang = maDonHang,
                MaKH = khachHang.MaKH,
                NgayDat = DateTime.Now,
                TrangThai = "Chờ xác nhận",
                CTDonHangs = new List<CTDonHang>()
            };

            foreach (var sp in products)
            {
                if (!cart.ContainsKey(sp.MaSP)) continue;

                int soLuong = cart[sp.MaSP];

                donHang.CTDonHangs.Add(new CTDonHang
                {
                    MaDonHang = maDonHang,
                    MaSP = sp.MaSP,
                    SoLuong = soLuong,
                    DonGia = sp.GiaBan
                });

                cart.Remove(sp.MaSP);
            }

            db.DonHang.Add(donHang);
            db.SaveChanges();

            HttpContext.Session.SetString(cartKey, JsonSerializer.Serialize(cart));

            TempData["Success"] = "Đặt hàng thành công!";
            return View("CheckoutSuccess", donHang);
        }


    }

}
