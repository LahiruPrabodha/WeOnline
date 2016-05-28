
namespace serverExample.Handling
{
    public interface IHandle
    {
        /// <summary>
        /// The method called when a packet with the specified CommandName is recieved
        /// </summary>
        /// <param name="client">The client the packet was recieved from</param>
        /// <param name="packet">The packet that the client sent</param>
        void DoHandle(Client client, Packet packet);

        /// <summary>
        /// The command that the interface should handle, this should be unique
        /// </summary>
        string GetCommandName();
    }

    public interface ILoggedInHandle : IHandle
    {
    }
    public interface IUnloggedInHandle : IHandle
    {
    }
    public interface IDisabledHandle : IHandle
    {
    }
}