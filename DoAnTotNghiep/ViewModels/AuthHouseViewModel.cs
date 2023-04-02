using DoAnTotNghiep.Entity;

namespace DoAnTotNghiep.ViewModels
{
    public class AuthHouseViewModel
    {
        public ListDetailHouses Houses { get; set; } = new ListDetailHouses();
        public OptionHouseViewModel OptionHouses { get; set; } = new OptionHouseViewModel();
    }
    public class OptionHouseViewModel
    {
        public List<Utilities> Utilities { get; set; } = new List<Utilities>();
        public List<Rules> Rules { get; set; } = new List<Rules>();
    }
}