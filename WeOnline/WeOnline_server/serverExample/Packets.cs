
namespace serverExample
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Information;

    public static class Packets
    {
        public static Packet UserList()
        {
            string payload =
                DataMap.Serialize(Handle.OnlineUsers.Values
                                      .Distinct()
                                      .Select(a => new List<string>
                                                       {
                                                           a.Nickname, 
                                                           a.Role.ToString(),
                                                           a.Banned.ToString(),
                                                           a.Id.ToString()
                                                       })
                                      .Select(DataMap.Serialize)
                                      .ToList());
            return new Packet("USER LIST", payload);
        }
        public static Packet YourRank(User user)
        {
            return new Packet("YOUR RANK", user.Role.ToString());
        }
        public static Packet LoginSuccess(int uid, string nickname)
        {
            return new Packet("LOGIN SUCCESS", DataMap.Serialize(
                new List<string>()
                    {
                        uid.ToString(CultureInfo.InvariantCulture),
                        nickname
                    }));
        }

        public static Packet LoginFail(string response)
        {
            return new Packet("LOGIN FAILED", response);
        }
        public static Packet CreateResponse(string message)
        {
            return new Packet("CREATE RESPONSE", message);
        }
        public static Packet CreateFailed(string message)
        {
            return new Packet("CREATE FAILED", message);
        }
        public static Packet Message(string nickname, string data)
        {
            return new Packet("MESSAGE", DataMap.Serialize(new List<string>
                            {
                                nickname,
                                data
                            }));
        }
    }
}
