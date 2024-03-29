﻿using DoAnTotNghiep.Entity;
using DoAnTotNghiep.Enum;
using DoAnTotNghiep.Modules;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.ViewModels
{
    public class RequestViewModel
    {
        public RequestViewModel(Request request) {
            this.IdHouse = request.IdHouse;
            this.IdSwapHouse = request.IdSwapHouse;
            this.StartDate = request.StartDate;
            this.EndDate = request.EndDate;
            this.Price = request.Point;
            this.Type = request.Type;
        }
        public RequestViewModel() { }

        [Required(ErrorMessage = "Hãy chọn nhà trao đổi")]
        public int IdHouse { get; set; } = 0;
        public int Type { get; set; } = 0;
        public int Price { get; set; } = 0;
        public int? IdSwapHouse { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
    }
    public class EditRequestViewModel: RequestViewModel
    {
        public EditRequestViewModel(Request request): base(request)
        {
            this.Id = request.Id;
        }
        public EditRequestViewModel() { }
        public int Id { get; set; }
    }
    public class DetailRequestViewModel: EditRequestViewModel
    {
        public DetailRequestViewModel(Request request, bool isOwner, byte[] salt, string host) : base(request)
        {
            if(request.Users != null)
            {
                this.User = new UserMessageViewModel(request.Users, salt, host);
            }
            this.Status = request.Status;
            this.isOwner = isOwner;
        }
        public DetailRequestViewModel() { }
        public UserMessageViewModel User { get; set; } = new UserMessageViewModel();
        public int Status { get; set; }
        public bool isOwner { get; set; } = false;
    }
    public class UpdateStatusViewModel
    {
        public int Id { get; set; } = 0;
        public int Status { get; set; } = 0;
    }
    public class DetailRequest
    {

        public DetailRequest() { }
        public DetailRequest(DetailHouseViewModel house, DetailRequestViewModel request, DetailHouseViewModel? swapHouse, DetailRatingViewModel? userRating, DetailRatingViewModel? myRating)
        {
            House = house;
            Request = request;
            SwapHouse = swapHouse;
            UserRating = userRating;
            MyRating = myRating;
        }

        public DetailHouseViewModel House { get; set; } = new DetailHouseViewModel();
        public DetailRequestViewModel Request { get; set; } = new DetailRequestViewModel();
        public DetailHouseViewModel? SwapHouse { get; set; } = null;
        public DetailRatingViewModel? UserRating { get; set; } = null;
        public DetailRatingViewModel? MyRating { get; set; } = null;
    }
    public class NotifyRequest
    {
        public DetailRequestViewModel Request { get; set; } = new DetailRequestViewModel();
        public DetailHouseViewModel? SwapHouse { get; set; } = null;
    }
    public class ModelRequestForm
    {
        public List<DetailHouseViewModel> UserHouses { get; set; } = new List<DetailHouseViewModel>();
        public List<DetailHouseViewModel> MyHouses { get; set; } = new List<DetailHouseViewModel>();
    }
}
