using DoAnTotNghiep.Entity;
using DoAnTotNghiep.Enum;
using DoAnTotNghiep.Modules;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.ViewModels
{
    public class CircleRequestViewModel
    {
        public CircleRequestViewModel(CircleRequestDetail prevNode, 
                                    CircleRequestDetail myNode, 
                                    CircleRequestDetail nextNode, 
                                    CircleExchangeHouse circleExchangeHouse,
                                    int IdUser)
        {
            PrevNode = prevNode;
            MyNode = myNode;
            NextNode = nextNode;
            StartDate = circleExchangeHouse.StartDate;
            EndDate = circleExchangeHouse.EndDate;
            Status = circleExchangeHouse.Status;
            Id = circleExchangeHouse.Id;
            if(circleExchangeHouse.FeedBacks != null)
            {
                this.Rating = circleExchangeHouse.FeedBacks.Select(m => new DetailRatingViewModel(m, IdUser)).ToList();
            }
        }

        public CircleRequestDetail PrevNode { get; set; }
        public CircleRequestDetail MyNode { get; set; }
        public CircleRequestDetail NextNode { get; set; }
        public List<DetailRatingViewModel> Rating { get; set; } = new List<DetailRatingViewModel>();
        public DateTime StartDate { get; set; } 
        public DateTime EndDate { get; set; }
        public int Status { get; set; }
        public int Id { get; set; }
    }

    public class CircleRequestDetail
    {
        public CircleRequestDetail(DetailHouseViewModel house, 
                                    WaitingRequest request, 
                                    UserInfo userInfo,
                                    ImageBase imageBase)
        {
            House = house;
            this.IdRequest = request.Id;
            this.Status = request.Status;
            User = userInfo;
            ImageHouse = imageBase;
        }

        public DetailHouseViewModel House { get; set; }
        public int IdRequest { get; set; }
        public UserInfo User { get; set; }
        public ImageBase ImageHouse { get; set; }
        public int Status { get; set; }
    }
    public class UpdateStatusCircleViewModel
    {
        public int Id { get; set; } = 0;
        public int Status { get; set; } = 0;
        public int IdCircle { get; set; } = 0;
    }
}
