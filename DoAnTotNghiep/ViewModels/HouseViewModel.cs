using DoAnTotNghiep.Entity;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Printing;

namespace DoAnTotNghiep.ViewModels
{
    public class CreateHouseViewModel
    {
        [Required(ErrorMessage = "Hãy điền tên căn nhà")]
        [MaxLength(100, ErrorMessage = "Tên nhà tối đa 100 ký tự")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hãy chọn loại nhà")]
        public int Option { get; set; } = 0;
        [Required(ErrorMessage = "Hãy điền mô tả căn nhà")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hãy điền số người có thể ở")]
        public int People { get; set; } = 0;
        [Required(ErrorMessage = "Hãy điền số phòng ngủ")]
        public int BedRoom { get; set; } = 0;
        [Required(ErrorMessage = "Hãy điền số phòng tắm")]
        public int BathRoom { get; set; } = 0;
        [Required(ErrorMessage = "Hãy điền diện tích căn nhà")]
        public int Square { get; set; } = 0;
        [Required(ErrorMessage = "Hãy điền địa chỉ nhà")]
        public string Location { get; set; } = string.Empty;

        public double Lat { get; set; } = 0;
        public double Lng { get; set; } = 0;
        public int IdCity { get; set; } = 0;
        public int IdDistrict { get; set; } = 0;
        public int IdWard { get; set; } = 0;

        [Required(ErrorMessage = "Hãy điền giá căn nhà")]
        public int Price { get; set; } = 0;
        public List<int> Utilities { get; set; } = new List<int>();
        public List<int> Rules { get; set; } = new List<int>();
        [Required(ErrorMessage = "Hãy thêm hình ảnh nhà của bạn")]
        public List<ImageBase?> Images { get; set; } = new List<ImageBase?>();
    }

    public class EditHouseViewModel: CreateHouseViewModel
    {
        [Required(ErrorMessage = "Không tìm thấy mã định danh. Hãy tải lại trang!")]
        public int Id { get; set; }
    }

    public class DetailHouseViewModel: EditHouseViewModel
    {
        public int Status { get; set; } = 0;
        public double Rating { get; set; } = 0;
        public int Request { get; set; } = 0; 

        public static DetailHouseViewModel GetByHouse(House house)
        {
            List<int> rules = new List<int>();
            if (house.RulesInHouses != null)
            {
                foreach (var rule in house.RulesInHouses)
                {
                    if (rule.Status == true) rules.Add(rule.IdRules);
                }
            }
            List<int> utilities = new List<int>();

            if (house.UtilitiesInHouses != null)
            {
                foreach (var utility in house.UtilitiesInHouses)
                {
                    if (utility.Status == true) utilities.Add(utility.IdUtilities);
                }
            }
            return new DetailHouseViewModel()
            {
                Id = house.Id,
                Name = house.Name,
                Option = house.Type,
                Description = house.Description,
                People = house.People,
                BathRoom = house.BathRoom,
                BedRoom = house.BedRoom,
                Square = (int)house.Area,
                Location = house.StreetAddress,
                Lat = house.Lat,
                Lng = house.Lng,
                IdCity = house.IdCity == null ? 0 : house.IdCity.Value,
                IdDistrict = house.IdDistrict == null ? 0 : house.IdDistrict.Value,
                IdWard = house.IdWard == null ? 0 : house.IdWard.Value,
                Price = house.Price,
                Utilities = utilities,
                Rules = rules,
                Images = new List<ImageBase?>(),
                Status = house.Status,
                Rating = house.Rating,
                Request = house.Requests == null ? 0: house.Requests.Count()
            };
        }
    }


    public class ImageBase
    {
        public string Name { get; set; } = string.Empty;
        public string Data { get; set; } = string.Empty;
        public string? Folder { get; set; } = string.Empty;
        public int? Id { get; set; } = 0;
    }
}