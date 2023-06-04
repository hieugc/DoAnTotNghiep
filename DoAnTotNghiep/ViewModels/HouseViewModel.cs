using DoAnTotNghiep.Entity;
using DoAnTotNghiep.Enum;
using DoAnTotNghiep.Modules;
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
        public string? CityName { get; set; } = string.Empty;
        public string? DistrictName { get; set; } = string.Empty;
        public string? WardName { get; set; } = string.Empty;
        public double Lat { get; set; } = 0;
        public double Lng { get; set; } = 0;
        public int IdCity { get; set; } = 0;
        public int IdDistrict { get; set; } = 0;
        public int IdWard { get; set; } = 0;
        public int? Bed { get; set; } = 0;

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
        public DetailHouseViewModel() { }
        public DetailHouseViewModel(House house, byte[] salt, User? user = null, string? host = null)
        {
            List<int> rules = new List<int>();
            if (house.RulesInHouses != null)
            {
                rules.AddRange(house.RulesInHouses.Where(m => m.Status == true).Select(m => m.IdRules));
            }
            List<int> utilities = new List<int>();
            if (house.UtilitiesInHouses != null)
            {
                utilities.AddRange(house.UtilitiesInHouses.Where(m => m.Status == true).Select(m => m.IdUtilities));
            }

            this.Id = house.Id;
            this.Name = house.Name;
            this.Option = house.Type;
            this.Description = house.Description;//string
            this.People = house.People;//int
            this.BathRoom = house.BathRoom;//int
            this.BedRoom = house.BedRoom;//int
            this.Square = (int)house.Area; // tạm thời int   check này lại
            this.Location = house.StreetAddress; //địa chỉ full
            this.Lat = house.Lat;
            this.Lng = house.Lng;
            this.IdCity = house.IdCity == null ? 0 : house.IdCity.Value; //id bắt buộc => tạo yêu cầu xoay vòng
            this.IdDistrict = house.IdDistrict == null ? 0 : house.IdDistrict.Value;//idDistrict 
            this.IdWard = house.IdWard == null ? 0 : house.IdWard.Value;// có thể không có
            this.CityName = house.Citys == null ? string.Empty : house.Citys.Name;
            this.DistrictName = house.Districts == null ? string.Empty : house.Districts.Name;
            this.WardName = house.Wards == null ? string.Empty : house.Wards.Name;
            this.Price = house.Price;//int
            this.Utilities = utilities;//list<int>
            this.Rules = rules;//list<int>
            this.Images = new List<ImageBase?>();
            this.Rating = house.Rating;//double :)) quên
            this.Bed = house.Bed;
            this.Status = house.Status;//int
            if (house.Requests != null)
            {
                this.Request = house.Requests.Where(m => m.Status == (int) StatusRequest.WAIT_FOR_SWAP).Count();
                DateTime now = DateTime.Now;
                List<RangeDate> rangeDates = house.Requests.Where(r => (r.Status == (int)StatusRequest.ACCEPT
                                                                        || r.Status == (int)StatusRequest.CHECK_IN
                                                                    )
                                                                    && ((DateTime.Compare(r.StartDate, now) <= 0 && DateTime.Compare(now, r.EndDate) <= 0)
                                                                        || DateTime.Compare(r.StartDate, now) >= 0)
                                                                )
                                                        .Select(m => new RangeDate()
                                                        {
                                                            StartDate = m.StartDate,
                                                            EndDate = m.EndDate
                                                        })
                                                        .ToList();
                this.InValidRangeDates.AddRange(rangeDates);
            }
            else
            {
                this.Request = 0;
            }
            this.UserAccess = Crypto.EncodeKey(house.IdUser.ToString(), salt);
            this.NumberRating = house.FeedBacks == null ? 0 : house.FeedBacks.Count();
            if (user != null && !string.IsNullOrEmpty(host))
            {
                this.User = new UserInfo(user, salt, host);
            }
        }

        public int Status { get; set; } = 0;
        public double Rating { get; set; } = 0;
        public int NumberRating { get; set; } = 0;
        public int Request { get; set; } = 0;
        public string UserAccess { get; set; } = string.Empty;
        public UserInfo? User { get; set; }
        public List<RangeDate> InValidRangeDates { get; set; } = new List<RangeDate>();
        //list Url
        public List<DetailRatingWithUser> Ratings { get; set; } = new List<DetailRatingWithUser>();
    }
    public class PackageDetailHouse: DetailHouseViewModel
    {
        public PackageDetailHouse(House house, byte[] salt, 
            User? user = null, string? host = null) 
            : base(house, salt, user, host) 
        {
        }

        public List<Utilities> AllUtilities { get; set; } = new List<Utilities>();
        public List<Rules> AllRules { get; set; } = new List<Rules>();
    }
    public class MobileCreateHouseViewModel
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
        public string Location { get; set; } = string.Empty; //địa chỉ
        public string? CityName { get; set; } = string.Empty;
        public string? DistrictName { get; set; } = string.Empty;
        public string? WardName { get; set; } = string.Empty;
        public double Lat { get; set; } = 0;// :))
        public double Lng { get; set; } = 0;//
        public int? Bed { get; set; } = 0;
        //public string? CityName { get; set; } = string.Empty;
        //public string? DistrictName { get; set; } = string.Empty;
        public int IdCity { get; set; } = 0;//sửa lại
        public int IdDistrict { get; set; } = 0; //sửa lại
        public int IdWard { get; set; } = 0; //sửa lại

        [Required(ErrorMessage = "Hãy điền giá căn nhà")]
        public int Price { get; set; } = 0;
        public List<int> Utilities { get; set; } = new List<int>();
        public List<int> Rules { get; set; } = new List<int>();

        public IFormFileCollection? Files { get; set; } //
    }
    public class MobileEditHouseViewModel : MobileCreateHouseViewModel
    {
        [Required(ErrorMessage = "Không tìm thấy mã định danh. Hãy tải lại trang!")] //id lấy từ lúc get list
        public int Id { get; set; }
        public List<int> IdRemove { get; set; } = new List<int>(); //danh sách id hình
    }
    public class CreateHouse
    {
        public CreateHouse(MobileCreateHouseViewModel? mobileCreateHouseViewModel, CreateHouseViewModel? createHouseViewModel)
        {
            if(mobileCreateHouseViewModel != null)
            {
                this.People = mobileCreateHouseViewModel.People;
                this.Price = mobileCreateHouseViewModel.Price;
                this.Name = mobileCreateHouseViewModel.Name;
                this.Option = mobileCreateHouseViewModel.Option;
                this.Description = mobileCreateHouseViewModel.Description;
                this.People = mobileCreateHouseViewModel.People;
                this.BathRoom = mobileCreateHouseViewModel.BathRoom;
                this.BedRoom = mobileCreateHouseViewModel.BedRoom;
                this.Lat = mobileCreateHouseViewModel.Lat;
                this.Lng = mobileCreateHouseViewModel.Lng;
                this.Square = mobileCreateHouseViewModel.Square;
                this.Location = mobileCreateHouseViewModel.Location;
                this.IdCity = mobileCreateHouseViewModel.IdCity;
                this.IdWard = mobileCreateHouseViewModel.IdWard;
                this.IdDistrict = mobileCreateHouseViewModel.IdDistrict;
                this.Utilities = mobileCreateHouseViewModel.Utilities;
                this.Rules = mobileCreateHouseViewModel.Rules;
                this.Files = mobileCreateHouseViewModel.Files;
                this.Bed = mobileCreateHouseViewModel?.Bed;
                this.CityName = createHouseViewModel.CityName;
                this.DistrictName = createHouseViewModel.DistrictName;
                this.WardName = createHouseViewModel.WardName;
            }
            else if(createHouseViewModel != null)
            {
                this.People = createHouseViewModel.People;
                this.Price = createHouseViewModel.Price;
                this.Name = createHouseViewModel.Name;
                this.Option = createHouseViewModel.Option;
                this.Description = createHouseViewModel.Description;
                this.People = createHouseViewModel.People;
                this.BathRoom = createHouseViewModel.BathRoom;
                this.BedRoom = createHouseViewModel.BedRoom;
                this.Lat = createHouseViewModel.Lat;
                this.Lng = createHouseViewModel.Lng;
                this.Square = createHouseViewModel.Square;
                this.Location = createHouseViewModel.Location;
                this.IdCity = createHouseViewModel.IdCity;
                this.IdWard = createHouseViewModel.IdWard;
                this.IdDistrict = createHouseViewModel.IdDistrict;
                this.Utilities = createHouseViewModel.Utilities;
                this.Rules = createHouseViewModel.Rules;
                this.Files = null;
                this.Images = createHouseViewModel.Images;
                this.Bed = createHouseViewModel.Bed;
                this.CityName = createHouseViewModel.CityName;
                this.DistrictName = createHouseViewModel.DistrictName;
                this.WardName = createHouseViewModel.WardName;

            }
        }
        public string Name { get; set; } = string.Empty;
        public int Option { get; set; } = 0;
        public string Description { get; set; } = string.Empty;
        public int People { get; set; } = 0;
        public int BedRoom { get; set; } = 0;
        public int BathRoom { get; set; } = 0;
        public int Square { get; set; } = 0;
        public string Location { get; set; } = string.Empty;
        public double Lat { get; set; } = 0;
        public double Lng { get; set; } = 0;
        public string? CityName { get; set; } = string.Empty;
        public string? DistrictName { get; set; } = string.Empty;
        public string? WardName { get; set; } = string.Empty;
        public int IdCity { get; set; } = 0;
        public int? IdDistrict { get; set; } = 0;
        public int? IdWard { get; set; } = 0;
        public int Price { get; set; } = 0;
        public int? Bed { get; set; } = 0;
        public List<int> Utilities { get; set; } = new List<int>();
        public List<int> Rules { get; set; } = new List<int>();
        public List<ImageBase?> Images { get; set; } = new List<ImageBase?>();
        public IFormFileCollection? Files { get; set; }
    }
    public class EditHouse: CreateHouse
    {
        public EditHouse(MobileEditHouseViewModel? mobile, EditHouseViewModel? web): base(mobile, web)
        {
            if(mobile == null && web != null)
            {
                Id = web.Id;
            }
            else if (mobile != null && web == null)
            {
                Id = mobile.Id;
                IdRemove = mobile.IdRemove;
            }
        }
        public int Id { get; set; }
        public List<int> IdRemove { get; set; } = new List<int>();
        public int Status { get; set; } = (int)StatusHouse.VALID;
    }
    public class ImageBase
    {
        public ImageBase(Entity.File file, string host)
        {
            this.Name = file.FileName;
            this.Data = host + "/" + file.PathFolder + "/" + file.FileName;
            this.Id = file.Id;
        }
        public ImageBase() { }

        public string Name { get; set; } = string.Empty;
        public int? Id { get; set; } = 0; //Id FILE  => cập hình 
        public string Data { get; set; } = string.Empty; //image url :)) t còn base64
    }
    public class ListDetailHouses
    {
        public List<DetailHouseViewModel> Houses { get; set; } = new List<DetailHouseViewModel>();
        public Pagination Pagination { get; set; } = new Pagination();
    }
    public class RangeDate
    {
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; } = DateTime.Now;
    }
    public class Pagination
    {
        public Pagination()
        {
            Page = 1;
            Limit = 10;
        }
        public Pagination(int page, int limit)
        {
            Page = page;
            Limit = limit;
        }

        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
        public int Total { get; set; } = 0;
    }
    public class HouseSelector
    {
        public HouseSelector(House house)
        {
            this.Id = house.Id;
            this.Name = house.Name;
        }
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class HistoryViewModel
    {
        public HistoryViewModel(List<HouseSelector> house, double lat, double lng)
        {
            this.ListHouse = house;
            this.Lat = lat;
            this.Lng = lng;
        }
        public List<HouseSelector> ListHouse { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
    }
}
