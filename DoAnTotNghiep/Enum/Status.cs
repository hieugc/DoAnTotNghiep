namespace DoAnTotNghiep.Enum
{
    enum StatusRequest
    {
        WAIT_FOR_SWAP,
        ACCEPT,
        REJECT,
        CHECK_IN,
        CHECK_OUT,
        ENDED,
        DISABLE
    }
    /// <summary>
    /// Đợi accept => checkin => checkout => comment => end
    /// </summary>
    /// 
    enum StatusWaitingRequest
    {
        INIT,//mới khởi tạo
        IN_CIRCLE,//trong vòng + chưa xác nhận
        DISABLE,//không muốn vào vòng => không gợi ý nữa
        ACCEPT,//đã xác nhận
        CHECK_IN,
        CHECK_OUT,
        ENDED
    }

    enum StatusMessage
    {
        SEEN,
        UNSEEN,
        REMOVED
    }
    enum StatusTransaction
    {
        PENDING,
        VALID,
        REFUND,
        USED
    }
    enum StatusHouse
    {
        VALID,
        PENDING,
        SWAPPING,
        DISABLE
    }
    enum StatusAdminReport
    {
        WAITING,
        RESPONSED
    }
    public class StatusRequestStr
    {
        public static string getStatus(int status)
        {
            switch(status)
            {
                case (int)StatusRequest.WAIT_FOR_SWAP:
                    return "Chờ xác nhận";
                case (int)StatusRequest.REJECT:
                    return "Đã từ chối";
                case (int)StatusRequest.ACCEPT:
                    return "Đã chấp nhận";
                case (int)StatusRequest.CHECK_IN:
                    return "Check-In";
                case (int)StatusRequest.CHECK_OUT:
                    return "Chờ đánh giá";
            }

            return "Kết thúc";
        }

        public static string ConvertAction(int status)
        {
            switch (status)
            {
                case (int)StatusRequest.WAIT_FOR_SWAP:
                    return "Chờ xác nhận";
                case (int)StatusRequest.REJECT:
                    return "Đã từ chối";
                case (int)StatusRequest.ACCEPT:
                    return "Check-In";
                case (int)StatusRequest.CHECK_IN:
                    return "Check-Out";
                case (int)StatusRequest.CHECK_OUT:
                    return "Đánh giá";
            }

            return "Kết thúc";
        }

        public static List<int> getStatus()
        {
            return new List<int>()
            {
                (int) StatusRequest.WAIT_FOR_SWAP,
                (int) StatusRequest.REJECT,
                (int) StatusRequest.ACCEPT,
                (int) StatusRequest.CHECK_IN,
                (int) StatusRequest.CHECK_OUT,
                (int) StatusRequest.ENDED
            };
        }

        public static List<int> getStatusTransaction()
        {
            return new List<int>()
            {
                (int) StatusRequest.ACCEPT,
                (int) StatusRequest.CHECK_IN,
                (int) StatusRequest.CHECK_OUT,
                (int) StatusRequest.ENDED
            };
        }

        /// <summary>
        /// Unvalid <=> request is accepted or checked In or wait payment
        /// </summary>
        /// <param name="Status"></param>
        /// <returns></returns>
        public static bool IsUnValidHouse(int Status)
        {
            return Status == (int)StatusRequest.ACCEPT
                || Status == (int)StatusRequest.CHECK_IN;
        }
    }

    public class StatusCircleRequestStr
    {
        public static string getStatus(int status)
        {
            switch (status)
            {
                case (int)StatusWaitingRequest.INIT:
                    return "Gợi ý mới";
                case (int)StatusWaitingRequest.ACCEPT:
                    return "Đã chấp nhận";
                case (int)StatusWaitingRequest.CHECK_IN:
                    return "Check-In";
                case (int)StatusWaitingRequest.CHECK_OUT:
                    return "Chờ đánh giá";
            }

            return "Kết thúc";
        }
        public static List<int> getStatus()
        {
            return new List<int>()
            {
                (int) StatusWaitingRequest.INIT,
                (int) StatusWaitingRequest.ACCEPT,
                (int) StatusWaitingRequest.CHECK_IN,
                (int) StatusWaitingRequest.CHECK_OUT,
                (int) StatusWaitingRequest.ENDED
            };
        }
    }
}
