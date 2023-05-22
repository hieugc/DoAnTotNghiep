namespace DoAnTotNghiep.Enum
{
    public class Subject
    {
        public static string SendOTP() => "Xác thực đăng ký người dùng";
        public static string SendCheckIn() => "Thông báo Check In phòng";
        public static string SendCheckOut() => "Thông báo Check Out phòng";
        public static string SendRequestDetail() => "Chi tiết yêu cầu trao đổi";
        public static string SendReject() => "Yêu cầu bị từ chối";
    }
}
