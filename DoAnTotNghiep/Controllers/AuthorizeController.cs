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

namespace DoAnTotNghiep.Controllers
{
    public class AuthorizeController: BaseController
    {
        private readonly DoAnTotNghiepContext _context;
        private readonly IConfiguration _configuration;

        public AuthorizeController(DoAnTotNghiepContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<IActionResult> SignIn()
        {
            var role = this.GetRole();
            if (role == Role.NoneString() || role == Role.UpdateInfo()) await this.SignOutCookie();
            if (this.GetIdUser() != 0) return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = this._context.Users.Where(x => x.Email == loginViewModel.Email).ToList();

                if (user != null && user.Count() == 1)
                {
                    var checkUser = user.First();
                    if (Crypto.IsPass(loginViewModel.Password, checkUser.Password, checkUser.Salt))
                    {
                        var claims = new List<Claim>() {
                            new Claim(ClaimTypes.Name, checkUser.Id.ToString()),
                            new Claim(ClaimTypes.Role, Role.MemberString()),
                            new Claim(ClaimTypes.Email, checkUser.Email),
                        };

                        await this.SignClaim(claims, Scheme.AuthenticationCookie(), DateTime.UtcNow.AddHours(24));
                        SetCookie(Enum.Cookie.RoleAccess(), Role.MemberString(), 24);

                        this._context.Entry(checkUser).Reference(m => m.Files).Load();

                        byte[] RSA = Crypto.Salt(this._configuration);
                        string userInfo = JsonConvert.SerializeObject(new UserMessageViewModel(checkUser, RSA, this.GetWebsitePath()));

                        SetCookie(Enum.Cookie.UserInfo(), userInfo, 24);

                        this._context.Entry(checkUser).Collection(m => m.ChatRoom).Query().Load();

                        if(checkUser.ChatRoom != null)
                        {
                            var ids = checkUser.ChatRoom.Select(m => m.IdChatRoom);
                            SetCookie(Enum.Cookie.ChatRoom(), string.Join(",", ids), 24);
                        }
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("PassWord", "Mật khẩu không đúng");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Không tìm thấy người dùng");
                }
            }
            return View(loginViewModel);
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

        [HttpPost("/api/SignIn")]//done
        public IActionResult ApiSignIn([FromBody] LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = this._context.Users.Where(x => x.Email == loginViewModel.Email);
                if (user != null && user.Count() == 1)
                {
                    var checkUser = user.First();
                    if (Crypto.IsPass(loginViewModel.Password, checkUser.Password, checkUser.Salt))
                    {
                        var token = new JwtHelper(this._configuration).GenerateToken(new List<Claim>() {
                            new Claim(ClaimTypes.Name, checkUser.Id.ToString()),
                            new Claim(ClaimTypes.Role, Role.MemberString()),
                            new Claim(ClaimTypes.Email, checkUser.Email),
                        });

                        return Json(new
                        {
                            Status = 200,
                            Message = "Đăng nhập thành công",
                            Token = new JwtSecurityTokenHandler().WriteToken(token),
                            Data = new UserInfo(checkUser, Crypto.Salt(this._configuration), this.GetWebsitePath()),
                            Expires = token.ValidTo
                        });
                    }
                    else
                    {
                        ModelState.AddModelError("PassWord", "Mật khẩu không đúng");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Không tìm thấy người dùng");
                }
            }
            return Json(new { 
                Status = 400,
                Message = this.ModelErrors()
            });
        }

        [HttpGet]
        public IActionResult SignUpCheckMail()
        {
            string role = this.GetRole();
            if(role == Role.UnAuthorize()) return View(new RegisterCheckMailViewModel());
            else if(role == Role.MemberString())
            {
                return RedirectToAction("Index", "Home");
            }
            else if (role == Role.AdminString())
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction(nameof(SignUpPassword), new {Email = this.GetEmail() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SignUpCheckMail(RegisterCheckMailViewModel registerViewModel)
        {
            if (this.GetIdUser() != 0) return NotFound();
            if (ModelState.IsValid)
            {
                if (!this.IsMailExist(registerViewModel.Email))
                {
                    this.SetCookie("Email", registerViewModel.Email, null);
                    return RedirectToAction(nameof(SignUpPassword), new {Email = registerViewModel.Email });
                }
                ModelState.AddModelError("Email", "Email đã tồn tại");
            }
            return View(registerViewModel);
        }

        [HttpPost("/api/SignUpCheckMail")]
        public JsonResult ApiCheckMail([FromBody] RegisterCheckMailViewModel registerViewModel)
        {
            if (ModelState.IsValid)
            {
                if (!this.IsMailExist(registerViewModel.Email))
                {
                    return Json(new
                    {
                        Status = 200,
                        Message = "Email có thể sử dụng",
                        Data = true
                    });
                }
                return Json(new
                {
                    Status = 200,
                    Message = "Email tồn tại trong hệ thống",
                    Data = false
                });
            }
            return Json(new
            {
                Status = 400,
                Message = ModelErrors(),
                Data = false
            });
        }

        [HttpGet]
        public IActionResult SignUpPassword(string Email)
        {
            string role = this.GetRole();
            if(role == Role.UnAuthorize()) return View(new RegisterPasswordViewModel() { Email = Email});
            else if(role == Role.MemberString())
            {
                return RedirectToAction("Index", "Home");
            }
            else if (role == Role.AdminString())
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction(nameof(SignUpOTP));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]//oke
        public async Task<IActionResult> SignUpPassword(RegisterPasswordViewModel registerPasswordViewModel)
        {
            if (this.GetIdUser() != 0) return NotFound();
            if (ModelState.IsValid)
            {
                string? email = this.GetCookie("Email");
                if (string.IsNullOrEmpty(email)) email = registerPasswordViewModel.Email;
                if (!this.IsMailExist(email))
                {
                    string OTP = new Random().Next(100000, 999999).ToString();
                    //tạo OTP

                    if(this.SendOTP(OTP, registerPasswordViewModel.Email))
                    {
                        var claims = new List<Claim>() {
                            new Claim(ClaimTypes.Name, OTP),
                            new Claim(ClaimTypes.Role, Role.NoneString()),
                            new Claim(ClaimTypes.Email, registerPasswordViewModel.Email),
                            new Claim(ClaimTypes.Expired, DateTime.UtcNow.AddMinutes(3).ToString()),
                            new Claim(ClaimTypes.Hash, registerPasswordViewModel.Password)
                        };
                        await this.SignClaim(claims, Scheme.AuthenticationCookie(), DateTime.UtcNow.AddMinutes(3));
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

        [HttpPost("/api/SignPassword")]
        public JsonResult ApiPassword([FromBody] MobileRegisterPasswordViewModel data)
        {
            if (ModelState.IsValid)
            {
                if (!this.IsMailExist(data.Email))
                {
                    string OTP = new Random().Next(100000, 999999).ToString();
                    //tạo OTP

                    if (this.SendOTP(OTP, data.Email))
                    {
                        var claims = new List<Claim>() {
                            new Claim(ClaimTypes.Name, OTP),
                            new Claim(ClaimTypes.Role, Role.NoneString()),
                            new Claim(ClaimTypes.Email, data.Email),
                            new Claim(ClaimTypes.Expired, DateTime.UtcNow.AddMinutes(3).ToString()),
                            new Claim(ClaimTypes.Hash, data.Password)
                        };
                        var token = new JwtHelper(this._configuration).GenerateToken(claims);

                        return Json(new
                        {
                            Status = 200,
                            Message = "Đã gửi OTP xác nhận",
                            Token = new JwtSecurityTokenHandler().WriteToken(token),
                            Expires = token.ValidTo
                        });
                    }
                    else
                    {
                        ModelState.AddModelError("", "Hệ thống đang bảo trì vui lòng quay lại sau!!");
                    }
                }
                else
                {
                    ModelState.AddModelError("Email", "Mật khẩu đã tồn tại");
                }
            }
            return Json(new
            {
                Status = 400,
                Message = this.ModelErrors()
            });
        }
        private bool SendOTP(string OTP, string to)
        {
            string? moduleEmail = this._configuration.GetConnectionString(ConfigurationEmail.Email());
            string? modulePassword = this._configuration.GetConnectionString(ConfigurationEmail.Password());
            if (string.IsNullOrEmpty(moduleEmail) || string.IsNullOrEmpty(modulePassword)) return false;

            Email sender = new Email(moduleEmail, modulePassword);
            sender.SendMail(to, Subject.SendOTP(), OTP, null, string.Empty);
            return true;
        }
        [HttpGet]
        [Authorize(Roles = "NONE")]
        public IActionResult SignUpOTP()
        {
            return View(new RegisterOTPViewModel());
        }
        [HttpGet]
        [Authorize(Roles = "NONE")]
        public async Task<IActionResult> ResendSignUpOTP()
        {
            string OTP = new Random().Next(100000, 999999).ToString();
            string email = this.GetEmail();
            string password = this.GetPassword();
            //tạo OTP

            if (this.SendOTP(OTP, email))
            {
                var claims = new List<Claim>() {
                            new Claim(ClaimTypes.Name, OTP),
                            new Claim(ClaimTypes.Role, Role.NoneString()),
                            new Claim(ClaimTypes.Email, email),
                            new Claim(ClaimTypes.Expired, DateTime.UtcNow.AddMinutes(3).ToString()),
                            new Claim(ClaimTypes.Hash, password)
                        };
                await this.SignClaim(claims, Scheme.AuthenticationCookie(), DateTime.UtcNow.AddMinutes(3));

                return RedirectToAction(nameof(SignUpOTP));
            }
            return NotFound();
        }

        [HttpGet("/api/SignUp/ResendOTP")]
        [Authorize(Roles = "NONE")]
        public JsonResult ApiResendSignUpOTP()
        {
            string OTP = new Random().Next(100000, 999999).ToString();
            string email = this.GetEmail();
            string password = this.GetPassword();
            //tạo OTP

            if (this.SendOTP(OTP, email))
            {
                var claims = new List<Claim>() {
                            new Claim(ClaimTypes.Name, OTP),
                            new Claim(ClaimTypes.Role, Role.NoneString()),
                            new Claim(ClaimTypes.Email, email),
                            new Claim(ClaimTypes.Expired, DateTime.UtcNow.AddMinutes(3).ToString()),
                            new Claim(ClaimTypes.Hash, password)
                        };
                var token = new JwtHelper(this._configuration).GenerateToken(claims);

                return Json(new
                {
                    Status = 200,
                    Message = "Đã gửi OTP xác nhận",
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    Expires = token.ValidTo
                });
            }
            return Json(new
            {
                Status = 500,
                Message = "Không thể gửi OTP"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "NONE")]
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


                        User userModel = new User
                        {
                            Email = email,
                            Password = password,
                            Salt = Crypto.SaltStr(salt),
                            PhoneNumber = string.Empty,
                            FirstName = string.Empty,
                            LastName = string.Empty,
                            Point = 0,
                            BonusPoint = 0,
                            IdFile = null,
                            BirthDay = DateTime.UtcNow,
                            Gender = false
                        };

                        this._context.Add(userModel);
                        await _context.SaveChangesAsync();

                        await HttpContext.SignOutAsync(scheme: Scheme.AuthenticationCookie());

                        var claims = new List<Claim>() {
                            new Claim(ClaimTypes.Name, userModel.Id.ToString()),
                            new Claim(ClaimTypes.Role, Role.UpdateInfo()),
                            new Claim(ClaimTypes.Email, userModel.Email),
                        };
                        await this.SignClaim(claims, scheme: Scheme.AuthenticationCookie(), DateTime.UtcNow.AddHours(24));

                        return RedirectToAction(nameof(SignUpInfo));
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
            //                    new Claim(ClaimTypes.Role, Role.NoneString()),
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

        [HttpPost("/api/ConfirmOTP")]
        [Authorize(Roles = "NONE")]
        public async Task<JsonResult> ConfirmOTP([FromBody] RegisterOTPViewModel model)
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
                        User userModel = new User
                        {
                            Email = email,
                            Password = password,
                            Salt = Crypto.SaltStr(salt),
                            PhoneNumber = string.Empty,
                            FirstName = string.Empty,
                            LastName = string.Empty,
                            Point = 0,
                            BonusPoint = 0,
                            IdFile = null,
                            BirthDay = DateTime.UtcNow,
                            Gender = false
                        };

                        this._context.Add(userModel);
                        await _context.SaveChangesAsync();


                        var claims = new List<Claim>() {
                            new Claim(ClaimTypes.Name, userModel.Id.ToString()),
                            new Claim(ClaimTypes.Role, Role.MemberString()),
                            new Claim(ClaimTypes.Email, userModel.Email),
                        };
                        var token = new JwtHelper(this._configuration).GenerateToken(claims);

                        return Json(new
                        {
                            Status = 200,
                            Message = "Khởi tạo thành công",
                            Token = new JwtSecurityTokenHandler().WriteToken(token),
                            Expires = token.ValidTo
                        });
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
            return Json(new
            {
                Status = 400,
                Message = this.ModelErrors()
            });
        }
        [HttpGet]
        [Authorize(Roles = "UPDATEINFO")]
        public IActionResult SignUpInfo()
        {
            ViewData["Gender"] = new SelectList(new List<GenderSelect>() { new GenderSelect() { Value = false, Name = "Nữ"}, new GenderSelect() { Value = true, Name = "Nam" } }, "Value", "Name");
            return View(new RegisterInfoViewModel() { Gender = false , BirthDay = DateTime.Now}) ;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "UPDATEINFO")]
        public async Task<IActionResult> SignUpInfo(RegisterInfoViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = this._context.Users.Where(m => m.Id == this.GetIdUser()).FirstOrDefault();

                if(user != null)
                {
                    user.PhoneNumber = model.PhoneNumber;
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.BirthDay = model.BirthDay;
                    user.Gender = model.Gender;
                    this._context.Update(user);
                    await _context.SaveChangesAsync();

                    await this.SignOutCookie();

                    var claims = new List<Claim>() {
                            new Claim(ClaimTypes.Name, user.Id.ToString()),
                            new Claim(ClaimTypes.Role, Role.MemberString()),
                            new Claim(ClaimTypes.Email, user.Email),
                        };
                    await this.SignClaim(claims, scheme: Scheme.AuthenticationCookie(), DateTime.UtcNow.AddHours(24));

                    return RedirectToAction("Index", "Home");
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

        [HttpPost("/api/UpdateInfomation")]
        [Authorize(Roles = "MEMBER")]
        public async Task<JsonResult> UpdateInfomation([FromBody] RegisterInfoViewModel model)
        { 
            if (ModelState.IsValid)
            {
                var user = this._context.Users.Where(m => m.Id == this.GetIdUser()).FirstOrDefault();

                if (user != null)
                {
                    user.PhoneNumber = model.PhoneNumber;
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.BirthDay = model.BirthDay;
                    user.Gender = model.Gender;
                    this._context.Update(user);
                    await _context.SaveChangesAsync();
                    return Json(new
                    {
                        Status = 200,
                        Message = "Cập nhật thông tin thành công",
                        Data = new UserInfo(user, Crypto.Salt(this._configuration), this.GetWebsitePath()),

                    });
                }
                else
                {
                    return Json(new
                    {
                        Status = 500,
                        Message = "Hệ thống cần cập nhật"
                    });
                }
            }
            return Json(new
            {
                Status = 400,
                Message = this.ModelErrors()
            });
        }
        private bool IsMailExist(string Email)
        {
            var email = this._context.Users.Where(m => m.Email == Email);
            return email.Any();
        }



        //[HttpGet]
        //public IActionResult SignUp()
        //{
        //    if (this.GetIdUser() != 0) return RedirectToAction("Index", "Home");
        //    return View(new RegisterViewModel());
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> SignUp(RegisterViewModel registerViewModel)
        //{
        //    if (this.GetIdUser() != 0) return NotFound();
        //    if (ModelState.IsValid)
        //    {
        //        byte[] salt = Crypto.Salt();

        //        registerViewModel.Password = Crypto.HashPass(registerViewModel.Password, salt);

        //        User userModel = new User
        //        {
        //            Email = registerViewModel.Email,
        //            Password = registerViewModel.Password,
        //            Salt = Crypto.SaltStr(salt),
        //            PhoneNumber = registerViewModel.PhoneNumber,
        //            FirstName = registerViewModel.FirstName,
        //            LastName  = registerViewModel.LastName,
        //            Point = 0,
        //            BonusPoint = 0,
        //            IdFile = null
        //        };

        //        this._context.Add(userModel);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(SignIn));
        //    }
        //    return View(registerViewModel);
        //}

        //[HttpPost("/api/SignUp")]
        //public async Task<IActionResult> ApiSignUp([FromBody] RegisterViewModel registerViewModel)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        byte[] salt = Crypto.Salt();
        //        registerViewModel.Password = Crypto.HashPass(registerViewModel.Password, salt);

        //        User userModel = new User
        //        {
        //            Email = registerViewModel.Email,
        //            Password = registerViewModel.Password,
        //            Salt = Crypto.SaltStr(salt),
        //            PhoneNumber = registerViewModel.PhoneNumber,
        //            FirstName = registerViewModel.FirstName,
        //            LastName = registerViewModel.LastName,
        //            Point = 0,
        //            BonusPoint = 0,
        //            IdFile = null
        //        };

        //        this._context.Add(userModel);
        //        await _context.SaveChangesAsync();

        //        return Json(new
        //        {
        //            Status = 200,
        //            Message = "Đăng ký thành công"
        //        });
        //    }
        //    return Json(new
        //    {
        //        Status = 400,
        //        Message = this.ModelErrors()
        //    });
        //}

        
        
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
                if (this.IsMailExist(data.Email))
                {
                    string OTP = new Random().Next(100000, 999999).ToString();
                    //tạo OTP

                    if (this.SendOTP(OTP, data.Email))
                    {
                        var claims = new List<Claim>() {
                            new Claim(ClaimTypes.Name, OTP),
                            new Claim(ClaimTypes.Role, Role.NoneString()),
                            new Claim(ClaimTypes.Email, data.Email),
                            new Claim(ClaimTypes.Expired, DateTime.UtcNow.AddMinutes(3).ToString())
                        };
                        await this.SignClaim(claims, Scheme.AuthenticationCookie(), DateTime.UtcNow.AddMinutes(3));

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

        [HttpPost("/api/ForgotCheckMail")]
        public JsonResult ApiForgotCheckMail([FromBody] RegisterCheckMailViewModel data)
        {
            if (ModelState.IsValid)
            {
                if (this.IsMailExist(data.Email))
                {
                    string OTP = new Random().Next(100000, 999999).ToString();
                    //tạo OTP

                    if (this.SendOTP(OTP, data.Email))
                    {
                        var claims = new List<Claim>() {
                            new Claim(ClaimTypes.Name, OTP),
                            new Claim(ClaimTypes.Role, Role.NoneString()),
                            new Claim(ClaimTypes.Email, data.Email),
                            new Claim(ClaimTypes.Expired, DateTime.UtcNow.AddMinutes(3).ToString())
                        };
                        var token = new JwtHelper(this._configuration).GenerateToken(claims);

                        return Json(new
                        {
                            Status = 200,
                            Message = "Đã gửi OTP xác nhận",
                            Token = new JwtSecurityTokenHandler().WriteToken(token),
                            Expires = token.ValidTo
                        });
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
            return Json(new
            {
                Status = 400,
                Message = ModelErrors(),
                Data = false
            });
        }
        [HttpGet]
        [Authorize(Roles = "NONE")]
        public IActionResult ForgotOTP()
        {
            return View(new RegisterOTPViewModel());
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "NONE")]
        public IActionResult ForgotOTP(RegisterOTPViewModel data)
        {
            if (ModelState.IsValid)
            {
                int IdUser = this.GetIdUser();
                if (data.OTP.Equals(IdUser.ToString()))
                {
                    DateTime expire = DateTime.Parse(this.GetExpired());
                    if (DateTime.Compare(expire, DateTime.UtcNow) > 0)
                    {
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

        [HttpPost("/api/ForgotConfirmOTP")]
        [Authorize(Roles = "NONE")]
        public JsonResult ApiForgotConfirmOTP([FromBody] RegisterOTPViewModel data)
        {
            if (ModelState.IsValid)
            {
                int IdUser = this.GetIdUser();
                if (data.OTP.Equals(IdUser.ToString()))
                {
                    DateTime expire = DateTime.Parse(this.GetExpired());
                    if (DateTime.Compare(expire, DateTime.UtcNow) > 0)
                    {
                        return Json(new
                        {
                            Status = 200,
                            Message = "Mã OTP chính xác"
                        });
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
            return Json(new
            {
                Status = 400,
                Message = ModelErrors(),
                Data = false
            });
        }

        [HttpGet("/api/Forgot/ResendOTP")]
        [Authorize(Roles = "NONE")]
        public JsonResult ApiForgotResendOTP()
        {
            string OTP = new Random().Next(100000, 999999).ToString();
            string email = this.GetEmail();

            if (this.SendOTP(OTP, email))
            {
                var claims = new List<Claim>() {
                            new Claim(ClaimTypes.Name, OTP),
                            new Claim(ClaimTypes.Role, Role.NoneString()),
                            new Claim(ClaimTypes.Email, email),
                            new Claim(ClaimTypes.Expired, DateTime.UtcNow.AddMinutes(3).ToString())
                        };
                var token = new JwtHelper(this._configuration).GenerateToken(claims);

                return Json(new
                {
                    Status = 200,
                    Message = "Đã gửi OTP xác nhận",
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    Expires = token.ValidTo
                });
            }
            return Json(new
            {
                Status = 200,
                Message = "Không thể gửi OTP",
                Data = false
            });
        }

        [HttpGet]
        [Authorize(Roles = "NONE")]
        public async Task<IActionResult> ForgotResendOTP()
        {
            string OTP = new Random().Next(100000, 999999).ToString();
            string email = this.GetEmail();

            if (this.SendOTP(OTP, email))
            {
                var claims = new List<Claim>() {
                            new Claim(ClaimTypes.Name, OTP),
                            new Claim(ClaimTypes.Role, Role.NoneString()),
                            new Claim(ClaimTypes.Email, email),
                            new Claim(ClaimTypes.Expired, DateTime.UtcNow.AddMinutes(3).ToString())
                        };
                await this.SignClaim(claims, Scheme.AuthenticationCookie(), DateTime.UtcNow.AddMinutes(4));

                return RedirectToAction(nameof(ForgotOTP));
            }
            return RedirectToAction(nameof(Forgot));
        }

        [HttpGet]
        [Authorize(Roles = "NONE")]
        public IActionResult RefreshPassword()
        {
            return View(new UpdatePasswordViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "NONE")]
        public async Task<IActionResult> RefreshPassword(UpdatePasswordViewModel data)
        {
            if (ModelState.IsValid)
            {
                string email = this.GetEmail();

                var user = this._context.Users.Where(m => m.Email == email).FirstOrDefault();
                if(user == null)
                {
                    ModelState.AddModelError("", "Người dùng không tồn tại");
                }
                else
                {
                    var salt = Crypto.Salt();
                    user.Password = Crypto.HashPass(data.Password, salt);
                    user.Salt = Crypto.SaltStr(salt);
                    this._context.Users.Update(user);
                    this._context.SaveChanges();
                    await this.SignOutCookie();
                    return RedirectToAction(nameof(SignIn));
                }
            }
            return View(data);
        }

        [HttpPost("/api/Forgot/RefreshPassword")]
        [Authorize(Roles = "NONE")]
        public JsonResult ApiRefreshPassword([FromBody] UpdatePasswordViewModel data)
        {
            if (ModelState.IsValid)
            {
                string email = this.GetEmail();
                var user = this._context.Users.Where(m => m.Email == email).FirstOrDefault();
                if (user == null)
                {
                    ModelState.AddModelError("", "Người dùng không tồn tại");
                }
                else
                {
                    var salt = Crypto.Salt();
                    user.Password = Crypto.HashPass(data.Password, salt);
                    user.Salt = Crypto.SaltStr(salt);
                    this._context.Users.Update(user);
                    this._context.SaveChanges();
                    var claims = new List<Claim>() {
                            new Claim(ClaimTypes.Name, user.Id.ToString()),
                            new Claim(ClaimTypes.Role, Role.MemberString()),
                            new Claim(ClaimTypes.Email, user.Email)
                        };
                    var token = new JwtHelper(this._configuration).GenerateToken(claims);
                    return Json(new
                    {
                        Status = 200,
                        Message = "Cập nhật mật khẩu thành công",
                        Data = new UserInfo(user, Crypto.Salt(this._configuration), this.GetWebsitePath()),
                        Token = new JwtSecurityTokenHandler().WriteToken(token),
                        Expires = token.ValidTo
                    });
                }
            }
            return Json(new
            {
                Status = 400,
                Message = ModelErrors(),
                Data = false
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
            RemoveCookie(Enum.Cookie.ChatRoom());
            RemoveCookie(Enum.Cookie.DataChat());
            RemoveCookie(Enum.Cookie.UserInfo());
        }

        //public IActionResult TestSignUp()
        //{
        //    byte[] salt = Crypto.Salt();
        //    string password = Crypto.HashPass(this.GetPassword(), salt);

        //    User userModel = new User
        //    {
        //        Email = "phamminhhieu1594@gmail.com",
        //        Password = password,
        //        Salt = Crypto.SaltStr(salt),
        //        PhoneNumber = string.Empty,
        //        FirstName = string.Empty,
        //        LastName = string.Empty,
        //        Point = 0,
        //        BonusPoint = 0,
        //        IdFile = null,
        //        BirthDay = DateTime.UtcNow,
        //        Gender = false
        //    };

        //    this._context.Add(userModel);
        //    this._context.SaveChangesAsync();
        //    return RedirectToAction("Index", "Home");
        //}
    }
}
