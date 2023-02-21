using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace ThuVienPhapLuat.Controllers
{
    public class LoginController : Controller
    {
        //private IDocumentService documentSer;
        //private IUserService userSer;
        //private IRoleService roleSer;
        //private IDonViService donviSer;
        //private ILogService logSer;
        //private string fromAddress;
        //private string mailPassword;
        //public LoginController(
        //    ILogger<LoginController> logger, 
        //    IDocumentService documentSer,
        //    IUserService _userSer,
        //    IHostEnvironment environment,
        //    IRoleService roleSer,
        //    IDonViService donviSer,
        //    ILogService logSer,
        //    IConfigService ConfigSer
        //    ) : base(environment, donviSer)
        //{
        //    this.documentSer = documentSer;
        //    userSer = _userSer;
        //    this.roleSer = roleSer;
        //    this.donviSer = donviSer;
        //    this.logSer = logSer;
        //    var config = ConfigSer.GetById(1);
        //    if (config == null)
        //    {
        //        this.fromAddress = string.Empty;
        //        this.mailPassword = string.Empty;
        //    }
        //    else
        //    {
        //        this.fromAddress = config.Key;
        //        this.mailPassword = config.Value;
        //    }
        //}
        
        //[Authorize]
        //public async Task<IActionResult> Logout()
        //{
        //    await HttpContext.SignOutAsync(
        //            scheme: "SecurityScheme");
        //    RemoveCookie("DonViName");
        //    RemoveCookie("RoleAccess");
        //    RemoveCookie("RoleUser");
        //    RemoveCookie("FullName");
        //    RemoveCookie("Url");
        //    return RedirectToAction("Login", "Login");
        //}

        //private JsonResult FailError(int number)
        //{
        //    return new JsonResult(new
        //    {
        //        successful = false,
        //        error = number
        //    });
        //}

        //[AllowAnonymous]
        //[HttpGet]
        //public IActionResult Login()
        //{
        //    if (this.HttpContext.User.Claims.Any())//đã đăng nhập
        //    {
        //        return RedirectToAction("Index", "Dashboard");
        //    }
        //    return View();
        //}

        //[AllowAnonymous]
        //[HttpPost]
        //public async Task<IActionResult> Login(UserLoginModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

        //    if (this.HttpContext.User.Claims.Any())
        //    {
        //        return RedirectToAction("Index", "Dashboard");
        //    }

        //    var user = userSer.FindByUserNameInLogin(model.UserName);
        //    if (user == null)
        //    {
        //        ModelState.AddModelError("", "Lỗi đăng nhập");
        //        return View(model);
        //    }

        //    //byte[] salt = Convert.FromBase64String(user.Salt);
        //    //string hashpaword = this.getHashPwd(model.Password, salt);
        //    if (!Auth.IsPass(model.Password, user.Password, user.Salt))
        //    {
        //        ModelState.AddModelError("", "Mật khẩu không đúng");
        //        return View(model);
        //    }
        //    Role? role = roleSer.FindById(user.Role);

        //    this.logSer.Add(new Log()
        //    {
        //        Content = LogContent.Login(user.Id, user.FullName),
        //        CreatedDate = DateTime.Now,
        //        Type = LogContent.AuthorizeType
        //    });
        //    if(user.StringRequest != null)
        //    {
        //        user.StringRequest = null;
        //        this.userSer.Update(user);
        //    }
        //    if (role != null && role.Code == RoleType.ADMIN)
        //    {
        //        List<Claim> claims = new List<Claim>
        //        {
        //            new Claim(ClaimTypes.Name, user.Id.ToString()), // id
        //            new Claim(ClaimTypes.Role, RoleType.ADMIN), //admin ? member
        //            new Claim(ClaimTypes.UserData, RoleType.ADMIN)//permission
        //        };

        //        ClaimsIdentity identity = new ClaimsIdentity(claims, "cookie");
        //        ClaimsPrincipal principal = new ClaimsPrincipal(identity);

        //        await HttpContext.SignInAsync(
        //                scheme: "SecurityScheme",
        //                principal: principal,
        //                properties: new AuthenticationProperties
        //                {
        //                    ExpiresUtc = DateTime.UtcNow.AddHours(8)
        //                }).WaitAsync(TimeSpan.FromSeconds(15));

        //        SetCookie("DonViName", "", 8);
        //        SetCookie("RoleAccess", RoleType.ADMIN, 8);
        //        SetCookie("RoleUser", RoleType.ADMIN, 8);
        //        SetCookie("Url", user.UrlImage == null ? string.Empty: user.EditedImage == null ? "null" : user.EditedImage, 8);
        //        SetCookie("FullName", user.FullName == null ? "" : user.FullName, 8);
        //    }
        //    else
        //    {
        //        UserRole? userRole = userSer.RoleById(user.Id);
        //        string roleUser = userRole != null ? userRole.UserRoleCode : "";

        //        List<Claim> claims = new List<Claim>
        //        {
        //            new Claim(ClaimTypes.Name, user.Id.ToString()), // id
        //            new Claim(ClaimTypes.Role, roleUser), //admin ? member
        //            new Claim(ClaimTypes.UserData, userRole != null && userRole.RoleAccess != null ? userRole.RoleAccess : "")//permission
        //        };

        //        ClaimsIdentity identity = new ClaimsIdentity(claims, "cookie");
        //        ClaimsPrincipal principal = new ClaimsPrincipal(identity);

        //        await HttpContext.SignInAsync(
        //                scheme: "SecurityScheme",
        //                principal: principal,
        //                properties: new AuthenticationProperties
        //                {
        //                    ExpiresUtc = DateTime.UtcNow.AddHours(8)
        //                }).WaitAsync(TimeSpan.FromSeconds(15));

        //        SetCookie("DonViName", userRole == null? "": userRole.DonViName != null ? userRole.DonViName : "", 8);
        //        SetCookie("RoleAccess", userRole == null? "": userRole.RoleAccess != null ? userRole.RoleAccess : "", 8);
        //        SetCookie("RoleUser", roleUser, 8);
        //        SetCookie("Url", user.UrlImage == null ? string.Empty : user.EditedImage == null ? "null": user.EditedImage, 8);
        //        SetCookie("FullName", user.FullName == null ? "": user.FullName, 8);

        //        SetCookie("FirstLogin", "true", 1);
        //    }
            
        //    return RedirectToAction("Index", "Dashboard");
        //}


        //[AllowAnonymous]
        //[HttpGet]
        //public IActionResult Forgot()
        //{
        //    return View();
        //}

        //[AllowAnonymous]
        //[HttpPost]
        //public IActionResult Forgot(UserForgotModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (!(new SendMailModule(String.Empty, string.Empty).IsMailValid(model.Email)))
        //        {
        //            ModelState.AddModelError("", "Email không hợp lệ");
        //        }
        //        else if (!userSer.ExistedEmail(model.Email))
        //        {
        //            ModelState.AddModelError("", "Email không tồn tại");
        //        }
        //        else
        //        {
        //            User? user = this.userSer.FindByEmailInLogin(model.Email);
        //            if(user != null)
        //            {
        //                //gửi mail
        //                string host = "http://";
        //                if (HttpContext.Request.IsHttps)
        //                {
        //                    host = "https://";
        //                }
        //                host += HttpContext.Request.Host.Value;

        //                string path = Path.Combine(environment.ContentRootPath, "Template", "ResetPassword.html");
        //                StreamReader stream = new StreamReader(path);

        //                user.StringRequest = Auth.HashPass(user.Id + "-" + user.Email + DateTime.Now.ToString("-dd/MM/yyyy"), Auth.Salt());
        //                this.userSer.Update(user);

        //                string link = host + Url.Action("Reset", "Login", new { Token = user.StringRequest });

        //                string template = stream.ReadToEnd();
        //                template = template.Replace("[User]", user.FullName);
        //                template = template.Replace("[Organize]", "Hội đồng nhân dân");
        //                template = template.Replace("[Email]", user.Email);
        //                template = template.Replace("[Link]", link);

        //                new SendMailModule(this.fromAddress, this.mailPassword).SenddMailInThread(user.Email, "Đổi mới mật khẩu", template, null, string.Empty);

        //                return RedirectToAction("Login");
        //            }
        //            ModelState.AddModelError("", "Tài khoản không tồn tại");
        //        }
        //    }
        //    return View(model);
        //}

        //[AllowAnonymous]
        //[HttpGet]
        //public IActionResult Reset(string Token)
        //{
        //    User? user = this.userSer.FindByRequest(Token);
        //    if (user == null) return NotFound();
        //    return View(new UserResetModel() { Token = Token});
        //}

        //[AllowAnonymous]
        //[HttpPost]
        //public IActionResult Reset(UserResetModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        User? user = this.userSer.FindByRequest(model.Token);
        //        if (user == null) return NotFound();
        //        byte[] salt = Auth.Salt();
        //        user.Password = Auth.HashPass(model.Password, salt);
        //        user.Salt = Auth.SaltStr(salt);
        //        user.StringRequest = null;
        //        this.userSer.Update(user);
        //        return RedirectToAction("Login");
        //    }
        //    return View(model);
        //}

    }
}
