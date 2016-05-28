
namespace serverExample
{
    using System.IO;
    using System.Net.Sockets;
    using System.Threading;

    public class Client
    {
        public Client(TcpClient client)
        {
            var ns = client.GetStream();
            StreamReader = new StreamReader(ns);
            StreamWriter = new StreamWriter(ns);
            TcpClient = client;
        }

        public TcpClient TcpClient;
        public StreamReader StreamReader;
        public StreamWriter StreamWriter;
        public Thread ReadThread;
    }
}