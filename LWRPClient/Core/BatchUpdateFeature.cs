using LWRPClient.Features;
using System;
using System.Collections.Generic;
using System.Text;

namespace LWRPClient.Core
{
    abstract class BatchUpdateFeature<T> : BaseFeature, ILWRPBatchUpdateFeature<T>
    {
        public BatchUpdateFeature(LWRPConnection connection) : base(connection)
        {
            //Bind to when a group is complete
            connection.OnGroupProcessingEnd += Connection_OnGroupProcessingEnd;
        }

        /// <summary>
        /// Event raised when updates are received. If recieved as a batch, this will contain all updates.
        /// </summary>
        public event LWRPConnection.BatchUpdateEventArgs<T> OnBatchUpdate;

        /// <summary>
        /// List of items that are pending updates FROM SERVER.
        /// </summary>
        private readonly List<T> rxPendingUpdates = new List<T>();

        protected void EnqueueReceivedUpdate(T item, bool isProcessingGroup)
        {
            //Add to event list
            lock (rxPendingUpdates)
            {
                if (!rxPendingUpdates.Contains(item))
                    rxPendingUpdates.Add(item);
            }

            //If NOT in a group, send a batch update now. Otherwise it'll be sent when we end the group
            if (!isProcessingGroup)
                SendBatchEvents();
        }

        /// <summary>
        /// Sent either when a group ends or a stray group-less message is processed.
        /// </summary>
        private void SendBatchEvents()
        {
            //Get items and clear pending
            T[] items;
            lock (rxPendingUpdates)
            {
                items = rxPendingUpdates.ToArray();
                rxPendingUpdates.Clear();
            }

            //Fire event
            if (items.Length > 0 && IsReady())
                OnBatchUpdate?.Invoke(connection, items);

            //Automatically mark as ready
            MarkAsReady();
        }

        private void Connection_OnGroupProcessingEnd(LWRPConnection conn)
        {
            //Send current recieved batch, if any
            SendBatchEvents();
        }
    }
}
