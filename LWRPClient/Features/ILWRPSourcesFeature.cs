using System;
using System.Collections.Generic;
using System.Text;

namespace LWRPClient.Features
{
    public interface ILWRPSourcesFeature : ILWRPBatchUpdateFeature<ILWRPSource>
    {
        /// <summary>
        /// The number of sources available.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Accesses a source by index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        ILWRPSource this[int index]
        {
            get;
        }
    }
}
