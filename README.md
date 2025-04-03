# LWRPClient

LWRPClient is a C# library for communicating with Axia Livewire devices. It allows for reading/writing settings, sources, destinations, GPIs, GPOs, and more. This is the protocol internally used by products such as Pathfinder.

## Supported Features

* Establishing persistent, recoverable connections with devices
* Reading/writing source properties
* Reading/writing destination properties
* Reading GPIs
* Reading/setting/configuring GPOs
* Reading audio levels

## Usage

Create the ``LWRPConnection`` item and pass in the IPAddress of the device as well as the ``LWRPEnabledFeature`` features you want to enable, ORed together with a pipe.

Features you enable dictate what parts of the connection are available. Once the connection is created they can't be changed.

Once created, you should bind events and then call ``Initialize()``. From there, await a call to ``WaitForReadyAsync()`` which will complete when info data about the device is received.

From there, use the library. Any changes you make won't be applied immediately; Instead, after changing properties, call ``SendUpdatesAsync()`` to apply all changes.

For more examples see the ``LWRPClient.Example.OneShot`` project.

## Disclaimer

This project is not associated with Axia, Telos, or anyone else involved in the development of Livewire.
