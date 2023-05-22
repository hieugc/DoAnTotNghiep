using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DoAnTotNghiep.Data;
using DoAnTotNghiep.Entity;
using DoAnTotNghiep.ViewModels;
using DoAnTotNghiep.Modules;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.Security.Claims;
using DoAnTotNghiep.Enum;
using NuGet.Protocol;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using NuGet.Common;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static System.Net.WebRequestMethods;
using System.Composition;
using DoAnTotNghiep.Service;
using DoAnTotNghiep.Response;
using static QRCoder.PayloadGenerator;
using Org.BouncyCastle.Utilities.Collections;
using System.Runtime.Intrinsics.X86;

namespace DoAnTotNghiep.Controllers
{
    public class AuthorizeController: BaseController
    {
        //xong rồi
        private readonly DoAnTotNghiepContext _context;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        public AuthorizeController(DoAnTotNghiepContext context, 
                                    IConfiguration configuration,
                                    IHostEnvironment environment,
                                    IUserService userService): base(environment)
        {
            _context = context;
            _configuration = configuration;
            _userService = userService;
        }

        public async Task<IActionResult> SignIn()
        {
            var role = this.GetRole();
            if (role == Role.Member)
            {
                return RedirectToAction("Index", "Home");
            }
            else if (role == Role.Admin)
            {
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                await this.SignOutCookie();
            }
            int IdUser = this.GetIdUser();
            if(IdUser != 0)
            {
                this.SetViewData(new DoAnTotNghiepContext(this._context.GetConfig()), IdUser, Crypto.Salt(this._configuration));
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(LoginViewModel loginViewModel)
        {
            return await this.GeneralSignInAsync(loginViewModel, true);
        }
        private async Task SignClaim(List<Claim> claims, string scheme, DateTimeOffset dateTimeOffset)
        {
            var identity = new ClaimsIdentity(claims, scheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                scheme: scheme,
                principal: principal,
                properties: new AuthenticationProperties()
                {
                    IsPersistent = true,
                    ExpiresUtc = dateTimeOffset
                }).WaitAsync(TimeSpan.FromSeconds(15)
            );
        }
        [HttpPost("/api/SignIn")]
        public async Task<IActionResult> ApiSignInAsync([FromBody] LoginViewModel loginViewModel)
        {
            return await this.GeneralSignInAsync(loginViewModel, false);
        }
        private async Task<IActionResult> GeneralSignInAsync(LoginViewModel loginViewModel, bool isWeb)
        {
            if (ModelState.IsValid)
            {
                var user = this._userService.GetByEmail(loginViewModel.Email);
                if (user != null)
                {
                    if (Crypto.IsPass(loginViewModel.Password, user.Password, user.Salt))
                    {
                        List<Claim> claims = new List<Claim>() {
                            new Claim(ClaimTypes.Name, user.Id.ToString()),
                            new Claim(ClaimTypes.Email, user.Email),
                        };

                        if (user.Role == Role.MemberCode)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, Role.Member));
                        }
                        else
                        {
                            claims.Add(new Claim(ClaimTypes.Role, Role.Admin));
                        }

                        if (isWeb)
                        {
                            await this.SignClaim(claims, Scheme.AuthenticationCookie(), DateTime.UtcNow.AddHours(Cookie.TimeExpireWeb));
                            if(user.Role == Role.AdminCode)
                                return RedirectToAction("Index", "Admin");
                            return RedirectToAction("Index", "Home");
                        }

                        var token = new JwtHelper(this._configuration).GenerateToken(claims);
                        UserInfo.GetEntityRelated(user, this._context);//get Image, numberHouse

                        return Json(new
                        {
                            Status = 200,
                            Message = "Đăng nhập thành công",
                            Data = new
                            {
                                Token = new JwtSecurityTokenHandler().WriteToken(token),
                                Expire = token.ValidTo,
                                UserInfo = new UserInfo(user, Crypto.Salt(this._configuration), this.GetWebsitePath())
                            }
                        });

                    }
                    else
                    {
                        ModelState.AddModelError("PassWord", AuthorizeResponseHttp.InCorrectPassword);
                    }
                }
                else
                {
                    ModelState.AddModelError("", AuthorizeResponseHttp.NotFoundUser);
                }
            }
            if (isWeb)
            {
                return View(loginViewModel);
            }

            return BadRequest(new
            {
                Status = 400,
                Message = this.ModelErrors()
            });
        }

        [HttpGet]
        public IActionResult SignUpCheckMail()
        {
            string role = this.GetRole();
            if (role == Role.UnAuthorize) return View(new RegisterCheckMailViewModel());
            else if (role == Role.Member)
            {
                return RedirectToAction("Index", "Home");
            }
            else if (role == Role.Admin)
            {
                return RedirectToAction("", "Admin");
            }
            return RedirectToAction(nameof(SignUpPassword), new { Email = this.GetEmail() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SignUpCheckMail(RegisterCheckMailViewModel registerViewModel)
        {
            return this.GeneralCheckMail(registerViewModel, true);
        }

        [HttpPost("/api/SignUp/CheckMail")]
        public IActionResult ApiCheckMail([FromBody] RegisterCheckMailViewModel registerViewModel)
        {
            return this.GeneralCheckMail(registerViewModel, false);
        }
        private IActionResult GeneralCheckMail(RegisterCheckMailViewModel registerViewModel, bool isWeb)
        {
            if(isWeb && this.GetIdUser() != 0) return NotFound();
            if (ModelState.IsValid)
            {
                if (!this._userService.IsExistEmail(registerViewModel.Email))//không tồn tại
                {
                    if (isWeb)
                    {
                        this.SetCookie("Email", registerViewModel.Email, null);
                        return RedirectToAction(nameof(SignUpPassword), new { Email = registerViewModel.Email });
                    }

                    return Json(new
                    {
                        Status = 200,
                        Message = AuthorizeResponseHttp.EmailValid,
                        Data = new 
                        {
                            isExisted = false
                        }
                    });
                }
                else if(!isWeb)
                {
                    return Json(new
                    {
                        Status = 200,
                        Message = AuthorizeResponseHttp.EmailValid,
                        Data = new
                        {
                            isExisted = true
                        }
                    });
                }
                else
                {
                    ModelState.AddModelError("Email", AuthorizeResponseHttp.EmailUsed);
                }
            }
            if (isWeb) return View(registerViewModel);

            return BadRequest(new
            {
                Status = 400,
                Message = ModelErrors()
            });
        }


        [HttpGet]
        public IActionResult SignUpPassword(string Email)
        {
            string role = this.GetRole();
            if(role == Role.UnAuthorize) return View(new RegisterPasswordViewModel() { Email = Email});
            else if(role == Role.Member)
            {
                return RedirectToAction("Index", "Home");
            }
            else if (role == Role.Admin)
            {
                return RedirectToAction("", "Admin");
            }
            return RedirectToAction(nameof(SignUpOTP));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUpPassword(RegisterPasswordViewModel registerPasswordViewModel)
        {
            if (this.GetIdUser() != 0) return NotFound();
            if (ModelState.IsValid)
            {
                string? email = this.GetCookie("Email");
                if (string.IsNullOrEmpty(email)) email = registerPasswordViewModel.Email;
                if (!this._userService.IsExistEmail(email))
                {
                    string OTP = OtpGender.RandomOTP();

                    if(this.SendOTP(OTP, registerPasswordViewModel.Email))
                    {
                        var claims = this.GetClaims(OTP, email, registerPasswordViewModel.Password);
                        await this.SignClaim(claims, Scheme.AuthenticationCookie(), DateTime.UtcNow.AddSeconds(135));
                        this.RemoveCookie("Email");
                        return RedirectToAction(nameof(SignUpOTP));
                    }
                    else
                    {
                        ModelState.AddModelError("", "Lỗi hệ thống!!! Vui lòng nhấn gửi lại OTP!!");
                    }
                }
                else
                {
                    ModelState.AddModelError("Email", "Mật khẩu đã tồn tại");
                }
            }
            return View(registerPasswordViewModel);
        }

        [HttpPost("/api/SignUp/Password")]
        public IActionResult ApiPassword([FromBody] MobileRegisterPasswordViewModel data)
        {
            if (ModelState.IsValid)
            {
                if (!this._userService.IsExistEmail(data.Email)) //t gửi xong => không lưu vô DB => phí token tồn tại 5ph
                {
                    string OTP = OtpGender.RandomOTP();
                    if (this.SendOTP(OTP, data.Email)) // đã gưi tới email => không bảo mật
                    {
                        var claims = this.GetClaims(OTP, data.Email, data.Password); 
                        var token = new JwtHelper(this._configuration).GenerateToken(claims);

                        return Json(new
                        {
                            Status = 200,
                            Message = AuthorizeResponseHttp.OtpSent,
                            Data = new 
                            {
                                token = new JwtSecurityTokenHandler().WriteToken(token),
                                expire = token.ValidTo
                            }
                        });
                    }
                    else
                    {
                        ModelState.AddModelError("", ResponseHttp.System500);
                    }
                }
                else
                {
                    ModelState.AddModelError("Email", AuthorizeResponseHttp.EmailUsed);
                }
            }
            return BadRequest(new
            {
                Status = 400,
                Message = this.ModelErrors()
            });
        }
        private bool SendOTP(string OTP, string to)
        {
            string? moduleEmail = this._configuration.GetConnectionString(ConfigurationEmail.Email());
            string? modulePassword = this._configuration.GetConnectionString(ConfigurationEmail.Password());
            return OtpGender.SendOTP(OTP, to, moduleEmail, modulePassword);
        }


        private List<Claim> GetClaims(string Name, string Email, string Password)
        {
            return new List<Claim>() {
                            new Claim(ClaimTypes.Name, Name),
                            new Claim(ClaimTypes.Role,  Enum.Role.None),
                            new Claim(ClaimTypes.Email, Email),
                            new Claim(ClaimTypes.Expired, DateTime.UtcNow.AddSeconds(135).ToString()),
                            new Claim(ClaimTypes.Hash, Password)
                        };
        }

        [HttpGet]
        [Authorize(Roles = Enum.Role.None)]
        public IActionResult SignUpOTP()
        {
            ViewData["timer"] = (DateTime.Parse(this.GetExpired()) - DateTime.UtcNow.AddSeconds(15)).TotalSeconds.ToString();
            return View(new RegisterOTPViewModel());
        }
        [HttpGet]
        [Authorize(Roles = Enum.Role.None)]
        public async Task<IActionResult> ResendSignUpOTP()
        {
            string OTP = OtpGender.RandomOTP();
            string email = this.GetEmail();
            string password = this.GetPassword();
            //tạo OTP

            if (this.SendOTP(OTP, email))
            {
                var claims = this.GetClaims(OTP, email, password);
                await this.SignOutCookie();
                await this.SignClaim(claims, Scheme.AuthenticationCookie(), DateTime.UtcNow.AddSeconds(135));

                return RedirectToAction(nameof(SignUpOTP));
            }
            return NotFound();
        }

        [HttpGet("/api/SignUp/ResendOTP")]
        [Authorize(Roles = Enum.Role.None)]
        public IActionResult ApiResendSignUpOTP()
        {
            string OTP = OtpGender.RandomOTP();
            string email = this.GetEmail();
            string password = this.GetPassword();

            if (this.SendOTP(OTP, email))
            {
                var claims = this.GetClaims(OTP, email, password);
                var token = new JwtHelper(this._configuration).GenerateToken(claims);

                return Json(new
                {
                    Status = 200,
                    Message = AuthorizeResponseHttp.OtpSent,
                    Data = new TokenModel()
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expire = token.ValidTo
                    }
                });
            }
            return BadRequest(new
            {
                Status = 500,
                Message = "Không thể gửi OTP"
            });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Enum.Role.None)]
        public async Task<IActionResult> SignUpOTP(RegisterOTPViewModel model)
        {
            if (ModelState.IsValid)
            {
                int IdUser = this.GetIdUser();
                if(model.OTP.Equals(IdUser.ToString()))
                {
                    DateTime expire = DateTime.Parse(this.GetExpired());
                    if (DateTime.Compare(expire, DateTime.UtcNow) > 0)
                    {
                        byte[] salt = Crypto.Salt();
                        string password = Crypto.HashPass(this.GetPassword(), salt);
                        string email = this.GetEmail();

                        User userModel = Entity.User.CreateUserByRegister(email, password, Crypto.SaltStr(salt));
                        if (this._userService.SaveUser(userModel))
                        {

                            await HttpContext.SignOutAsync(scheme: Scheme.AuthenticationCookie());

                            var claims = new List<Claim>() {
                                new Claim(ClaimTypes.Name, userModel.Id.ToString()),
                                new Claim(ClaimTypes.Role, Role.UpdateInfo),
                                new Claim(ClaimTypes.Email, userModel.Email),
                            };
                            await this.SignClaim(claims, scheme: Scheme.AuthenticationCookie(), DateTime.UtcNow.AddHours(24));
                            return RedirectToAction(nameof(SignUpInfo));
                        }
                        else
                        {
                            ModelState.AddModelError("", ResponseHttp.System500);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("OTP", "OTP quá hạn");
                    }
                }
                else
                {
                    ModelState.AddModelError("OTP", "OTP không đúng");
                }
            }
            return View(model);
        }

        [HttpGet("/api/RefreshToken")]
        public JsonResult RefreshToken([FromBody] RefreshToken data)
        {
            //    var claims = new List<Claim>() {
            //                    new Claim(ClaimTypes.Name, OTP),
            //                    new Claim(ClaimTypes.Role, Role.None),
            //                    new Claim(ClaimTypes.Email, data.Email),
            //                    new Claim(ClaimTypes.Expired, DateTime.UtcNow.AddMinutes(3).ToString()),
            //                    new Claim(ClaimTypes.Hash, data.Password)
            //                };
            //    var token = new JwtHelper(this._configuration).GenerateToken(claims);
            return Json(new
            {
                Status = 200,
                Message = ""
            });
        }

        [HttpPost("/api/SignUp/OTP")]
        [Authorize(Roles = Enum.Role.None)]
        public IActionResult ConfirmOTP([FromBody] RegisterOTPViewModel model)
        {
            if (ModelState.IsValid)
            {
                int IdUser = this.GetIdUser();
                if (model.OTP.Equals(IdUser.ToString()))
                {
                    DateTime expire = DateTime.Parse(this.GetExpired());
                    if (DateTime.Compare(expire, DateTime.UtcNow) > 0)
                    {
                        byte[] salt = Crypto.Salt();
                        string password = Crypto.HashPass(this.GetPassword(), salt);
                        string email = this.GetEmail();

                        User userModel = Entity.User.CreateUserByRegister(email, password, Crypto.SaltStr(salt));
                        if (this._userService.SaveUser(userModel))
                        {
                            var claims = new List<Claim>() {
                                new Claim(ClaimTypes.Name, userModel.Id.ToString()),
                                new Claim(ClaimTypes.Role, Role.UpdateInfo),
                                new Claim(ClaimTypes.Email, userModel.Email),
                            };
                            var token = new JwtHelper(this._configuration).GenerateToken(claims);

                            return Json(new
                            {
                                Status = 200,
                                Message = "Khởi tạo thành công",
                                Data = new
                                {
                                    token = new JwtSecurityTokenHandler().WriteToken(token),
                                    expire = token.ValidTo
                                }
                            });
                        }
                        else
                        {
                            ModelState.AddModelError("", ResponseHttp.System500);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("OTP", "OTP quá hạn");
                    }
                }
                else
                {
                    ModelState.AddModelError("OTP", "OTP không đúng");
                }
            }
            return BadRequest(new
            {
                Status = 400,
                Message = this.ModelErrors()
            });
        }
        [HttpGet]
        [Authorize(Roles = Enum.Role.UpdateInfo)]
        public IActionResult SignUpInfo()
        {
            ViewData["Gender"] = new SelectList(new List<GenderSelect>() { new GenderSelect() { Value = false, Name = "Nữ"}, new GenderSelect() { Value = true, Name = "Nam" } }, "Value", "Name");
            return View(new RegisterInfoViewModel() { Gender = false , BirthDay = DateTime.Now}) ;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Enum.Role.UpdateInfo)]
        public async Task<IActionResult> SignUpInfo(RegisterInfoViewModel model)
        {
            if (ModelState.IsValid)
            {
                int IdUser = this.GetIdUser();
                var user = this._userService.GetById(IdUser);

                if(user != null)
                {
                    user.UpdateInfoUser(model);
                    this._userService.UpdateUser(user);
                    await this.SignOutCookie();
                    return RedirectToAction(nameof(SignIn));
                }
                else
                {
                    await this.SignOutCookie();
                    return this.UnderMaintenance();
                }
            }
            ViewData["Gender"] = new SelectList(new List<GenderSelect>() { new GenderSelect() { Value = false, Name = "Nữ" }, new GenderSelect() { Value = true, Name = "Nam" } }, "Value", "Name");
            return View(model);
        }

        [HttpPost("/api/SignUp/UpdateInfo")]
        [Authorize(Roles = Enum.Role.UpdateInfo)]
        public IActionResult UpdateInfomation([FromBody] RegisterInfoViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = this._userService.GetById(this.GetIdUser());

                if (user != null)
                {

                    user.UpdateInfoUser(model);
                    this._userService.UpdateUser(user);

                    var claims = new List<Claim>() {
                            new Claim(ClaimTypes.Name, user.Id.ToString()),
                            new Claim(ClaimTypes.Role, Role.Member),
                            new Claim(ClaimTypes.Email, user.Email),
                        };
                    var token = new JwtHelper(this._configuration).GenerateToken(claims);
                    UserInfo.GetEntityRelated(user, this._context);
                    return Json(new
                    {
                        Status = 200,
                        Message = "Cập nhật thông tin thành công",
                        Data = new ApiLoginData()
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expire = token.ValidTo,
                            userInfo = new UserInfo(user, Crypto.Salt(this._configuration), this.GetWebsitePath())
                        }
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        Status = 500,
                        Message = ResponseHttp.System500
                    });
                }
            }
            return BadRequest(new
            {
                Status = 400,
                Message = this.ModelErrors()
            });
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await this.SignOutCookie();
            return RedirectToAction(nameof(SignIn));
        }

        [Authorize]
        [HttpGet("api/Logout")]
        public IActionResult ApiLogout()
        {
            HttpContext.Response.Headers.Authorization = string.Empty;
            return Json(new
            {
                Status = 200,
                Message = "Đăng xuất thành công!"
            });
        }

        [HttpGet]
        public IActionResult Forgot()
        {
            if (this.GetIdUser() != 0)
                return RedirectToAction("Index", "Home");
            return View(new RegisterCheckMailViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Forgot(RegisterCheckMailViewModel data)
        {
            if (this.GetIdUser() != 0) return NotFound();
            if (ModelState.IsValid)
            {
                if (this._userService.IsExistEmail(data.Email))
                {
                    string OTP = OtpGender.RandomOTP();
                    //tạo OTP

                    if (this.SendOTP(OTP, data.Email))
                    {
                        var claims = new List<Claim>() {
                            new Claim(ClaimTypes.Name, OTP),
                            new Claim(ClaimTypes.Role, Role.None),
                            new Claim(ClaimTypes.Email, data.Email),
                            new Claim(ClaimTypes.Expired, DateTime.UtcNow.AddSeconds(135).ToString())
                        };
                        await this.SignClaim(claims, Scheme.AuthenticationCookie(), DateTime.UtcNow.AddSeconds(135));

                        return RedirectToAction(nameof(ForgotOTP));
                    }
                    else
                    {
                        ModelState.AddModelError("", "Lỗi hệ thống!!! Vui lòng nhấn gửi lại OTP!!");
                    }
                }
                else
                {
                    ModelState.AddModelError("Email", "Email không tồn tại");
                }
            }
            return View(data);
        }

        [HttpPost("/api/Forgot/CheckMail")]
        public IActionResult ApiForgotCheckMail([FromBody] RegisterCheckMailViewModel data)
        {
            if (ModelState.IsValid)
            {
                if (this._userService.IsExistEmail(data.Email))
                {
                    string OTP = new Random().Next(100000, 999999).ToString();
                    //tạo OTP

                    if (this.SendOTP(OTP, data.Email))
                    {
                        var claims = new List<Claim>() {
                            new Claim(ClaimTypes.Name, OTP),
                            new Claim(ClaimTypes.Role, Role.None),
                            new Claim(ClaimTypes.Email, data.Email),
                            new Claim(ClaimTypes.Expired, DateTime.UtcNow.AddSeconds(135).ToString())
                        };
                        var token = new JwtHelper(this._configuration).GenerateToken(claims);

                        return Json(new
                        {
                            Status = 200,
                            Message = "Đã gửi OTP xác nhận",
                            Data = new TokenModel()
                            {
                                token = new JwtSecurityTokenHandler().WriteToken(token),
                                expire = token.ValidTo
                            }
                        });
                    }
                    else
                    {
                        return BadRequest(new
                        {
                            Status = 500,
                            Message = "Lỗi hệ thống!!! Vui lòng nhấn gửi lại OTP!!"
                        });
                    }
                }
                else
                {
                    ModelState.AddModelError("Email", "Email không tồn tại");
                }
            }
            return BadRequest(new
            {
                Status = 400,
                Message = ModelErrors()
            });
        }
        [HttpGet]
        [Authorize(Roles = Enum.Role.None)]
        public IActionResult ForgotOTP()
        {
            ViewData["timer"] = (DateTime.Parse(this.GetExpired()) - DateTime.UtcNow.AddSeconds(15)).TotalSeconds.ToString();
            return View(new RegisterOTPViewModel());
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Enum.Role.None)]
        public async Task<IActionResult> ForgotOTP(RegisterOTPViewModel data)
        {
            if (ModelState.IsValid)
            {
                int IdUser = this.GetIdUser();
                if (data.OTP.Equals(IdUser.ToString()))
                {
                    DateTime expire = DateTime.Parse(this.GetExpired());
                    if (DateTime.Compare(expire, DateTime.UtcNow) > 0)
                    {
                        string email = this.GetEmail();
                        await this.Logout();
                        if (email == Role.UnAuthorize) return RedirectToAction(nameof(Forgot));

                        var user = this._userService.GetByEmail(email);
                        if (user == null) return NotFound();
                        var claims = new List<Claim>() {
                            new Claim(ClaimTypes.Name, user.Id.ToString()),
                            new Claim(ClaimTypes.Role, Role.Member),
                            new Claim(ClaimTypes.Email, user.Email)
                        };
                        await this.SignClaim(claims, Scheme.AuthenticationCookie(), DateTime.UtcNow.AddMinutes(5));

                        return RedirectToAction(nameof(RefreshPassword));
                    }
                    else
                    {
                        ModelState.AddModelError("OTP", "OTP quá hạn");
                    }
                }
                else
                {
                    ModelState.AddModelError("OTP", "OTP không đúng");
                }
            }
            return View(data);
        }

        [HttpPost("/api/Forgot/OTP")]
        [Authorize(Roles = Enum.Role.None)]
        public IActionResult ApiForgotConfirmOTP([FromBody] RegisterOTPViewModel data)
        {
            if (ModelState.IsValid)
            {
                int IdUser = this.GetIdUser();//OTP
                string email = this.GetEmail();
                if(email == Role.UnAuthorize)
                {
                    return Json(new
                    {
                        Status = 400,
                        Message = "Token quá hạn"
                    });
                }
                if (data.OTP.Equals(IdUser.ToString()))
                {
                    DateTime expire = DateTime.Parse(this.GetExpired());
                    if (DateTime.Compare(expire, DateTime.UtcNow) > 0)
                    {
                        var user = this._userService.GetByEmail(email);
                        if (user == null)
                        {
                            return Json(new
                            {
                                Status = 400,
                                Message = "Không tìm thấy người dùng"
                            });
                        }
                        var claims = new List<Claim>() {
                                new Claim(ClaimTypes.Name, user.Id.ToString()),
                                new Claim(ClaimTypes.Role, Role.Member),
                                new Claim(ClaimTypes.Email, user.Email)
                            };
                        var token = new JwtHelper(this._configuration).GenerateToken(claims);

                        return Json(new
                        {
                            Status = 200,
                            Message = "Mã OTP chính xác",
                            Data = new TokenModel()
                            {
                                token = new JwtSecurityTokenHandler().WriteToken(token),
                                expire = token.ValidTo
                            }
                        });
                    }
                    else
                    {
                        return BadRequest(new
                        {
                            Status = 400,
                            Message = "OTP quá hạn"
                        });
                    }
                }
                else
                {
                    return BadRequest(new
                    {
                        Status = 400,
                        Message = "OTP không đúng"
                    });
                }
            }
            return BadRequest(new
            {
                Status = 400,
                Message = ModelErrors()
            });
        }

        [HttpGet("/api/Forgot/ResendOTP")]
        [Authorize(Roles = Enum.Role.None)]
        public IActionResult ApiForgotResendOTP()
        {
            string OTP = OtpGender.RandomOTP();
            string email = this.GetEmail();

            if (this.SendOTP(OTP, email))
            {
                var claims = new List<Claim>() {
                            new Claim(ClaimTypes.Name, OTP),
                            new Claim(ClaimTypes.Role, Role.None),
                            new Claim(ClaimTypes.Email, email),
                            new Claim(ClaimTypes.Expired, DateTime.UtcNow.AddSeconds(135).ToString())
                        };
                var token = new JwtHelper(this._configuration).GenerateToken(claims);

                return Json(new
                {
                    Status = 200,
                    Message = "Đã gửi OTP xác nhận",
                    Data = new TokenModel()
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expire = token.ValidTo
                    }
                });
            }
            return BadRequest(new
            {
                Status = 400,
                Message = "Không thể gửi OTP"
            });
        }

        [HttpGet]
        [Authorize(Roles = Enum.Role.None)]
        public async Task<IActionResult> ForgotResendOTP()
        {
            string OTP = OtpGender.RandomOTP();
            string email = this.GetEmail();

            if (this.SendOTP(OTP, email))
            {
                var claims = new List<Claim>() {
                            new Claim(ClaimTypes.Name, OTP),
                            new Claim(ClaimTypes.Role, Role.None),
                            new Claim(ClaimTypes.Email, email),
                            new Claim(ClaimTypes.Expired, DateTime.UtcNow.AddSeconds(135).ToString())
                        };
                await this.SignOutCookie();
                await this.SignClaim(claims, Scheme.AuthenticationCookie(), DateTime.UtcNow.AddSeconds(135));

                return RedirectToAction(nameof(ForgotOTP));
            }
            return RedirectToAction(nameof(Forgot));
        }

        [HttpGet]
        [Authorize(Roles = Enum.Role.Member)]
        public IActionResult RefreshPassword()
        {
            return View(new UpdatePasswordViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Enum.Role.Member)]
        public async Task<IActionResult> RefreshPassword(UpdatePasswordViewModel data)
        {
            if (ModelState.IsValid)
            {
                string email = this.GetEmail();

                var user = this._userService.GetByEmail(email);
                if(user == null)
                {
                    ModelState.AddModelError("", "Người dùng không tồn tại");
                }
                else
                {
                    var salt = Crypto.Salt();
                    user.Password = Crypto.HashPass(data.Password, salt);
                    user.Salt = Crypto.SaltStr(salt);
                    this._userService.UpdateUser(user);
                    await this.SignOutCookie();
                    return RedirectToAction(nameof(SignIn));
                }
            }
            return View(data);
        }

        [HttpPost("/api/Forgot/Password")]
        [Authorize(Roles = Enum.Role.Member)]
        public IActionResult ApiRefreshPassword([FromBody] UpdatePasswordViewModel data)
        {
            if (ModelState.IsValid)
            {
                string email = this.GetEmail();
                var user = this._userService.GetByEmail(email);
                if (user == null)
                {
                    return BadRequest(new
                    {
                        Status = 400,
                        Message = "Người dùng không tồn tại"
                    });
                }
                else
                {
                    var salt = Crypto.Salt();
                    user.Password = Crypto.HashPass(data.Password, salt);
                    user.Salt = Crypto.SaltStr(salt);
                    this._userService.UpdateUser(user);
                    return Json(new
                    {
                        Status = 200,
                        Message = "Cập nhật mật khẩu thành công"
                    });
                }
            }
            return BadRequest(new
            {
                Status = 400,
                Message = ModelErrors()
            });
        }
        protected int GetIdUser(string token)
        {
            try
            {
                JwtUserModel jwtUserModel = new JwtHelper(this._configuration).GetUserFromToken(token);
                return jwtUserModel.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 0;
            }
        }
        private async Task SignOutCookie()
        {
            await HttpContext.SignOutAsync(
                    scheme: Scheme.AuthenticationCookie());

            RemoveCookie(Enum.Cookie.RoleAccess());
            RemoveCookie(Enum.Cookie.UserInfo());
        }








        /*
        [HttpGet("/User/AddUser")]
        public IActionResult AddUser()
        {

            byte[] salt4 = Crypto.Salt();
            User user4 = new User()
            {
                FirstName = "Phạm Minh",
                LastName = "Hiếu",
                Gender = true,
                BirthDay = DateTime.Now,
                Email = "admin@gmail.com",
                Password = Crypto.HashPass("Vinova123", salt4),
                Salt = Crypto.SaltStr(salt4),
                PhoneNumber = "0973409127",
                Point = 10000,
                BonusPoint = 10000,
                IdFile = null,
                UserRating = 0,
                Role = Role.AdminCode
            }; 
            byte[] salt1 = Crypto.Salt();
            User user = new User()
            {
                FirstName = "Lộc Minh",
                LastName = "Hiếu",
                Gender = true,
                BirthDay = DateTime.Now,
                Email = "jayson@vinova.com.sg",
                Password = Crypto.HashPass("Vinova123", salt1),
                Salt = Crypto.SaltStr(salt1),
                PhoneNumber = "0973409127",
                Point = 10000,
                BonusPoint = 10000,
                IdFile = null,
                UserRating = 0,
                Role = Role.MemberCode
            };
             byte[] salt2 = Crypto.Salt();
            User user2 = new User()
            {
                FirstName = "Phạm Minh",
                LastName = "Hiếu",
                Gender = true,
                BirthDay = DateTime.Now,
                Email = "hieu.phamgc@gmail.com",
                Password = Crypto.HashPass("Vinova123", salt2),
                Salt = Crypto.SaltStr(salt2),
                PhoneNumber = "0973409127",
                Point = 10000,
                BonusPoint = 10000,
                IdFile = null,
                UserRating = 0,
                Role = Role.MemberCode
            };

            byte[] salt3 = Crypto.Salt();
            User user3 = new User()
            {
                FirstName = "Bùi Phát",
                LastName = "Đạt",
                Gender = true,
                BirthDay = DateTime.Now,
                Email = "buiphatdat2001@gmail.com",
                Password = Crypto.HashPass("Vinova123", salt2),
                Salt = Crypto.SaltStr(salt2),
                PhoneNumber = "0973409127",
                Point = 10000,
                BonusPoint = 10000,
                IdFile = null,
                UserRating = 0,
                Role = Role.MemberCode
            };

            
            this._context.AddRange(new List<User>()
            {
                user, user2, user3, user4
            });

            this._context.AddRange(new List<User>()
            {
                user4
            });
            this._context.SaveChanges();


            return Json(new { 
                Status = 200,
                Message = "oke"
            });
        }
        */
    }
}
