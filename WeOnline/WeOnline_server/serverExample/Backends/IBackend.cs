
namespace serverExample.Backends
{
    using Information;

    public interface IBackend
    {
        /// <summary>
        /// A generic name for the backend
        /// </summary>
        /// <returns>The name of the backend</returns>
        string GetBackendName();

        /// <summary>
        /// This method should return a user with the specified email from the database. If the user is non existent, "Exists" will be set to false.
        /// </summary>
        /// <param name="email">The email of the user that should be returned</param>
        /// <returns>A user class with it's attributes set accordingly to the database</returns>
        User GetUser(string email);
        /// <summary>
        /// This method should return a user with the specified ID from the database. If the user is non existent, "Exists" will be set to false.
        /// </summary>
        /// <param name="userId">The ID of the user that should be returned</param>
        /// <returns>A user class with it's attributes set accordingly to the database</returns>
        User GetUser(int userId);

        /// <summary>
        /// This method should create a user in the database
        /// </summary>
        /// <param name="email">The email of the user that will be added</param>
        /// <param name="password">The password of the user that will be added</param>
        /// <param name="role">The role of the user that will be added</param>
        void CreateUser(string email, string password, string nickname, User.UserRole role);

        /// <summary>
        /// This method should change a users nickname where the user's id is equal to the recieved userId
        /// </summary>
        /// <param name="userId">The id of the user to update</param>
        /// <param name="newNickname">The string that the user's nickname should be changed to</param>
        void UpdateUserNickname(int userId, string newNickname);
        /// <summary>
        /// This method should change a users password where the user's id is equal to the recieved userId
        /// </summary>
        /// <param name="userId">The id of the user to update</param>
        /// <param name="newPassword">The string that the user's password should be changed to</param>
        void UpdateUserPassword(int userId, string newPassword);
        /// <summary>
        /// This method should change a users role where the user's id is equal to the recieved userId
        /// </summary>
        /// <param name="userId">The id of the user to update</param>
        /// <param name="userRole">The User.UserRole that the user's role should be changed to</param>
        void UpdateUserRole(int userId, User.UserRole userRole);
        /// <summary>
        /// This method should change whether or not a user is banned, according to the recieved userId
        /// </summary>
        /// <param name="userId">The id of the user to update</param>
        /// <param name="banned">Whether or not this user is banned</param>
        void UpdateUserBanned(int userId, bool banned);
    }
}