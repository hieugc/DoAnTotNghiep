namespace DoAnTotNghiep.Enum
{
    enum StatusRequest
    {
        WAIT_FOR_SWAP,
        ACCEPT,
        REJECT,
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
        SWAPPING
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
                (int) StatusRequest.CHECK_OUT
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
                || Status == (int)StatusRequest.CHECK_IN
                || Status == (int)StatusRequest.CHECK_OUT;
        }
    }
}
