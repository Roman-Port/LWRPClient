using LWRPClient.Exceptions;
using LWRPClient.Features;
using LWRPClient.Layer1;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace LWRPClient.Core.Destinations
{
    class DestinationsFeature : BatchUpdateFeature<ILWRPDestination>, ILWRPDestinationsFeature
    {
        public DestinationsFeature(LWRPConnection connection) : base(connection)
        {
            //Create destinations
            for (int i = 0; i < destinations.Length; i++)
                destinations[i] = new LwDst(connection, i + 1);

            //Bind to events
            connection.OnInfoDataReceived += Connection_OnInfoDataReceived;
            connection.SubscribeToMessage("DST", OnUpdateMessageReceived);
        }

        private readonly LwDst[] destinations = new LwDst[256];

        /// <summary>
        /// The number of destinations.
        /// </summary>
        public int Count
        {
            get
            {
                lock (connection.mutex)
                {
                    //Make sure we have info data
                    if (!connection.HasInfoData)
                        throw new InfoDataNotReadyException();

                    return connection.DstNum;
                }
            }
        }

        /// <summary>
        /// Accesses a destination.
        /// </summary>
        public ILWRPDestination this[int index]
        {
            get
            {
                ILWRPDestination result;
                lock (connection.mutex)
                {
                    //Do bounds check
                    if (index < 0 || index >= Count)
                        throw new IndexOutOfRangeException();

                    //Read
                    result = destinations[index];
                }
                return result;
            }
        }

        private void Connection_OnInfoDataReceived(LWRPConnection conn)
        {
            //Request info
            conn.transport.SendMessage(new LWRPMessage("DST", new LWRPToken[0][]));
        }

        private void OnUpdateMessageReceived(LWRPMessage message, bool inGroup)
        {
            //Get the source index (starting at 1)
            int index = int.Parse(message.Arguments[0][0].Content);
            LwDst dst = destinations[index - 1];

            //Process update
            dst.ProcessUpdate(message);

            //Enqueue to batch updates
            EnqueueReceivedUpdate(dst, inGroup);
        }

        internal override void Apply(IList<LWRPMessage> updates)
        {
            //Loop through all and add any
            for (int i = 0; i < connection.DstNum; i++)
            {
                if (destinations[i].CreateUpdateMessage(out LWRPMessage msg))
                    updates.Add(msg);
            }
        }
    }
}
