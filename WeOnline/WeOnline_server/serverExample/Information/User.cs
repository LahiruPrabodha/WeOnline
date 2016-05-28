

namespace serverExample.Information
{
    public class User
    {
        /// <summary>
        /// Does this user exist in the current backend - or not?
        /// </summary>
        public bool Exists { get; set; }

        /// <summary>
        /// The unique Id of the user
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The users nickname, used in chat, and shown to other users in chat
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// The users email, used for login.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// The users password, used for login, and other verification methods
        /// </summary>
        public string Password { get; set; }
        
        /// <summary>
        /// The users role.
        /// </summary>
        public UserRole Role { get; set; }

        /// <summary>
        /// Whether or not this user is banned.
        /// </summary>
        public bool Banned { get; set; }

        /// <summary>
        /// Queries the backend for a user with the specified email
        /// </summary>
        /// <param name="email">The email of the user to retrieve</param>
        /// <returns>A user, if existing, with all the appropriate properties filled. If the user does not exist, "User.Exists" will be set to false and no properties should be filled</returns>
        public static User GetUser(string email)
        {
            return Program.Backend.GetUser(email);
        }

        /// <summary>
        /// Queries the backend for a user with the specified user id
        /// </summary>
        /// <param name="userId">The id of the user to retrieve</param>
        /// <returns>A user, if existing, with all the appropriate properties filled. If the user does not exist, "User.Exists" will be set to false and no properties should be filled</returns>
        public static User GetUser(int userId)
        {
            return Program.Backend.GetUser(userId);
        }

        /// <summary>
        ///  Calls the currently loaded backend's UpdateUserPassword method, purging password changes to the backend
        /// </summary>
        public void UpdatePassword()
        {
            Program.Backend.UpdateUserPassword(Id, Password);
        }

        /// <summary>
        /// Calls the currently loaded backend's UpdateUserNickname method, purging nickname changes to the backend
        /// </summary>
        public void UpdateNickname()
        {
            Program.Backend.UpdateUserNickname(Id, Nickname);
        }

        /// <summary>
        /// Calls the currently loaded backend's UpdateUserRole method, purging role changes to the backend
        /// </summary>
        public void UpdateRole()
        {
            Program.Backend.UpdateUserRole(Id, Role);
        }

        /// <summary>
        /// Calls the currently loaded backend's UpdateUserBanned method, purging banned changes to the backend
        /// </summary>
        public void UpdateBanned()
        {
            Program.Backend.UpdateUserBanned(Id, Banned);
        }

        public enum UserRole
        {
            /// <summary>
            /// Represents a user with no administrative privs
            /// </summary>
            Regular,

            /// <summary>
            /// Represents a user with administrative privs
            /// </summary>
            Admin
        }
    }
}
