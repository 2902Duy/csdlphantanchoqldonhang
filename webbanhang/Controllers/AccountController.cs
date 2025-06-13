using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using webbanhang.Data;
using webbanhang.Models;
using webbanhang.Models.ViewModels;

namespace webbanhang.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            ViewBag.KhuVuc = new SelectList(_context.KhuVuc.ToList(), "MaKhuVuc", "TenKhuVuc");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                string prefixKH = "KH"+model.MaKhuVuc;
                var lastKh = _context.KhachHang
                    .Where(k => k.MaKH.StartsWith(prefixKH))
                    .OrderByDescending(k => k.MaKH)
                    .Select(k => k.MaKH)
                    .FirstOrDefault();

                int nextNumber = 1;
                
                if (!string.IsNullOrEmpty(lastKh) && lastKh.Length > 4)
                {
                    if (int.TryParse(lastKh.Substring(4), out int number))
                    {
                        nextNumber = number + 1;
                    }
                }
                string newMaKH = prefixKH + nextNumber.ToString("D2");

                var kh = new KhachHang
                {
                    MaKH = newMaKH,
                    HoTen = model.HoTen,
                    DiaChi = model.DiaChi,
                    DienThoai = model.DienThoai,
                    Email = model.Email,
                    MaKhuVuc = model.MaKhuVuc
                };
                string prefixTK = "TK"+model.MaKhuVuc;

                var lastTK = _context.TaiKhoan
                    .Where(t => t.MaTK.StartsWith(prefixTK))
                    .OrderByDescending(t => t.MaTK)
                    .Select(t => t.MaTK)
                    .FirstOrDefault();

                int nextTK = 1;
                //string prefixTK = lastTK.Substring(0,4);
                if (!string.IsNullOrEmpty(lastTK) && lastTK.Length > 4)
                {
                    if (int.TryParse(lastTK.Substring(4), out int number))
                    {
                        nextTK = number + 1;
                    }
                }
                //string newMaTK;
                //while (true)
                //{
                //    newMaTK = prefixTK + nextNumber.ToString("D2");

                //    bool exists = _context.TaiKhoan.Any(tk => tk.MaTK == newMaKH);
                //    if (!exists) break;

                //    nextNumber++;
                //}
                string newMaTK = prefixTK + nextTK.ToString("D2");

                var tk = new TaiKhoan
                {
                    MaTK = newMaTK,
                    TenTaiKhoan = model.TenTaiKhoan,
                    MatKhau = model.MatKhau, 
                    MaKH = newMaKH   
                };

                _context.KhachHang.Add(kh);
                _context.TaiKhoan.Add(tk);
                await _context.SaveChangesAsync();

                var khuVuc = await _context.KhuVuc.FindAsync(model.MaKhuVuc);

                return RedirectToAction("Login");
            }

            ViewBag.KhuVuc = new SelectList(_context.KhuVuc.ToList(), "MaKhuVuc", "TenKhuVuc");
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string tenTaiKhoan, string matKhau)
        {
            var tk = _context.TaiKhoan.Include(t => t.KhachHang)
                     .FirstOrDefault(t => t.TenTaiKhoan == tenTaiKhoan && t.MatKhau == matKhau);

            if (tk != null)
            {
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, tk.TenTaiKhoan),
            new Claim("HoTen", tk.KhachHang?.HoTen ?? "")
        };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Sai tên đăng nhập hoặc mật khẩu.";
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
