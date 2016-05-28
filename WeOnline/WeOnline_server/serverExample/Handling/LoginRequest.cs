
namespace serverExample.Handling
{
    using System;
    using Information;

    public class LoginRequest : IUnloggedInHandle
    {
        public string GetCommandName()
        {
            return "LOGIN REQUEST";
        }

        public void DoHandle(Client client, Packet packet)
        {
            var clientMap = packet.Carriage.Deserialize();
            if (clientMap.Count != 2)
            {
                return;
            }
            string email = clientMap[0];
            string password = clientMap[1];

            if (string.IsNullOrEmpty(email)
                || string.IsNullOrEmpty(password))
            {
                Packets.LoginFail("Invalid Email or Password").Write(client);
                return;
            }

            var user = User.GetUser(email);

            if (!user.Exists)
            {
                Packets.LoginFail("Account does not exist").Write(client);
                return;
            }

            if (user.Password == password)
            {
                Packets.LoginSuccess(user.Id, user.Nickname).Write(client);

                AdminTools.DisconnectSessions(user.Id, "Logged In Elsewhere");

                Packets.Message("{0} joined the chat", user.Nickname).WriteAll();

                lock (Handle.OnlineUsers)
                {
                    if (Handle.OnlineUsers.ContainsKey(client))
                        Handle.OnlineUsers[client] = user;
                    else 
                        Handle.OnlineUsers.Add(client, user);
                }

                Console.WriteLine("[Info] User {0} ({1}) now online, role: {2}",
                    user.Nickname,
                    user.Email,
                    user.Role.ToString());

                Packets.Message("Server", Program.Motd).Write(client);

                Packets.UserList().WriteAll();
                return;
            }

            Packets.LoginFail("Invalid Password").Write(client);
        }
    }
}
