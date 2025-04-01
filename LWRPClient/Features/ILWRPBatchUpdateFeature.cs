using System;
using System.Collections.Generic;
using System.Text;

namespace LWRPClient.Features
{
    /// <summary>
    /// Generic feature interface for features that support getting batch updates from the server.
    /// </summary>
    public interface ILWRPBatchUpdateFeature<T> : ILWRPFeature
    {
        /// <summary>
        /// Event raised when there is a batch update from the server.
        /// Also raised when there are singular events not part of a group.
        /// </summary>
        event LWRPConnection.BatchUpdateEventArgs<T> OnBatchUpdate;
    }
}
