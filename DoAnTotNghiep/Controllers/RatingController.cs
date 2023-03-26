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
using Microsoft.Extensions.Hosting;
using static System.ComponentModel.BackgroundWorker;
using System.ComponentModel;
using System.Xml;

namespace DoAnTotNghiep.Controllers
{
    [Authorize(Roles = Role.Member)]
    public class RatingController: BaseController
    {
        private readonly DoAnTotNghiepContext _context;
        private readonly IConfiguration _configuration;

        public RatingController(DoAnTotNghiepContext context, IConfiguration configuration, IHostEnvironment environment) : base(environment)
        {
            _context = context;
            _configuration = configuration;
        }
        //Create

        //Update
        //Detail
    }
}
