using KlimaServisProje.Extensions;
using KlimaServisProje.Models;
using KlimaServisProje.Models.Identity;
using KlimaServisProje.Services;
using KlimaServisProje.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
namespace KlimaServisProje.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IEmailSender _emailSender;

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
            CheckRoles();
        }
        private void CheckRoles()
        {
            foreach (var roleName in RoleModels.Roles)
            {
                if (!_roleManager.RoleExistsAsync(roleName).Result)
                {
                    var result = _roleManager.CreateAsync(new ApplicationRole()
                    {
                        Name = roleName
                    }).Result;
                }
            }
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user != null)
            {
                ModelState.AddModelError(nameof(model.UserName), "Bu kullanıcı adı daha önce alınmıştır.");
                return View(model);
            }
            user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                ModelState.AddModelError(nameof(model.Email), "Bu email adresiyle daha önce kayıt olumuştur.");
                return View(model);
            }
            user = new ApplicationUser()
            {
                Email = model.Email,
                Name = model.Name,
                UserName = model.UserName,
                Surname = model.Surname
            };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                //kullanıcıya rol atama
                var count = _userManager.Users.Count();
                result = await _userManager.AddToRoleAsync(user, count == 1 ? RoleModels.Admin : RoleModels.User);//eğer kullanıcı sayısı 1 ise admin,değilse passive rolü atıyor.
                                                                                                                  //email onay maili
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Scheme);

                var emailMessage = new EmailMessage()
                {
                    Contacts = new string[] { user.Email },
                    Subject = "Confirm Your Email",
                    Body = $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>click here</a>"
                };
                await _emailSender.SendAsync(emailMessage);                                                                                                  //login sayfasına yönlendirme
                return RedirectToAction("Login", "Account");//home controllerın index sayfasına yönlendir
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Bir hata oluştu");
                return View(model);
            }
        }
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Bu Id'ye dair kullanıcı bulunamadı : {userId}");
            }
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result  = await _userManager.ConfirmEmailAsync(user, code);
            ViewBag.StatusMessage = result.Succeeded ? "E-mailiniz onaylanmıştır." : "E-mail onaylanırken bir hata oluştu";
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, true);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);

                if (_userManager.IsInRoleAsync(user, "Admin").Result)
                {
                    //var url = Url.Action("Index", "Manage", new { Areas = "Admin" });
                    //return RedirectToRoute("admin/manage");
                    //return RedirectToAction(""Manage", new { Areas = "admin" });
                    return LocalRedirect("/admin/manage/index");
                }
                else if (_userManager.IsInRoleAsync(user, "Operator").Result)
                {
                    return LocalRedirect("/admin/service/getreports");

                }
                else
                    return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Kullanıcı adı veya şifre hatalı");
                TempData["Message"] = "Kullanıcı adı veya şifre hatalı";
                return View(model);
            }
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ProfileAsync()
        {
            //var user = await _userManager.FindByIdAsync(HttpContext.User) 
            //userID ototik gelmiyor o yüzden extension method yapacağız.
            var user = await _userManager.FindByIdAsync(HttpContext.GetUserId());
            var model = new UserProfileViewModel()
            {
                Email = user.Email,
                Name = user.Name,
                Surname = user.Surname
            };
            return View(model);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Profile(UserProfileViewModel model)
        {
            var user = await _userManager.FindByIdAsync(HttpContext.GetUserId());
            user.Name = model.Name;
            user.Surname = model.Surname;
            if (user.Email != model.Email)
            {
                user.EmailConfirmed = false;
                user.Email = model.Email;
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Scheme);

                var emailMessage = new EmailMessage()
                {
                    Contacts = new string[] { user.Email },
                    Subject = "Confirm Your Email",
                    Body = $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>click here</a>"
                };
                await _emailSender.SendAsync(emailMessage);
            }
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, ModelState.ToFullErrorString());
            }
            return View(model);
        }
        [Authorize]
        [HttpGet]
        public IActionResult UpdatePassword()
        {
            return View();
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdatePassword(PasswordChangeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByIdAsync(HttpContext.GetUserId());
            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                ViewBag.Message = "Parola güncelleme işlemi başarılı";
                return RedirectToAction(nameof(Logout));
            }
            else
            {
                ViewBag.Message = $"Bir hata oluştu: {ModelState.ToFullErrorString()}";
            }
            return RedirectToAction(nameof(Profile));
            //return RedirectToAction("Profile");
        }
        [AllowAnonymous]
        public IActionResult ResetPassword()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ResetPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                ViewBag.Message = "Bu maile bağlı bir kullanıcı bulunamadı";
            }
            else
            {
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Action("ConfirmResetPassword", "Account", new
                {
                    userId = user.Id,
                    code = code
                }, protocol: Request.Scheme); //email resetleme linki oluşturdu.
                var emailMessage = new EmailMessage()
                {
                    Contacts = new string[] { user.Email },
                    Body = $"Please reset your password by clicking <a href= '{HtmlEncoder.Default.Encode(callbackUrl)}'>Here!</a>",
                    Subject = "Reset Password"
                };
                await _emailSender.SendAsync(emailMessage);
                ViewBag.Message = "Mailinize şifre sıfırlama yönergeniz gönderilmiştir.";
            }
            return View();
        }
        [AllowAnonymous]
        public IActionResult ConfirmResetPassword(string userId, string code)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
            {
                return BadRequest("Hatalı istek");
            }
            ViewBag.Code = code;
            ViewBag.UserId = userId;
            return View();
        }
        [AllowAnonymous, HttpPost] //böyle de yazılabiliyo virgülle
        public async Task<IActionResult> ConfirmResetPasswordAsync(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Kullanıcı Bulunamadı");
                return View();
            }
            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Code));
            var result = await _userManager.ResetPasswordAsync(user, code, model.NewPassword);
            if (result.Succeeded == true)
            {
                //email de gönderebilirsin
                TempData["Message"] = "Şifre değiştirme işlemi tamamlandı";
                return View();
            }
            else
            {
                var message = string.Join("<br>", result.Errors.Select(x => x.Description));
                TempData["Message"] = message;
                return View();
            }
        }
    }
}
