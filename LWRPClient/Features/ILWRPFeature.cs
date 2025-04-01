using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LWRPClient.Features
{
    /// <summary>
    /// A component of the client.
    /// </summary>
    public interface ILWRPFeature
    {
        /// <summary>
        /// Returns a task that can be awaited until the feature is ready.
        /// </summary>
        /// <returns></returns>
        Task WaitForReadyAsync();

        /// <summary>
        /// Returns a task that can be awaited until the feature is ready with a timeout.
        /// </summary>
        /// <returns></returns>
        Task WaitForReadyAsync(TimeSpan timeout);
    }
}
