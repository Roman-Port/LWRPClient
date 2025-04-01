using LWRPClient.Console.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LWRPClient.Console.Panels
{
    public partial class LwGpoControl : UserControl
    {
        public delegate void PinUpdatedEvent(LwGpoControl control, int index, LWRPPinState state);

        public LwGpoControl(int index)
        {
            this.index = index;
            InitializeComponent();
        }

        private readonly int index;

        public int Index => index;

        public bool ReadOnly
        {
            get => gpioPinsControl1.ReadOnly;
            set => gpioPinsControl1.ReadOnly = value;
        }

        /// <summary>
        /// Event raised when a user updates a pin.
        /// </summary>
        public event PinUpdatedEvent PinUpdated;

        private void LwGpoControl_Load(object sender, EventArgs e)
        {
            //Set text
            gpoIndex.Text = index.ToString();

            //Bind events
            gpioPinsControl1.PinUpdated += GpioPinsControl1_PinUpdated;
        }

        private void GpioPinsControl1_PinUpdated(GpioPinsControl control, int index, LWRPPinState state)
        {
            PinUpdated?.Invoke(this, index, state);
        }

        public void SetPins(LWRPPinState[] pins)
        {
            gpioPinsControl1.SetPins(pins);
        }
    }
}
