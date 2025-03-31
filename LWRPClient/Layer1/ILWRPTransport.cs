using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LWRPClient.Layer1
{
    public delegate void ILWRPTransport_OnConnected(ILWRPTransport transport);
    public delegate void ILWRPTransport_OnDisconnected(ILWRPTransport transport, Exception ex);
    public delegate void ILWRPTransport_OnMessageReceived(ILWRPTransport transport, LWRPMessage message);

    public interface ILWRPTransport : IDisposable
    {
        /// <summary>
        /// Event raised when connected successfully.
        /// </summary>
        event ILWRPTransport_OnConnected OnConnected;

        /// <summary>
        /// Event raised when disconnected ungracefully.
        /// </summary>
        event ILWRPTransport_OnDisconnected OnDisconnected;

        /// <summary>
        /// Event raised when a message is received.
        /// </summary>
        event ILWRPTransport_OnMessageReceived OnMessageReceived;

        bool IsConnected { get; }

        /// <summary>
        /// Starts initial connection. Call this once.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Sends a message in the background.
        /// </summary>
        /// <param name="message"></param>
        Task SendMessage(LWRPMessage message);

        /// <summary>
        /// Sends multiple messages in the background.
        /// </summary>
        /// <param name="message"></param>
        Task SendMessages(LWRPMessage[] messages);
    }
}
