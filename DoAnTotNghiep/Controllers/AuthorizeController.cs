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

        public IActionResult SignIn()
        {
            if (this.GetIdUser() != 0) return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = this._context.Users.Where(x => x.Email == loginViewModel.Email);

                if (user != null && user.AsEnumerable().Count() == 1)
                {
                    var checkUser = user.First();
                    if (Crypto.IsPass(loginViewModel.Password, checkUser.Password, checkUser.Salt))
                    {
                        var claims = new List<Claim>() {
                            new Claim(ClaimTypes.Name, checkUser.Id.ToString()),
                            new Claim(ClaimTypes.Role, Role.MemberString()),
                            new Claim(ClaimTypes.Email, checkUser.Email),
                        };

                        var identity = new ClaimsIdentity(claims, Scheme.AuthenticationCookie());
                        var principal = new ClaimsPrincipal(identity);

                        await HttpContext.SignInAsync(
                            scheme: Scheme.AuthenticationCookie(),
                            principal: principal,
                            properties: new AuthenticationProperties()
                            {
                                IsPersistent = true,
                                ExpiresUtc = DateTime.UtcNow.AddHours(24)
                            }).WaitAsync(TimeSpan.FromSeconds(15)
                        );

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


        [HttpPost("/api/SignIn")]
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

                        return Ok(new
                        {
                            Token = new JwtSecurityTokenHandler().WriteToken(token),
                            Expires = token.ValidTo,
                            DisplayName = string.Join(" ", new
                            {
                                checkUser.FirstName,
                                checkUser.LastName
                            })
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
            return Unauthorized(new { 
                Message = this.ModelErrors()
            });
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            if (this.GetIdUser() != 0) return RedirectToAction("Index", "Home");
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(RegisterViewModel registerViewModel)
        {
            if (this.GetIdUser() != 0) return NotFound();
            if (ModelState.IsValid)
            {
                byte[] salt = Crypto.Salt();

                registerViewModel.Password = Crypto.HashPass(registerViewModel.Password, salt);

                User userModel = new User
                {
                    Email = registerViewModel.Email,
                    Password = registerViewModel.Password,
                    Salt = Crypto.SaltStr(salt),
                    PhoneNumber = registerViewModel.PhoneNumber,
                    FirstName = registerViewModel.FirstName,
                    LastName  = registerViewModel.LastName,
                    Point = 0,
                    BonusPoint = 0,
                    IdFile = null
                };

                this._context.Add(userModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(SignIn));
            }
            return View(registerViewModel);
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                    scheme: Scheme.AuthenticationCookie());
            return RedirectToAction(nameof(SignIn));
        }

        public IActionResult Forgot()
        {
            if (this.GetIdUser() != 0)
                return RedirectToAction("Index", "Home");
            return View();
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
    }
}
