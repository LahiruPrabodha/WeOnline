
namespace clientExample
{
    using System.Collections.Generic;

    public static class Packets
    {
        public static Packet UpdateRole(string userid, Sets.UserRole newRole)
        {
            var package = new List<string>
                              {
                                 userid,
                                 newRole.ToString()
                              };
            return new Packet("UPDATE ROLE", package.Package());
        }
        public static Packet UpdateRole(string userid, string newRole)
        {
            var package = new List<string>
                              {
                                 userid,
                                 newRole
                              };
            return new Packet("UPDATE ROLE", package.Package());
        }
        public static Packet UpdatePassword(string newpass)
        {
            return new Packet("UPDATE PASSWORD", newpass);
        }
        public static Packet CreateRequest(string email, string password, string nickname)
        {
            var package = new List<string>
                              {
                                  email,
                                  password,
                                  nickname
                              };
            return new Packet("CREATE REQUEST", package.Package());
        }
        public static Packet UpdateNickname(string newNickname)
        {
            return new Packet("UPDATE NICKNAME", newNickname);
        }
        public static Packet Kill(string userId)
        {
            return new Packet("KILL", userId);
        }

        public static string Package(this List<string> raw)
        {
            return DataMap.Serialize(raw);
        }
    }
}
