using LWRPClient.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LWRPClient
{
    public interface ILWRPDestination
    {
        /// <summary>
        /// Index on this device starting with 1.
        /// </summary>
        int Index { get; }

        /// <summary>
        /// The name of the destination.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// The current RTP address being recieved on this destination.
        /// </summary>
        string Address { get; set; }

        /// <summary>
        /// The current Livewire channel being recieved, derived from the Address.
        /// </summary>
        LwChannel Channel { get; set; }

        /// <summary>
        /// Channel count.
        /// </summary>
        int ChannelCount { get; set; }
    }
}
