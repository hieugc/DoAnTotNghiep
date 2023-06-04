namespace DoAnTotNghiep.Enum
{
    public static class NotificationType
    {
        public const int REQUEST = 0;//
        public const int RATING = 1; //
        public const int ADMIN_REPORT = 2;//
        public const int MESSAGE = 3;//
        public const int CIRCLE_SWAP = 4;//
        public const int DEMO = 5;
        public const int PAYMENT = 6;//
        public const int CIRCLE_RATING = 7;//


        public const string DemoTitle = "DEMO";
        public const string RequestTitle = "Yêu cầu trao đổi";
        public const string RatingTitle = "Đánh giá mới";
        public const string AdminReportTitle = "Thư từ quản trị viên";
        public const string MessageTitle = "Thông báo tin nhắn";
        public const string CircleSwapTitle = "Gợi ý trao đổi";
        public const string PaymentTitle = "Nạp tích lũy";
    }

    public static class FunctionGetNotification
    {
        public static string GetFunction(int type, int idType, int id)
        {
            if(type == NotificationType.REQUEST || type == NotificationType.RATING)
            {
                return "requestView(" + idType + ", " + id + ")";
            }
            else if(type == NotificationType.CIRCLE_RATING || type == NotificationType.CIRCLE_SWAP)
            {
                return "circleRequestView(" + idType + ", " + id + ")";
            }
            return "paymentView(" + idType + ", " + id + ")";
        }
    }
}
