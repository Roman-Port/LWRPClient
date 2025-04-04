﻿using System;
using System.Collections.Generic;
using System.Text;

namespace LWRPClient.Features
{
    /// <summary>
    /// The feature for GPI (to network).
    /// </summary>
    public interface ILWRPGpiFeature : ILWRPBatchUpdateFeature<ILWRPGpiPort>
    {
        /// <summary>
        /// The number of GPIs available.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets a port by index (starting at 0).
        /// </summary>
        ILWRPGpiPort this[int index]
        {
            get;
        }
    }
}
