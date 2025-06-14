using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using webbanhang.Data;
using webbanhang.Models;
using webbanhang.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
namespace webbanhang.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ApplicationDbContextNam _contextNam;
        private readonly ApplicationDbContextBac _contextBac;
        private readonly ApplicationDbContextTrung _contextTrung;

        public AccountController(ApplicationDbContext context, ApplicationDbContextNam contextNam, ApplicationDbContextBac contextBac, ApplicationDbContextTrung contextTrung)
        {
            _context = context;
            _contextNam = contextNam;
            _contextBac = contextBac;
            _contextTrung = contextTrung;
        }

        private string GenerateNextCode<T>(DbSet<T> dbSet, string columnName, string prefix) where T : class
        {
            var lastCode = dbSet
                .AsQueryable()
                .Where(e => EF.Property<string>(e, columnName).StartsWith(prefix))
                .OrderByDescending(e => EF.Property<string>(e, columnName))
                .Select(e => EF.Property<string>(e, columnName))
                .FirstOrDefault();

            int nextNumber = 1;
            if (!string.IsNullOrEmpty(lastCode) && lastCode.Length > prefix.Length)
            {
                if (int.TryParse(lastCode.Substring(prefix.Length), out int number))
                {
                    nextNumber = number + 1;
                }
            }

            return prefix + nextNumber.ToString("D2");
        }


        [HttpGet]
        public IActionResult Register()
        {
            ViewBag.KhuVuc = new SelectList(_context.tinhTP.ToList(), "MaKhuVuc", "TenTinhTP");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {

                var existingAccount = _contextBac.TaiKhoan
                            .FirstOrDefault(t => t.TenTaiKhoan == model.TenTaiKhoan)
                         ?? _contextNam.TaiKhoan
                            .FirstOrDefault(t => t.TenTaiKhoan == model.TenTaiKhoan);

                if (existingAccount != null)
                {
                    ModelState.AddModelError("TenTaiKhoan", "Tên tài khoản đã tồn tại.");

                    ViewBag.KhuVuc = new SelectList(_context.tinhTP.ToList(), "MaKhuVuc", "TenTinhTP");
                    return View(model);
                }
                string prefixKH = "KH"+model.MaKhuVuc;

                var dbkh = model.MaKhuVuc == "MB" ? _contextBac.KhachHang
                         : model.MaKhuVuc == "MT" ? _contextTrung.KhachHang
                         : _contextNam.KhachHang;
                string newMaKH = GenerateNextCode(dbkh, "MaKH", prefixKH);

                var kh = new KhachHang
                {
                    MaKH = newMaKH,
                    HoTen = model.HoTen,
                    DiaChi = model.DiaChi,
                    DienThoai = model.DienThoai,
                    Email = model.Email,
                    MaKhuVuc = model.MaKhuVuc
                };
                string prefixTK = "TK" + model.MaKhuVuc;
                var dbtk = model.MaKhuVuc == "MB" ? _contextBac.TaiKhoan 
                    : model.MaKhuVuc == "MT" ? _contextTrung.TaiKhoan
                    : _contextNam.TaiKhoan;
                string newMaTK = GenerateNextCode(dbtk, "MaTK", prefixTK);

                var hasher = new PasswordHasher<TaiKhoan>();
                string hashedPassword = hasher.HashPassword(null, model.MatKhau);

                var tk = new TaiKhoan
                {
                    MaTK = newMaTK,
                    TenTaiKhoan = model.TenTaiKhoan,
                    MatKhau = hashedPassword, 
                    MaKH = newMaKH   
                };
                if(model.MaKhuVuc=="MB")
                {
                    _contextBac.KhachHang.Add(kh);
                    _contextBac.TaiKhoan.Add(tk);
                    await _contextBac.SaveChangesAsync();
                }
                else if(model.MaKhuVuc=="MT")
                {
                    _contextTrung.KhachHang.Add(kh);
                    _contextTrung.TaiKhoan.Add(tk);
                    await _contextTrung.SaveChangesAsync();
                }
                else
                {
                    _contextNam.KhachHang.Add(kh);
                    _contextNam.TaiKhoan.Add(tk);
                    await _contextNam.SaveChangesAsync();
                }
                
                    
                

                var khuVuc = await _context.KhuVuc.FindAsync(model.MaKhuVuc);

                return RedirectToAction("Login");
            }

            ViewBag.KhuVuc = new SelectList(_context.tinhTP.ToList(), "MaKhuVuc", "TenTinhTP");
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
            TaiKhoan tk = null;
            tk = _contextBac.TaiKhoan
                    .Include(t => t.KhachHang)
                    .FirstOrDefault(t => t.TenTaiKhoan == tenTaiKhoan);

            if (tk == null) {
                tk = _contextTrung.TaiKhoan
                        .Include(t => t.KhachHang)
                        .FirstOrDefault(t => t.TenTaiKhoan == tenTaiKhoan);
            }

            if (tk == null)
            {
                tk = _contextNam.TaiKhoan
                        .Include(t => t.KhachHang)
                        .FirstOrDefault(t => t.TenTaiKhoan == tenTaiKhoan);

            }

            if (tk != null)
            {
                var hasher = new PasswordHasher<TaiKhoan>();
                var result = hasher.VerifyHashedPassword(null, tk.MatKhau, matKhau);

                if (result == PasswordVerificationResult.Success)
                {
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, tk.TenTaiKhoan),
                new Claim("HoTen", tk.KhachHang?.HoTen ?? ""),
                new Claim("MaKH", tk.KhachHang?.MaKH ?? ""),
                new Claim("MaKhuVuc", tk.KhachHang?.MaKhuVuc ?? ""),
            };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                    return RedirectToAction("Index", "Home");
                }
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
