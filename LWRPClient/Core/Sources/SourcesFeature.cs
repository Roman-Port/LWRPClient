using LWRPClient.Exceptions;
using LWRPClient.Features;
using LWRPClient.Layer1;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace LWRPClient.Core.Sources
{
    class SourcesFeature : BatchUpdateFeature<ILWRPSource>, ILWRPSourcesFeature
    {
        public SourcesFeature(LWRPConnection connection) : base(connection)
        {
            //Create sources
            for (int i = 0; i < sources.Length; i++)
                sources[i] = new LwSrc(connection, i + 1);

            //Bind to events
            connection.OnInfoDataReceived += Connection_OnInfoDataReceived;
            connection.SubscribeToMessage("SRC", OnUpdateMessageReceived);
        }

        private readonly LwSrc[] sources = new LwSrc[256];

        /// <summary>
        /// The number of sources.
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

                    return connection.SrcNum;
                }
            }
        }

        /// <summary>
        /// Accesses a source.
        /// </summary>
        public ILWRPSource this[int index]
        {
            get
            {
                ILWRPSource result;
                lock (connection.mutex)
                {
                    //Do bounds check
                    if (index < 0 || index >= Count)
                        throw new IndexOutOfRangeException();

                    //Read
                    result = sources[index];
                }
                return result;
            }
        }

        private void Connection_OnInfoDataReceived(LWRPConnection conn)
        {
            //Request info
            conn.transport.SendMessage(new LWRPMessage("SRC", new LWRPToken[0][]));
        }

        private void OnUpdateMessageReceived(LWRPMessage message, bool inGroup)
        {
            //Get the source index (starting at 1)
            int index = int.Parse(message.Arguments[0][0].Content);
            LwSrc source = sources[index - 1];

            //Process update
            source.ProcessUpdate(message);

            //Enqueue to batch updates
            EnqueueReceivedUpdate(source, inGroup);
        }

        internal override void Apply(IList<LWRPMessage> updates)
        {
            //Loop through all and add any 
            for (int i = 0; i < connection.SrcNum; i++)
            {
                if (sources[i].CreateUpdateMessage(out LWRPMessage msg))
                    updates.Add(msg);
            }
        }
    }
}
