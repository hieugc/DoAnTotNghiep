namespace DoAnTotNghiep.Enum
{
    public class Role
    {
        public static bool Member() => false;
        public static int MemberInt() => 1;
        public static string MemberString() => "MEMBER";

        public static int AdminInt() => 2;
        public static bool Admin() => true;
        public static string AdminString() => "ADMIN";

        public static int NoneInt() => 0;
        public static string NoneString() => "NONE";
        public static string UnAuthorize() => "UNAUTHORIZE";
        public static string UpdateInfo() => "UPDATEINFO";
    }

    enum RoleType
    {
        NONE,
        MEMBER,
        ADMIN
    }

}
