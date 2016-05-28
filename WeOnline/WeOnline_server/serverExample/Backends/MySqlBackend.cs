
namespace serverExample.Backends
{
    using System;
    using System.Data;
    using Information;
    using MySql.Data.MySqlClient;

    public class MySqlBackend : IBackend
    {
        public string GetBackendName()
        {
            return "MySql";
        }

        public class MySqlConnectionString
    {
        /// <summary>
        /// Provides a private parameter reference for parameters specified on construction
        /// </summary>
        private readonly Params _csParams;

        /// <summary>
        /// Constructs an instance of the ConnectionString class
        /// </summary>
        /// <param name="csParams">The parameters required to initialize the connection string class</param>
        public MySqlConnectionString(Params csParams)
        {
            _csParams = csParams;
        }

        /// <summary>
        /// Provides the capability to implicitly cast <see cref="MySqlConnectionString"/> as a string
        /// </summary>
        public static implicit operator string(MySqlConnectionString value)
        {
            return value.ToString();
        }

        /// <summary>
        /// Converts the stored properties of the class to a valid MySQL connection string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format(
                "Server={0};Database={1};UID={2};Password={3};CharacterSet={4};Pooling=True;Maximum Pool Size={5};",
                _csParams.Server,
                _csParams.Database,
                _csParams.Username,
                _csParams.Password,
                _csParams.CharacterSet,
                _csParams.MaximumPoolSize);
        }

        /// <summary>
        /// Provides properties for initializing a ConnectionString
        /// </summary>
        public class Params
        {
            /// <summary>
            /// Parameters used for constructing a connection string class
            /// </summary>
            public Params(
                string server,
                string database,
                string username,
                string password,
                string charset,
                int maxpoolsize)
            {
                Server = server;
                Database = database;
                Username = username;
                Password = password;
                CharacterSet = charset;
                MaximumPoolSize = maxpoolsize;
            }

            /// <summary>
            /// The hostname of the database server
            /// </summary>
            public string Server
            {
                get;
                private set;
            }

            /// <summary>
            /// The default schema of the connection
            /// </summary>
            public string Database
            {
                get;
                private set;
            }

            /// <summary>
            /// The username of the database server
            /// </summary>
            public string Username
            {
                get;
                private set;
            }

            /// <summary>
            /// The password of the database server
            /// </summary>
            public string Password
            {
                get;
                private set;
            }

            /// <summary>
            /// The default character set to use (e.g. UTF8)
            /// </summary>
            public string CharacterSet
            {
                get;
                private set;
            }

            /// <summary>
            /// The maximum number of the connections in the connection pool of the database server
            /// </summary>
            public int MaximumPoolSize
            {
                get;
                private set;
            }
        }
    }

        private string MySQLConnectionString { get; set; }

        public MySqlBackend(MySqlConnectionString connectionString)
        {
            MySQLConnectionString = connectionString;
        }
        private MySqlConnection GetConnection()
        {
            var connection = new MySqlConnection(MySQLConnectionString);
            connection.Open();
            return connection;
        }
        public User GetUser(string email)
        {
            using (var connection = GetConnection())
            {
                var command = connection.CreateCommand();
                command.CommandType = CommandType.Text;
                command.CommandText = "SELECT * from users where email = @email LIMIT 1";
                command.Parameters.Add(new MySqlParameter("@email", email));
                return GetUser(command);
            }
        }
        public User GetUser(int userId)
        {
            using (var connection = GetConnection())
            {
                var command = connection.CreateCommand();
                command.CommandType = CommandType.Text;
                command.CommandText = "SELECT * from users where id = @id LIMIT 1";
                command.Parameters.Add(new MySqlParameter("@id", userId));
                return GetUser(command);
            }
        }
        private User GetUser(MySqlCommand recvCommand)
        {
            using (var command = recvCommand)
            {
                using (var reader = command.ExecuteReader())
                {
                    if (!reader.HasRows)
                        return new User
                                   {
                                       Exists = false
                                   };
                    while (reader.Read())
                    {
                        var userid = Convert.ToInt32(reader["id"].ToString());
                        var useremail = reader["email"].ToString();
                        var userbanned = reader["banned"].ToString().ToLower() == "true";
                        var username = reader["nickname"].ToString();
                        var userpassword = reader["password"].ToString();
                        var preuserrole = reader["role"].ToString();
                        var userrole =
                            (User.UserRole)
                            Enum.Parse(
                                typeof (User.UserRole),
                                preuserrole,
                                true);
                        return new User
                                   {
                                       Exists = true,
                                       Id = userid,
                                       Banned = userbanned,
                                       Nickname = username,
                                       Email = useremail,
                                       Password = userpassword,
                                       Role = userrole
                                   };
                    }

                    //This should never happen..
                    return new User
                               {
                                   Exists = false
                               };
                }
            }
        }

        public void CreateUser(string email, string password, string nickname, User.UserRole role)
        {
            using (var connection = GetConnection())
            {
                var command = connection.CreateCommand();
                command.CommandType = CommandType.Text;
                command.CommandText = "INSERT INTO users (nickname, email, password, role, banned) VALUES(@nickname, @email, @password, @role, @banned)";
                command.Parameters.Add(new MySqlParameter("@email", email));
                command.Parameters.Add(new MySqlParameter("@nickname", nickname));
                command.Parameters.Add(new MySqlParameter("@password", password));
                command.Parameters.Add(new MySqlParameter("@banned", "false"));
                command.Parameters.Add(new MySqlParameter("@role", role.ToString()));
                command.ExecuteNonQuery();
            }
        }

        public void UpdateUserNickname(int userId, string newNickname)
        {
            using (var connection = GetConnection())
            {
                var command = connection.CreateCommand();
                command.CommandType = CommandType.Text;
                command.CommandText = "UPDATE users set nickname = @nickname where id = @userId";
                command.Parameters.Add(new MySqlParameter("@userId", userId));
                command.Parameters.Add(new MySqlParameter("@nickname", newNickname));
                command.ExecuteNonQuery();
            }
        }
        public void UpdateUserPassword(int userId, string newPassword)
        {
            using (var connection = GetConnection())
            {
                var command = connection.CreateCommand();
                command.CommandType = CommandType.Text;
                command.CommandText = "UPDATE users set password = @pass where id = @userId";
                command.Parameters.Add(new MySqlParameter("@userId", userId));
                command.Parameters.Add(new MySqlParameter("@pass", newPassword));
                command.ExecuteNonQuery();
            }
        }
        public void UpdateUserRole(int userId, User.UserRole userRole)
        {
            using (var connection = GetConnection())
            {
                var command = connection.CreateCommand();
                command.CommandType = CommandType.Text;
                command.CommandText = "UPDATE users set role = @role where id = @userId";
                command.Parameters.Add(new MySqlParameter("@userId", userId));
                command.Parameters.Add(new MySqlParameter("@role", userRole.ToString()));
                command.ExecuteNonQuery();
            }
        }
        public void UpdateUserBanned(int userId, bool banned)
        {
            using (var connection = GetConnection())
            {
                var command = connection.CreateCommand();
                command.CommandType = CommandType.Text;
                command.CommandText = "UPDATE users set banned = @ban where id = @userId";
                command.Parameters.Add(new MySqlParameter("@userId", userId));
                command.Parameters.Add(new MySqlParameter("@ban", banned.ToString()));
                command.ExecuteNonQuery();
            }
        }
    }
}