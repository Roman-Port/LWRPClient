using System;
using System.Collections.Generic;
using System.Text;

namespace LWRPClient.Features
{
    /// <summary>
    /// The feature for GPO (from network).
    /// </summary>
    public interface ILWRPGpoFeature : ILWRPBatchUpdateFeature<ILWRPGpoPort>
    {
        /// <summary>
        /// The number of GPOs available.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets a port by index (starting at 0).
        /// </summary>
        ILWRPGpoPort this[int index]
        {
            get;
        }
    }
}
