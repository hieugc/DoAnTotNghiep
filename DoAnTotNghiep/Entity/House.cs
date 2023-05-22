using DoAnTotNghiep.Data;
using DoAnTotNghiep.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Composition;

namespace DoAnTotNghiep.Entity
{
    [Table("House")]
    public class House
    {
        public House CreateHouse(CreateHouse data, int IdUser, int Status)
        {
            return new House()
            {
                Name = data.Name,
                Type = data.Option,
                Description = data.Description,
                People = data.People,
                BedRoom = data.BedRoom,
                BathRoom = data.BathRoom,
                Area = data.Square,
                Lat = data.Lat,
                Lng = data.Lng,
                Price = data.Price,
                IdCity = (data.IdCity == 0 ? 1 : data.IdCity),
                IdDistrict = (data.IdDistrict == 0 ? null : data.IdDistrict),
                IdWard = (data.IdWard == 0 ? null : data.IdWard),
                Rating = 0,
                IdUser = IdUser,
                Status = Status,
                StreetAddress = data.Location,
                Bed = data.Bed
            };
        }
        public void EditHouse(EditHouse data)
        {
            this.Name = data.Name;
            this.Type = data.Option;
            this.Description = data.Description;
            this.People = data.People;
            this.BedRoom = data.BedRoom;
            this.BathRoom = data.BathRoom;
            this.Area = data.Square;
            this.Lat = data.Lat;
            this.Lng = data.Lng;
            this.StreetAddress = data.Location;
            this.IdCity = data.IdCity;
            this.IdDistrict = (data.IdDistrict == 0 ? null : data.IdDistrict);
            this.IdWard = (data.IdWard == 0 ? null : data.IdWard);
            this.Price = data.Price;
            this.Status = data.Status;
            this.Bed = data.Bed;
        }
        public void IncludeAll(DoAnTotNghiepContext context)
        {
            if(this.Citys == null && !context.Entry(this).Reference(m => m.Citys).IsLoaded)
                context.Entry(this).Reference(m => m.Citys).Load();
            if (this.Districts == null && !context.Entry(this).Reference(m => m.Districts).IsLoaded)
                context.Entry(this).Reference(m => m.Districts).Load();
            if (this.Wards == null && !context.Entry(this).Reference(m => m.Wards).IsLoaded)
                context.Entry(this).Reference(m => m.Wards).Load();
            if (this.Requests == null && !context.Entry(this).Collection(m => m.Requests).IsLoaded)
                context.Entry(this).Collection(m => m.Requests).Load();
            if (this.FeedBacks == null && !context.Entry(this).Collection(m => m.FeedBacks).IsLoaded)
                context.Entry(this).Collection(m => m.FeedBacks).Load();
            if (this.FileOfHouses == null && !context.Entry(this).Collection(m => m.FileOfHouses).IsLoaded)
                context.Entry(this).Collection(m => m.FileOfHouses).Load();
            if (this.RulesInHouses == null && !context.Entry(this).Collection(m => m.RulesInHouses).IsLoaded)
                context.Entry(this).Collection(m => m.RulesInHouses).Load();
            if (this.UtilitiesInHouses == null && !context.Entry(this).Collection(m => m.UtilitiesInHouses).IsLoaded)
                context.Entry(this).Collection(m => m.UtilitiesInHouses).Load();
            if (this.Users == null && !context.Entry(this).Reference(m => m.Users).IsLoaded)
                context.Entry(this).Reference(m => m.Users).Load();
        }


        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Hãy điền tên căn nhà")]
        [MaxLength(100, ErrorMessage = "Tên nhà tối đa 100 ký tự")]
        [Column("name")]
        public string Name { get; set; } = string.Empty;
        [Column("status")]
        public int Status { get; set; } = 0;
        [Column("type")]
        public int Type { get; set; } = 0;

        [Required(ErrorMessage = "Hãy điền giá căn nhà")]
        [Column("price")]
        public int Price { get; set; } = 0;

        [Required(ErrorMessage = "Hãy điền số người có thể ở")]
        [Column("people")]
        public int People { get; set; } = 0;

        [Required(ErrorMessage = "Hãy điền số phòng ngủ")]
        [Column("bedroom")]
        public int BedRoom { get; set; } = 0;

        [Required(ErrorMessage = "Hãy điền số phòng tắm")]
        [Column("bathroom")]
        public int BathRoom { get; set; } = 0;

        [Required(ErrorMessage = "Hãy điền mô tả căn nhà")]
        [Column("description")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hãy điền diện tích căn nhà")]
        [Column("area")]
        public double Area { get; set; } = 0;

        [Column("number_of_rating")]
        public int NumberOfRating { get; set; } = 0;

        [Column("rating")]
        public double Rating { get; set; } = 0;

        [Column("bed")]
        public int? Bed { get; set; } = 0;

        [Column("lat")]
        public double Lat { get; set; } = 0;
        [Column("lng")]
        public double Lng { get; set; } = 0;

        [Column("id_user")]
        public virtual int IdUser { get; set; }

        [ForeignKey(nameof(IdUser))]
        public virtual User? Users { get; set; }

        [MaxLength(200, ErrorMessage = "Số nhà, tên đường tối đa 200 ký tự")]
        [Column("street_address")]
        public string StreetAddress { get; set; } = string.Empty;

        [Column("id_city")]
        public virtual int? IdCity { get; set; }
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

        public virtual ICollection<RulesInHouse>? RulesInHouses { get; set; }
        public virtual ICollection<UtilitiesInHouse>? UtilitiesInHouses { get; set; }
        public virtual ICollection<FileOfHouse>? FileOfHouses { get; set; }
        [InverseProperty("Houses")]
        public virtual ICollection<Request>? Requests { get; set; }
        public virtual ICollection<UserReport>? UserReports { get; set; }
        public virtual ICollection<FeedBack>? FeedBacks { get; set; }
        public virtual ICollection<FeedBackOfCircle>? FeedBackOfCircles { get; set; }
    }
}