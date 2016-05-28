
namespace serverExample
{
    using System;
    using System.IO;
    using Backends;

    public static class Config
    {
        public static string GetStartupText()
        {
            return
@"%---------------%
| WeOnline Server |
| Version: 1.0    |
%-----------------%

Starting Server...
";
        }

        public static string GetMotd()
        {
            if (File.Exists("MOTD.txt"))
                return File.ReadAllText("MOTD.txt");
            return "Welcome!";
        }

        public static string GetListeningAddress()
        {
            if (File.Exists("LISTEN.txt"))
                return File.ReadAllText("LISTEN.txt").Split(':')[0];
            return "0.0.0.0";
        }

        public static int GetListeningPort()
        {
            if (File.Exists("LISTEN.txt"))
                return Convert.ToInt32(File.ReadAllText("LISTEN.txt").Split(':')[1]);
            return 1987;
        }

        public static IBackend GetBackend()
        {
            return new MySqlBackend(
                new MySqlBackend.MySqlConnectionString(
                    new MySqlBackend.MySqlConnectionString.Params(
                        "localhost",
                        "xsdevx",
                        "root",
                        "",
                        "utf8",
                        100
                        )
                    )
                );
        }
    }
}
