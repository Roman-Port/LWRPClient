using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LWRPClient.Layer1.Transports
{
    public class TcpLWRPTransport : ILWRPTransport
    {
        public TcpLWRPTransport(IPEndPoint endpoint, int bufferSize = 32768, int timeout = 1000)
        {
            this.endpoint = endpoint;
            this.timeout = timeout;
            buffer = new byte[bufferSize];
        }

        private readonly IPEndPoint endpoint;
        private readonly byte[] buffer;
        private readonly object mutex = new object();
        private readonly int timeout;
        private Socket sock;
        private int bufferConsumed;

        /// <summary>
        /// If true, will not attempt to reconnect when socket is closed.
        /// </summary>
        private bool disposing = false;

        public bool IsConnected => sock.Connected;

        /// <summary>
        /// Event raised when connected successfully.
        /// </summary>
        public event ILWRPTransport_OnConnected OnConnected;

        /// <summary>
        /// Event raised when disconnected ungracefully.
        /// </summary>
        public event ILWRPTransport_OnDisconnected OnDisconnected;

        /// <summary>
        /// Event raised when a message is received.
        /// </summary>
        public event ILWRPTransport_OnMessageReceived OnMessageReceived;

        /// <summary>
        /// Starts initial connection. Call this once.
        /// </summary>
        public void Initialize()
        {
            InitSocket();
        }

        private void InitSocket()
        {
            lock (mutex)
            {
                //Delete old socket if there is one
                if (sock != null)
                    sock.Dispose();

                //Create socket
                sock = new Socket(SocketType.Stream, ProtocolType.Tcp);
                sock.ReceiveTimeout = timeout;
                sock.SendTimeout = timeout;
                SetKeepAliveValues(1, (uint)(timeout * 10), (uint)timeout);

                //Connect
                sock.BeginConnect(endpoint, ConnectCallback, null);
            }
        }

        private void SetKeepAliveValues(uint on, uint keepAliveInterval, uint retryInterval)
        {
            int size = sizeof(uint);
            byte[] buffer = new byte[size * 3];
            Array.Copy(BitConverter.GetBytes(on), 0, buffer, 0, size);
            Array.Copy(BitConverter.GetBytes(keepAliveInterval), 0, buffer, size, size);
            Array.Copy(BitConverter.GetBytes(retryInterval), 0, buffer, size * 2, size);
            sock.IOControl(IOControlCode.KeepAliveValues, buffer, null);
        }

        /// <summary>
        /// Async result from connecting.
        /// </summary>
        /// <param name="result"></param>
        private void ConnectCallback(IAsyncResult result)
        {
            lock (mutex)
            {
                //Finish connect
                try
                {
                    sock.EndConnect(result);
                }
                catch (Exception ex)
                {
                    SocketError(ex);
                    return;
                }

                //Raise event
                OnConnected?.Invoke(this);

                //Start rx loop
                ReceiveNext();
            }
        }

        /// <summary>
        /// Async receive. Assumes we are in mutex.
        /// </summary>
        private void ReceiveNext()
        {
            //Check buffer size
            if (bufferConsumed == buffer.Length)
            {
                SocketError(new Exception("RX buffer is full."));
                return;
            }

            //Begin RX
            try
            {
                sock.BeginReceive(buffer, bufferConsumed, buffer.Length - bufferConsumed, SocketFlags.None, OnReceiveEnd, null);
            }
            catch (Exception ex)
            {
                SocketError(ex);
                return;
            }
        }

        private void OnReceiveEnd(IAsyncResult ar)
        {
            lock (mutex)
            {
                //End recieve
                int read;
                try
                {
                    read = sock.EndReceive(ar);
                }
                catch (Exception ex)
                {
                    SocketError(ex);
                    return;
                }

                //If end of stream, abort
                if (read == 0)
                {
                    SocketError(new EndOfStreamException());
                    return;
                }

                //Update
                bufferConsumed += read;

                //Attempt to read messages in buffer
                while (ReadNextInBuffer(out LWRPMessage message))
                    OnMessageReceived?.Invoke(this, message);

                //Start next read
                ReceiveNext();
            }
        }

        /// <summary>
        /// Raised when there's a socket error sending, reciving, or connecting.
        /// </summary>
        private void SocketError(Exception ex)
        {
            //Send out event
            OnDisconnected?.Invoke(this, ex);

            //Dispose of socket
            sock.Dispose();

            //Attempt to reconnect
            if (!disposing)
                InitSocket();
        }

        private bool ReadNextInBuffer(out LWRPMessage message)
        {
            //Scan for newline
            for (int i = 0; i < bufferConsumed; i++)
            {
                if (buffer[i] == (byte)'\n')
                {
                    //Pop this off the buffer and convert to string
                    string raw = Encoding.ASCII.GetString(buffer, 0, i);

                    //Shift buffer
                    Array.Copy(buffer, i + 1, buffer, 0, buffer.Length - (i + 1));
                    bufferConsumed -= i + 1;

                    //Decode
                    message = LWRPMessage.Deserialize(raw);
                    return true;
                }
            }

            //No complete message (yet)
            message = null;
            return false;
        }

        public void Dispose()
        {
            //Set flag
            disposing = true;

            //Shut down socket
            sock.Dispose();
        }

        public Task SendMessage(LWRPMessage message)
        {
            return SendMessages(new LWRPMessage[] { message });
        }

        public Task SendMessages(LWRPMessage[] messages)
        {
            //Serialize messages
            string raw = "";
            foreach (var m in messages)
                raw += m.Serialize() + "\r\n";

            //Convert to bytes
            byte[] data = Encoding.ASCII.GetBytes(raw);

            //Send
            TaskCompletionSource<int> promise = new TaskCompletionSource<int>();
            sock.BeginSend(data, 0, data.Length, SocketFlags.None, OnSendMessage, promise);
            return promise.Task;
        }

        private void OnSendMessage(IAsyncResult ar)
        {
            //Get promise from the object
            TaskCompletionSource<int> promise = (TaskCompletionSource<int>)ar.AsyncState;

            //Handle response
            int sent;
            try
            {
                //End
                sent = sock.EndSend(ar);

                //Set result
                promise.SetResult(sent);
            }
            catch (Exception ex)
            {
                //Raise error on the promise
                promise.SetException(ex);

                //Fire socket error event for use re-connecting
                lock (mutex)
                    SocketError(ex);
            }
        }
    }
}
