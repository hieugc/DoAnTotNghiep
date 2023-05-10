﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.Entity
{
    [Table("Circle_exchange_house")]
    public class CircleExchangeHouse
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("status")]
        public int Status { get; set; } = 0;


        [Column("start_date")]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [Column("end_date")]
        public DateTime EndDate { get; set; } = DateTime.Now;

        public ICollection<RequestInCircleExchangeHouse>? RequestInCircles { get; set; }
        public ICollection<CircleExchangeHouseOfUser>? UserInCircles { get; set; }
        public ICollection<FeedBackOfCircle>? FeedBacks { get; set; }
    }
}
