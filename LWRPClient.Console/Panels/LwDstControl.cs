using LWRPClient.Entities;
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
    public partial class LwDstControl : UserControl
    {
        public LwDstControl()
        {
            InitializeComponent();
        }

        public LwDstControl(ILWRPDestination dst)
        {
            this.dst = dst;
            InitializeComponent();
        }

        private readonly ILWRPDestination dst;
        private bool updating = false;

        public ILWRPDestination Destination => dst;

        private void LwDstControl_Load(object sender, EventArgs e)
        {
            if (dst != null)
            {
                mainGroup.Text = $"Destination #{dst.Index}";
            }
        }

        public void SourceUpdated()
        {
            //Remove events
            updating = true;

            //Decode channel
            LwChannel ch = dst.Channel;

            //Update
            fieldName.Text = dst.Name;
            fieldAddress.Text = dst.Address;
            fieldChannelNum.Value = ch.Type == LwChannelType.INVALID ? 0 : ch.Channel;
            fieldChannelType.SelectedIndex = (int)ch.Type;

            //Clear modified colors
            fieldName.BackColor = SystemColors.Window;
            fieldAddress.BackColor = SystemColors.Window;
            fieldChannelType.BackColor = SystemColors.Window;
            fieldChannelNum.BackColor = SystemColors.Window;

            //Add events again
            updating = false;
        }

        private void FieldName_TextChanged(object sender, EventArgs e)
        {
            if (!updating)
            {
                dst.Name = fieldName.Text;
                fieldName.BackColor = Color.Yellow;
            }
        }

        private void FieldAddress_TextChanged(object sender, EventArgs e)
        {
            if (!updating)
            {
                dst.Address = fieldAddress.Text;
                fieldAddress.BackColor = Color.Yellow;
            }
        }

        private void fieldChannelType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChannelUpdated();
            fieldChannelNum.Visible = fieldChannelType.SelectedIndex > 0;
        }

        private void fieldChannelNum_ValueChanged(object sender, EventArgs e)
        {
            ChannelUpdated();
        }

        private void ChannelUpdated()
        {
            if (!updating)
            {
                dst.Channel = new LwChannel((LwChannelType)fieldChannelType.SelectedIndex, (ushort)fieldChannelNum.Value);
                fieldChannelType.BackColor = Color.Yellow;
                fieldChannelNum.BackColor = Color.Yellow;
            }
        }
    }
}
