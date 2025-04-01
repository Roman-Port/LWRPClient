using System;
using System.Collections.Generic;
using System.Text;

namespace LWRPClient.Features
{
    public interface ILWRPDestinationsFeature : ILWRPBatchUpdateFeature<ILWRPDestination>
    {
        /// <summary>
        /// The number of destinations available.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Accesses a destination by index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        ILWRPDestination this[int index]
        {
            get;
        }
    }
}
