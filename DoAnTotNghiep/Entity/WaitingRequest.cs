﻿using DoAnTotNghiep.Modules;
using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.Entity
{
    [Table("Waiting_request")]
    public class WaitingRequest
    {
        public WaitingRequest CreateModel(CreateWaitingRequest model, int IdHouse)
        {
            return new WaitingRequest()
            {
                StartDate = model.DateStart,
                EndDate = model.DateEnd,
                IdCity = model.IdCity,
                IdUser = model.IdUser,
                IdHouse = IdHouse,
                Status = 0,
                Point = 0,
                IdDistrict = null,
                IdWard = null
            };
        }


        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("start_date")]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [Column("end_date")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [Column("status")]
        public int Status { get; set; } = 0;

        [Column("point")]
        public int Point { get; set; } = 0;

        [Required]
        [Column("id_user")]
        public virtual int IdUser { get; set; }

        [ForeignKey(nameof(IdUser))]
        public virtual User? Users { get; set; }

        [Required]
        [Column("id_city")]
        public virtual int IdCity { get; set; }
        [ForeignKey(nameof(IdCity))]
        public virtual City? Citys { get; set; }

        [Column("id_district")]
        public virtual int? IdDistrict { get; set; }
        [ForeignKey(nameof(IdDistrict))]
        public virtual District? Districts { get; set; }
        [Column("id_ward")]
        public virtual int? IdWard { get; set; }
        [ForeignKey(nameof(IdWard))]
        public virtual Ward? Wards { get; set; }

        [Required]
        [Column("id_house")]
        public virtual int IdHouse { get; set; }

        [ForeignKey(nameof(IdHouse))]
        public virtual House? Houses { get; set; }

        public ICollection<RequestInCircleExchangeHouse>? Requests { get; set; }
    }
}
