using DoAnTotNghiep.Entity;

namespace DoAnTotNghiep.ViewModels
{
    public class AuthHouseViewModel
    {
        public List<House> Houses { get; set; } = new List<House>();
        public OptionHouseViewModel OptionHouses { get; set; } = new OptionHouseViewModel();
    }
    public class OptionHouseViewModel
    {
        public List<Utilities> Utilities { get; set; } = new List<Utilities>();
        public List<Rules> Rules { get; set; } = new List<Rules>();
    }

    public class HouseViewModel
    {

    }
}