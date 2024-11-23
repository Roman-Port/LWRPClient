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
    public partial class LwSourceControl : UserControl
    {
        public LwSourceControl()
        {
            InitializeComponent();
        }

        public LwSourceControl(ILWRPSource src)
        {
            this.src = src;
            InitializeComponent();
        }

        private readonly ILWRPSource src;

        public ILWRPSource Source => src;

        private void LwSourceControl_Load(object sender, EventArgs e)
        {
            if (src != null)
            {
                mainGroup.Text = $"Source #{src.Index}";
            }
        }

        public void SourceUpdated()
        {
            //Remove events
            fieldEnabled.CheckedChanged -= FieldEnabled_CheckedChanged;
            fieldName.TextChanged -= FieldName_TextChanged;
            fieldAddress.TextChanged -= FieldAddress_TextChanged;

            //Update
            fieldEnabled.Checked = src.RtpStreamEnabled;
            fieldName.Text = src.PrimarySourceName;
            fieldAddress.Text = src.RtpStreamAddress;

            //Clear modified colors
            fieldEnabled.BackColor = SystemColors.Control;
            fieldName.BackColor = SystemColors.Window;
            fieldAddress.BackColor = SystemColors.Window;

            //Add events again
            fieldEnabled.CheckedChanged += FieldEnabled_CheckedChanged;
            fieldName.TextChanged += FieldName_TextChanged;
            fieldAddress.TextChanged += FieldAddress_TextChanged;
        }

        private void FieldEnabled_CheckedChanged(object sender, EventArgs e)
        {
            src.RtpStreamEnabled = fieldEnabled.Checked;
            fieldEnabled.BackColor = Color.Yellow;
        }

        private void FieldName_TextChanged(object sender, EventArgs e)
        {
            src.PrimarySourceName = fieldName.Text;
            fieldName.BackColor = Color.Yellow;
        }

        private void FieldAddress_TextChanged(object sender, EventArgs e)
        {
            src.RtpStreamAddress = fieldAddress.Text;
            fieldAddress.BackColor = Color.Yellow;
        }
    }
}
