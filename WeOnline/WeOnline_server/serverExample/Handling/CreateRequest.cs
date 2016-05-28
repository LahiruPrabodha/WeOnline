
namespace serverExample.Handling
{
    using System;
    using System.Collections.Generic;
    using Information;

    public class CreateRequest : IUnloggedInHandle
    {
        public string GetCommandName()
        {
            return "CREATE REQUEST";
        }

        public void DoHandle(Client client, Packet packet)
        {
            if (string.IsNullOrEmpty(packet.Carriage)) return;
            List<string> dataMap;
            try
            {
                dataMap = packet.Carriage.Deserialize();
            }
            catch
            {
                Packets.CreateFailed("Invalid Carriage Data").Write(client);
                return;
            }

            if (dataMap.Count != 3)
            {
                Packets.CreateFailed("Invalid Carriage Data").Write(client);
                return;
            }

            string email = dataMap[0].ToLower().Trim();
            string pass = dataMap[1];
            string nickname = dataMap[2].Trim();

            if (string.IsNullOrEmpty(email) 
                || string.IsNullOrEmpty(pass))
            {
                Packets.CreateFailed("Invalid Carriage Data").Write(client);
                return;
            }

            if (User.GetUser(email).Exists)
            {
                Packets.CreateFailed("A user with that email already exists.").Write(client);
                return;
            }

            Program.Backend.CreateUser(email, pass, nickname, User.UserRole.Regular);

            User actualUser = User.GetUser(email);
            Console.WriteLine("[Info] New user {0} (ID: {1}) registered with nickname {2}", actualUser.Email, actualUser.Id, actualUser.Nickname);

            Packets.CreateResponse("Created").Write(client);
        }
    }
}