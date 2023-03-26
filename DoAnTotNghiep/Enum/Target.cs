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
    }
}
