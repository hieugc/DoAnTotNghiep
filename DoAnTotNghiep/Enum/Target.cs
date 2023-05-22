namespace DoAnTotNghiep.Enum
{
    public class TargetSignalR
    {
        public static string Receive() => "ReceiveMessages";
        public static string Connect() => "ConnectTag";


        //user tự group bản thân = userAccess;
        public static string Request() => "Request";
        public static string Notification() => "Notification";
        public static string CircleRequest() => "CircleRequest";
        public static string AdminReport() => "AdminReport";
        public static string Payment() => "Payment";
    }


    public static class TargetFunction
    {
        public const string ExecuteCheckIn = "ExecuteCheckIn";
        public const string ExecuteCheckOut = "ExecuteCheckOut";
        public const string ExecuteCheckTransaction = "ExecuteCheckTransaction";
        public const string ExecuteCreateWaiting = "ExecuteCreateWaiting";
        public const string ExecuteCheckInCircleRequest = "ExecuteCheckInCircleRequest";
        public const string ExecuteCheckOutCircleRequest = "ExecuteCheckOutCircleRequest";
    }
}
