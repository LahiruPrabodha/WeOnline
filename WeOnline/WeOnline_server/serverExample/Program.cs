
namespace serverExample
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using Backends;

    public static class Program
    {
        public static IBackend Backend { get; private set; }
        public static string Motd { get; private set; }

        public delegate void ClientCarrier(Client client);
        public static event ClientCarrier OnClientConnected;
        public static event ClientCarrier OnClientDisconnected;

        public delegate void ErrorCarrier(Exception e);
        public static event ErrorCarrier OnServerError;

        public delegate void DataRecieved(Client client, string data);
        public static event DataRecieved OnDataRecieved;

        private static TcpListener _tcpListener;
        private static Thread _acceptThread;
        private static List<Client> _connectedClients;
        public static List<Client> ConnectedClients
        {
            get { return _connectedClients; }
            set { _connectedClients = value; }
        }

        private static void Main(string[] args)
        {
            Console.WriteLine(Config.GetStartupText());

            _connectedClients = new List<Client>();

            OnDataRecieved += Handle.MessageRecieved;
            OnClientDisconnected += Handle.ClientLeft;
            OnClientConnected += Handle.ClientJoined;
            OnServerError+=(e)=>
                Console.WriteLine("[Error Occured]: {0}", e.ToString().Replace("\n", String.Empty));

            LoadMotd();
            LoadBackend(Config.GetBackend());

            Handle.LoadHandles();
            Listen(Config.GetListeningAddress(), Config.GetListeningPort());
        }

        private static void LoadBackend(IBackend backend)
        {
            Backend = backend;
            Console.WriteLine("Backend type is {0}", backend.GetBackendName());
        }
        private static void LoadMotd()
        {
            Motd = Config.GetMotd();
        }
        private static void Listen(string ipAddress, int port)
        {
            try
            {
                _tcpListener = new TcpListener(IPAddress.Parse(ipAddress), port);
                _tcpListener.Start(1000);
                _acceptThread = new Thread(AcceptClients);
                _acceptThread.Start();

                Console.WriteLine("Listening on {0}:{1}", ipAddress, port);
            }
            catch(Exception e)
            {
                if (OnServerError != null)
                    OnServerError(e);
            }
        }
        private static void AcceptClients()
        {
            do
            {
                try
                {
                    var client = _tcpListener.AcceptTcpClient();
                    var srvClient = new Client(client)
                                        {
                                            ReadThread = new Thread(ClientRead)
                                        };
                    srvClient.ReadThread.Start(srvClient);

                    if (OnClientConnected != null)
                        OnClientConnected(srvClient);
                }
                catch(Exception e)
                {
                    if (OnServerError != null)
                        OnServerError(e);
                }

            } while (true);
        }
        private static void ClientRead(object client)
        {
            var cli = client as Client;
            var charBuffer = new List<int>();

            do
            {
                try
                {
                    if (cli == null)
                        break;
                    if (cli.StreamReader.EndOfStream)
                        break;
                    int charCode = cli.StreamReader.Read();
                    if (charCode == -1)
                        break;
                    if (charCode != 0)
                    {
                        charBuffer.Add(charCode);
                        continue;
                    }
                    if (OnDataRecieved != null)
                    {
                        var chars = new char[charBuffer.Count];
                        //Convert all the character codes to their representable characters
                        for (int i = 0; i < charBuffer.Count; i++)
                        {
                            chars[i] = Convert.ToChar(charBuffer[i]);
                        }
                        //Convert the character array to a string
                        var message = new string(chars);

                        //Invoke our event
                        OnDataRecieved(cli, message);
                    }
                    charBuffer.Clear();
                }
                catch (IOException)
                {
                    break;
                }
                catch (Exception e)
                {
                    if (OnServerError != null)
                        OnServerError(e);

                    break;
                }
            } while (true);

            if (OnClientDisconnected != null)
                OnClientDisconnected(cli);
        }
        public static void WriteMessage(Client client, string message)
        {
            try
            {
                client.StreamWriter.Write(message + '\0');
                client.StreamWriter.Flush();
            }
            catch(Exception e)
            {
                if (OnServerError != null)
                    OnServerError(e);
            }
        }
        public static void Write(this Packet packet, Client client)
        {
            WriteMessage(client, packet.Serialize());
        }
        public static void WriteAll(this Packet packet)
        {
            lock (Handle.OnlineUsers)
            {
                foreach (var user in Handle.OnlineUsers)
                {
                    packet.Write(user.Key);
                }
            }
        }
        public static void WriteRankUpdate()
        {
            lock (Handle.OnlineUsers)
            {
                foreach (var client in Handle.OnlineUsers)
                {
                    try
                    {
                        Packets.YourRank(client.Value).Write(client.Key);
                    }
                    catch
                    {
                    }
                }
            }
        }
    }
}
