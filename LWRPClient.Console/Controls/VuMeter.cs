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
    public partial class VuMeter : Control
    {
        public VuMeter()
        {
            InitializeComponent();
            DoubleBuffered = true;
        }

        private float maxValue = 0;
        private float minValue = -100;
        private float valueL = -50;
        private float valueR = -50;

        public float MaxValue
        {
            get => maxValue;
            set
            {
                maxValue = value;
                Invalidate();
            }
        }

        public float MinValue
        {
            get => minValue;
            set
            {
                minValue = value;
                Invalidate();
            }
        }

        public float ValueL
        {
            get => valueL;
            set
            {
                valueL = value;
                Invalidate();
            }
        }

        public float ValueR
        {
            get => valueR;
            set
            {
                valueR = value;
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            int half = Width / 2;
            RenderGraph(pe.Graphics, new Rectangle(0, 0, half, Height), valueL);
            RenderGraph(pe.Graphics, new Rectangle(half, 0, Width - half, Height), valueR);
        }

        private void RenderGraph(Graphics g, Rectangle rect, float value)
        {
            //Validate
            if (rect.Width <= 0 || rect.Height <= 0)
                return;

            //Determine how filled this should be
            float fill = (value - minValue) / (maxValue - minValue);

            //Using the height of the rectangle, convert this to fill in pixels
            int fillPx = (int)(fill * rect.Height);

            //Constrain
            if (fillPx > rect.Height)
                fillPx = rect.Height;
            if (fillPx < 0)
                fillPx = 0;

            //Calculate height of unfilled region
            int unfilled = rect.Height - fillPx;

            //Render unfilled part
            if (unfilled > 0)
                g.FillRectangle(new SolidBrush(BackColor), new Rectangle(rect.X, rect.Y, rect.Width, unfilled));

            //Render filled part
            if (fillPx > 0)
                g.FillRectangle(new SolidBrush(ForeColor), new Rectangle(rect.X, rect.Y + unfilled, rect.Width, fillPx));
        }
    }
}
