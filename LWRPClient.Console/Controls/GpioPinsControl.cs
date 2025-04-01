using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LWRPClient.Console.Controls
{
    public partial class GpioPinsControl : Control
    {
        public delegate void PinUpdatedEvent(GpioPinsControl control, int index, LWRPPinState state);

        public GpioPinsControl()
        {
            pinsAdapter = new PinsAdapter(this);
            DoubleBuffered = true;
            InitializeComponent();
            RefreshCursor();
        }

        private Color highColor = Color.White;
        private Color lowColor = Color.Red;
        private readonly LWRPPinState[] pins = new LWRPPinState[5];
        private ILWRPPins pinsAdapter;
        private bool readOnly;

        /// <summary>
        /// Event raised when a user updates a pin.
        /// </summary>
        public event PinUpdatedEvent PinUpdated;

        public ILWRPPins Pins => pinsAdapter;

        /// <summary>
        /// Sets all pins at once.
        /// </summary>
        public void SetPins(LWRPPinState[] pins)
        {
            if (pins.Length != 5)
                throw new Exception("Expected exactly 5 pins.");
            pins.CopyTo(this.pins, 0);
            Invalidate();
        }

        public bool ReadOnly
        {
            get => readOnly;
            set
            {
                readOnly = value;
                RefreshCursor();
            }
        }

        public Color HighColor
        {
            get => highColor;
            set
            {
                highColor = value;
                Invalidate();
            }
        }

        public Color LowColor
        {
            get => lowColor;
            set
            {
                lowColor = value;
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            //Paint nothing if less than 5 px
            int contentWidth = Width - 2;
            if (contentWidth < 5)
                return;

            //Split width between the 5 pins
            int pinWidth = contentWidth / 5;
            int remainder = contentWidth % 5;

            //Paint each
            int x = 1;
            for (int i = 0; i < 5; i++)
            {
                //Get width of this section
                int secWidth = pinWidth;
                if (remainder > 0)
                {
                    secWidth++;
                    remainder--;
                }

                //Get content rectangle (minus border)
                RectangleF rect = new RectangleF(x, 0, i == 4 ? secWidth : (secWidth - 1), Height);

                //Render border
                if (i != 4)
                    pe.Graphics.DrawLine(new Pen(ForeColor), x + (secWidth - 1), 0, x + (secWidth - 1), Height);

                //Get background color
                Color bg = Color.Blue;
                if ((int)pins[i] > 0)
                    bg = highColor;
                if ((int)pins[i] < 0)
                    bg = lowColor;

                //Paint background
                pe.Graphics.FillRectangle(new SolidBrush(bg), rect);

                //Determine text formatting
                string text = "?";
                switch (pins[i])
                {
                    case LWRPPinState.FALLING:
                        text = "L";
                        break;
                    case LWRPPinState.LOW:
                        text = "l";
                        break;
                    case LWRPPinState.HIGH:
                        text = "h";
                        break;
                    case LWRPPinState.RISING:
                        text = "H";
                        break;
                }

                //Render text
                pe.Graphics.DrawString(text, Font, new SolidBrush(ForeColor), rect, new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                });

                //Increment
                x += secWidth;
            }

            //Paint border
            pe.Graphics.DrawRectangle(new Pen(ForeColor), 0, 0, Width - 1, Height - 1);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            //Only proceed if not read-only
            if (!readOnly)
            {
                //Find the selected pin
                int index = e.X / ((Width - 2) / 5);
                if (index < 0)
                    index = 0;
                if (index > 4)
                    index = 4;

                //Get new state
                LWRPPinState newState = LWRPPinState.FALLING;
                switch (pins[index])
                {
                    case LWRPPinState.FALLING:
                    case LWRPPinState.LOW:
                        newState = LWRPPinState.RISING;
                        break;
                    case LWRPPinState.HIGH:
                    case LWRPPinState.RISING:
                        newState = LWRPPinState.FALLING;
                        break;
                }

                //Apply
                pins[index] = newState;
                PinUpdated?.Invoke(this, index, newState);
                Invalidate();
            }
        }

        private void RefreshCursor()
        {
            if (readOnly)
                Cursor = Cursors.Default;
            else
                Cursor = Cursors.Hand;
        }

        class PinsAdapter : ILWRPPins
        {
            public PinsAdapter(GpioPinsControl control)
            {
                this.control = control;
            }

            private readonly GpioPinsControl control;

            public LWRPPinState this[int index]
            {
                get => control.pins[index];
                set
                {
                    control.pins[index] = value;
                    control.Invalidate();
                }
            }

            public bool ReadOnly => control.readOnly;

            public LWRPPinState[] ToArray()
            {
                throw new NotImplementedException();
            }
        }
    }
}
