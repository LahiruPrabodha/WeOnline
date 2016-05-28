
namespace serverExample
{
    using System.Linq;
    using Information;

    public static class AdminTools
    {
        /// <summary>
        /// Changes a users rank, purges the cache, and notifies other logged in users
        /// </summary>
        /// <param name="user">The user to change the rank of</param>
        /// <param name="newRank">The rank that you wish to change the user to</param>
        public static void ChangeRank(User user, User.UserRole newRank)
        {
            user.Role = newRank;
            user.UpdateRole();
            Purge(user);
        }

        /// <summary>
        /// Changes a users nickname, and purges the cache
        /// </summary>
        /// <param name="user">User to change nickname of</param>
        /// <param name="nickname">Nickname you would like to change this user to</param>
        public static void ChangeNickname(User user, string nickname)
        {
            user.Nickname = nickname;
            user.UpdateNickname();
            Purge(user);
        }


        /// <summary>
        /// Bans a user, purges the cache, and notifies other logged in users
        /// </summary>
        /// <param name="user">The user to ban</param>
        public static void BanUser(User user)
        {
            user.Banned = true;
            user.UpdateBanned();
            Purge(user);
        }
    
        /// <summary>
        /// Unbans a user, purges the cache, and notifies other logged in users
        /// </summary>
        /// <param name="user">The user to unban</param>
        public static void UnbanUser(User user)
        {
            user.Banned = false;
            user.UpdateBanned();
            Purge(user);
        }

        /// <summary>
        /// Calls "Handle.PurgeCache" with the specified users id, writes a list update to all clients, and then writes a rank update to all users
        /// </summary>
        /// <param name="user"></param>
        private static void Purge(User user)
        {
            Handle.PurgeCache(user.Id);
            Packets.UserList().WriteAll();
            Program.WriteRankUpdate();
        }

        /// <summary>
        /// Loops through the logged in users dictionary, finds all clients with the specified user, notifies the client with a generic reason, then closes the stream.
        /// </summary>
        /// <param name="user">The user that should have their logged in sessions disconnected</param>
        public static void DisconnectSessions(User user)
        {
            int userId = user.Id;
            DisconnectSessions(userId);
        }

        /// <summary>
        /// Loops through the logged in users dictionary, finds all clients with the specified user, notifies the client with the specified reason, then closes the stream.
        /// </summary>
        /// <param name="user">The user that should have their logged in sessions disconnected</param>
        /// <param name="reason">The reason the client is being disconnected</param>
        public static void DisconnectSessions(User user, string reason)
        {
            int userId = user.Id;
            DisconnectSessions(userId, reason);
        }

        /// <summary>
        /// Loops through the logged in users dictionary, finds all clients with the specified user id, notifies the client with the specified reason, then closes the stream.
        /// </summary>
        /// <param name="userId">The id of the user that should have their logged in sessions disconnected</param>
        public static void DisconnectSessions(int userId)
        {
            lock (Handle.OnlineUsers)
            {
                var sessions = Handle.OnlineUsers.Where(a => a.Value.Id == userId).Select(a => a.Key);
                foreach (var session in sessions)
                {
                    try
                    {
                        Packets.LoginFail("Disconnected").Write(session);
                        session.TcpClient.Close();
                    }
                    catch
                    {
                    }
                }
            }
        }

        /// <summary>
        /// Loops through the logged in users dictionary, finds all clients with the specified user id, notifies the client with the specified reason, then closes the stream.
        /// </summary>
        /// <param name="userId">The id of the user that should have their logged in sessions disconnected</param>
        /// <param name="reason">The reason the client is being disconnected</param>
        public static void DisconnectSessions(int userId, string reason)
        {
            lock (Handle.OnlineUsers)
            {
                var sessions = Handle.OnlineUsers.Where(a => a.Value.Id == userId).Select(a => a.Key);
                foreach (var session in sessions)
                {
                    try
                    {
                        Packets.LoginFail(reason).Write(session);
                        session.TcpClient.Close();
                    }
                    catch
                    {
                    }
                }
            }
        }
    }
}
