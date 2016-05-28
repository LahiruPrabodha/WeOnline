
namespace clientExample
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;

    public class Client
    {
        public TcpClient TcpClient { get; private set; }
        public NetworkStream Stream { get; private set; }
        public Thread ReadThread { get; private set; }
        public StreamWriter Writer { get; private set; }

        public delegate void DataCarrier(string data);
        public event DataCarrier OnDataRecieved;

        public delegate void DisconnectNotify();
        public event DisconnectNotify OnDisconnect;
        
        public delegate void ErrorCarrier(Exception e);
        public event ErrorCarrier OnError;

        public bool Connect(string ipAddress, int port)
        {
            try
            {
                TcpClient = new TcpClient();
                TcpClient.Connect(IPAddress.Parse(ipAddress), port);
                Stream = TcpClient.GetStream();
                Writer = new StreamWriter(Stream);
                ReadThread = new Thread(Read);
                ReadThread.Start();
                return true;
            }
            catch(Exception e)
            {
                if (OnError != null)
                    OnError(e);
                return false;
            }
        }

        private void Read()
        {
            var streamReader = new StreamReader(Stream);
            var charBuffer = new List<int>();

            do
            {
                try
                {
                    if (streamReader.EndOfStream)
                        break;
                    int charCode = streamReader.Read();
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
                        for (int i = 0; i < charBuffer.Count; i++)
                        {
                            chars[i] = Convert.ToChar(charBuffer[i]);
                        }
                        var message = new string(chars);
                        OnDataRecieved(message);
                    }
                    charBuffer.Clear();
                }
                catch(Exception e)
                {
                    if (OnError != null)
                        OnError(e);

                    break;
                }
            } while (true);
            if (OnDisconnect != null)
                OnDisconnect();
        }
        private void WriteRaw(string message)
        {
            try
            {
                Writer.Write(message + "\0");
                Writer.Flush();
            }
            catch(Exception e)
            {
                if (OnError != null)
                    OnError(e);
            }
        }
        public void WritePacket(Packet packet)
        {
            WriteRaw(packet);
        }
    }
}
