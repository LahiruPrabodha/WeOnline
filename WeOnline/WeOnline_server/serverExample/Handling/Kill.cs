
namespace serverExample.Handling
{
    using System;
    using Information;

    public class Kill : ILoggedInHandle
    {
        public string GetCommandName()
        {
            return "KILL";
        }

        public void DoHandle(Client client, Packet packet)
        {
            var currentUser = Handle.OnlineUsers[client];

            if (currentUser.Role != User.UserRole.Admin)
                return;

            int uid = Convert.ToInt32(packet.Carriage);

            AdminTools.DisconnectSessions(uid, "Connection discontinued by an administrator.");
        }
    }
}
