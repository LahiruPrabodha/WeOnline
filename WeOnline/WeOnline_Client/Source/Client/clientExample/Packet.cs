
namespace clientExample
{
    using System;

    public class Packet
    {
        public Packet(string rawdata)
        {
            int sepindex = rawdata.IndexOf(":", StringComparison.Ordinal);
            Command = rawdata.Substring(0, sepindex);
            Carriage = rawdata.Substring(Command.Length + 2);
        }

        public Packet(string command, string carriage)
        {
            Command = command;
            Carriage = carriage;
        }

        public string Serialize()
        {
            return string.Format("{0}: {1}", Command, Carriage.Replace("\0", ""));
        }

        public static implicit operator string(Packet value)
        {
            return value.Serialize();
        }

        public string Command { get; set; }
        public string Carriage { get; set; }
    }
}
