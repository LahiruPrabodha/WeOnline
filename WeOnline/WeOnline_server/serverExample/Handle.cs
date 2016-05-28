
namespace serverExample
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Handling;
    using Information;

    public class Handle
    {
        public readonly static Dictionary<string, IHandle> Handles =
            new Dictionary<string, IHandle>();

        public static Dictionary<Client, User> OnlineUsers =
            new Dictionary<Client, User>();

        public static void LoadHandles()
        {
            //Login Request Handle.
            var type = typeof (IHandle);

            //Get a list of all the handles 
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(type.IsAssignableFrom)
                .Where(t=>!t.IsInterface)
                .Select(Activator.CreateInstance)
                .Cast<IHandle>()
                .ToArray();

            //Load all the handles
            foreach (IHandle plg in types)
            {
                //The interface is disabled? don't load.
                if (plg is IDisabledHandle) continue;

                Console.WriteLine("Handle for " + plg.GetCommandName() + " loaded.");
                string commandName = plg.GetCommandName().ToUpper();
                if (!Handles.ContainsKey(commandName))
                    Handles.Add(commandName, plg);
            }
        }
        public static void PurgeCache(int userId)
        {
            lock (OnlineUsers)
            {
                var cached = OnlineUsers.Where(a => a.Value.Id == userId).ToDictionary(a => a.Key, a => a.Value);
                foreach (var user in cached)
                    if (OnlineUsers.ContainsKey(user.Key))
                        lock (OnlineUsers)
                            OnlineUsers[user.Key] = Program.Backend.GetUser(user.Value.Id);
            }
        }
        public static void ClientJoined(Client client)
        {
            lock (Program.ConnectedClients)
                if (!Program.ConnectedClients.Contains(client))
                    Program.ConnectedClients.Add(client);

            Console.WriteLine("[Info] Client connected: {0}", client.TcpClient.Client.RemoteEndPoint);
        }
        public static void ClientLeft(Client client)
        {
            lock (Program.ConnectedClients)
                if (!Program.ConnectedClients.Contains(client))
                    Program.ConnectedClients.Add(client);


            lock (OnlineUsers)
            {
                if (OnlineUsers.ContainsKey(client))
                {
                    Console.WriteLine("[Info] User {0} left", OnlineUsers[client].Email);
                    OnlineUsers.Remove(client);
                    Packets.UserList().WriteAll();
                }
                else
                    Console.WriteLine("[Info] Client disconnected");
            }
        }
        public static void MessageRecieved(Client client, string data)
        {
            try
            {
                var packet = new Packet(data);
                string command = packet.Command.ToUpper();
                if (Handles.ContainsKey(command))
                {
                    if ((Handles[command] is ILoggedInHandle) && !OnlineUsers.ContainsKey(client)
                        || Handles[command] is IUnloggedInHandle && OnlineUsers.ContainsKey(client))
                        return;

                    Handles[command].DoHandle(client, packet);
                }
                else
                {
                    Console.WriteLine("Unhandeled Packet Recieved: " + packet.Command);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.Write("Invalid Packet Data Recieved");
            }
        }
    }
}
