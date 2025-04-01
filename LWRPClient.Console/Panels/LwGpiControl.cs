using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LWRPClient.Console.Panels
{
    public partial class LwGpiControl : UserControl
    {
        public LwGpiControl(int index)
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

        private void LwGpiControl_Load(object sender, EventArgs e)
        {
            //Set text
            gpiIndex.Text = index.ToString();
        }

        public void SetPins(LWRPPinState[] pins)
        {
            gpioPinsControl1.SetPins(pins);
        }
    }
}
