using LWRPClient.Entities;
using LWRPClient.Exceptions;
using LWRPClient.Layer1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LWRPClient
{
    public partial class LWRPConnection
    {
        public delegate void MeterLevelEventArgs(LWRPConnection conn, MeterChannelReading[] ich, MeterChannelReading[] och);

        /// <summary>
        /// Event raised when meter levels are recieved.
        /// </summary>
        public event MeterLevelEventArgs OnMeterLevelsUpdated;

        /// <summary>
        /// Requests a refresh of the audio meters.
        /// </summary>
        /// <returns></returns>
        public Task RequestMetersAsync()
        {
            //Create the meter command
            LWRPMessage msg = new LWRPMessage("MTR", new LWRPToken[0][]);

            //Send
            return transport.SendMessage(msg);
        }

        /// <summary>
        /// Meter updates recieved since the last group end.
        /// </summary>
        private readonly List<MeterChannelReading> pendingMeterUpdates = new List<MeterChannelReading>();

        /// <summary>
        /// Processes a meter command from the server.
        /// </summary>
        /// <param name="msg"></param>
        private void ProcessMeter(LWRPMessage msg)
        {
            //Check that the required number of arguments exist
            if (msg.Arguments.Length < 3)
                throw new InvalidResponseException("Meter command has too few arguments.", msg);

            //Determine if this is a input channel or output channel
            MeterChannelType type;
            switch (msg.Arguments[0][0].Content)
            {
                case "ICH": type = MeterChannelType.INPUT; break;
                case "OCH": type = MeterChannelType.OUTPUT; break;
                default: throw new InvalidResponseException("Meter command specifies invalid metering type.", msg);
            }

            //Get the meter index from the second argument
            if (!int.TryParse(msg.Arguments[1][0].Content, out int index))
                throw new InvalidResponseException("Meter command specifies invalid channel number.", msg);

            //Parse each meter group
            MeterLevelReading peek = null;
            MeterLevelReading rms = null;
            for (int i = 2; i < msg.Arguments.Length; i++)
            {
                if (ParseMeterLevels(msg.Arguments[i], out string levelType, out MeterLevelReading reading))
                {
                    switch (levelType.ToUpper())
                    {
                        case "PEEK": peek = reading; break;
                        case "RMS": rms = reading; break;
                    }
                }
            }

            //Add to collection
            pendingMeterUpdates.Add(new MeterChannelReading(type, index, peek, rms));
        }

        /// <summary>
        /// Called when a group is started for processing meter channels.
        /// </summary>
        private void MeterProcessBeginGroup()
        {
            //Clear saved items (there shouldn't be any)
            pendingMeterUpdates.Clear();
        }

        /// <summary>
        /// Called when a group is ended for processing meter channels.
        /// </summary>
        private void MeterProcessEndGroup()
        {
            if (pendingMeterUpdates.Count > 0)
            {
                //Break out the meter readings between input and output channels and sort them
                MeterChannelReading[] ich = pendingMeterUpdates.Where(x => x.Type == MeterChannelType.INPUT).OrderBy(x => x.Index).ToArray();
                MeterChannelReading[] och = pendingMeterUpdates.Where(x => x.Type == MeterChannelType.OUTPUT).OrderBy(x => x.Index).ToArray();

                //Clear pending updates
                pendingMeterUpdates.Clear();

                //Fire events
                OnMeterLevelsUpdated?.Invoke(this, ich, och);
            }
        }

        /// <summary>
        /// Parses a PEEK:-1000:-1000 meter level argument
        /// </summary>
        /// <param name="tokens">The input to parse</param>
        /// <param name="type">The name of the reading.</param>
        /// <param name="reading">Value for the reading.</param>
        /// <returns>True on success, otherwise false.</returns>
        private bool ParseMeterLevels(LWRPToken[] tokens, out string type, out MeterLevelReading reading)
        {
            //Clear
            type = null;
            reading = null;

            //Validate the number of tokens is valid
            if (tokens.Length != 3)
                return false;

            //Set type from first token
            type = tokens[0].Content;

            //Parse out values for left and right channels;
            if (!float.TryParse(tokens[1].Content, out float l) || !float.TryParse(tokens[2].Content, out float r))
                return false;

            //Scale
            l /= 10;
            r /= 10;

            //Set up return
            reading = new MeterLevelReading(l, r);
            return true;
        }
    }
}
