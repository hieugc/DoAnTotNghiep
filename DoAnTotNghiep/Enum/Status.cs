namespace DoAnTotNghiep.Enum
{
    enum StatusRequest
    {
        WAIT_FOR_SWAP,
        ACCEPT,
        REJECT,
        WAIT_FOR_RATE,
        ENDED
    }

    enum StatusMessage
    {
        SEEN,
        UNSEEN,
        REMOVED
    }

    enum StatusHouse
    {
        VALID,
        PENDING,
        SWAPPING
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
                case (int)StatusRequest.WAIT_FOR_RATE:
                    return "Chờ đánh giá";
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
                (int) StatusRequest.WAIT_FOR_RATE,
                (int) StatusRequest.ENDED
            };
        }
    }
}
