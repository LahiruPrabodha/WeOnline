
namespace serverExample.Handling
{
    using System;
    using Information;

    public class UpdateRole : ILoggedInHandle
    {
        public string GetCommandName()
        {
            return "UPDATE ROLE";
        }

        public void DoHandle(Client client, Packet packet)
        {
            var user = Handle.OnlineUsers[client];

            if (string.IsNullOrEmpty(packet.Carriage)) return;

            var map = packet.Carriage.Deserialize();
            if (map.Count != 2)
                return;

            int userId = Convert.ToInt32(map[0]);
            string newRole = map[1];
            if (user.Role != User.UserRole.Admin) return;

            try
            {
                User tuser = User.GetUser(userId);
                if (!tuser.Exists) return;
                if (newRole == "banned")
                {
                    AdminTools.BanUser(user);
                    Console.WriteLine("[Info] {0} banned user {1}", user.Email, tuser.Email);
                    return;
                }
                if (newRole == "unbanned")
                {
                    AdminTools.UnbanUser(tuser);
                    Console.WriteLine("[Info] {0} unbanned user {1}", user.Email, tuser.Email);
                    return;
                }
                var role = (User.UserRole) (Enum.Parse(typeof (User.UserRole), newRole, true));
                User tuser2 = User.GetUser(userId);
                if (!tuser2.Exists) return;
                AdminTools.ChangeRank(tuser, role);
                Console.WriteLine("[Info] {0} changed {1}'s rank to {2}", user.Email, tuser.Email, newRole.ToString());
            }
            catch
            {
            }
        }
    }
}