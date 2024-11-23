namespace LWRPClient.Console.Panels
{
    partial class LwDstControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mainGroup = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.fieldAddress = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.fieldName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.fieldChannelType = new System.Windows.Forms.ComboBox();
            this.fieldChannelNum = new System.Windows.Forms.NumericUpDown();
            this.mainGroup.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fieldChannelNum)).BeginInit();
            this.SuspendLayout();
            // 
            // mainGroup
            // 
            this.mainGroup.AutoSize = true;
            this.mainGroup.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.mainGroup.Controls.Add(this.tableLayoutPanel1);
            this.mainGroup.Dock = System.Windows.Forms.DockStyle.Top;
            this.mainGroup.Location = new System.Drawing.Point(0, 0);
            this.mainGroup.Name = "mainGroup";
            this.mainGroup.Size = new System.Drawing.Size(400, 59);
            this.mainGroup.TabIndex = 1;
            this.mainGroup.TabStop = false;
            this.mainGroup.Text = "groupBox1";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30.76923F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 23.07692F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 23.07692F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 23.07692F));
            this.tableLayoutPanel1.Controls.Add(this.fieldAddress, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.fieldName, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label3, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.fieldChannelType, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.fieldChannelNum, 3, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(394, 40);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // fieldAddress
            // 
            this.fieldAddress.Dock = System.Windows.Forms.DockStyle.Top;
            this.fieldAddress.Location = new System.Drawing.Point(124, 16);
            this.fieldAddress.Name = "fieldAddress";
            this.fieldAddress.Size = new System.Drawing.Size(84, 20);
            this.fieldAddress.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Location = new System.Drawing.Point(124, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Address";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(115, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name";
            // 
            // fieldName
            // 
            this.fieldName.Dock = System.Windows.Forms.DockStyle.Top;
            this.fieldName.Location = new System.Drawing.Point(3, 16);
            this.fieldName.Name = "fieldName";
            this.fieldName.Size = new System.Drawing.Size(115, 20);
            this.fieldName.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.label3, 2);
            this.label3.Location = new System.Drawing.Point(214, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Channel";
            // 
            // fieldChannelType
            // 
            this.fieldChannelType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fieldChannelType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fieldChannelType.FormattingEnabled = true;
            this.fieldChannelType.Items.AddRange(new object[] {
            "(invalid)",
            "From Source",
            "To Source"});
            this.fieldChannelType.Location = new System.Drawing.Point(214, 16);
            this.fieldChannelType.Name = "fieldChannelType";
            this.fieldChannelType.Size = new System.Drawing.Size(84, 21);
            this.fieldChannelType.TabIndex = 6;
            this.fieldChannelType.SelectedIndexChanged += new System.EventHandler(this.fieldChannelType_SelectedIndexChanged);
            // 
            // fieldChannelNum
            // 
            this.fieldChannelNum.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fieldChannelNum.Location = new System.Drawing.Point(304, 16);
            this.fieldChannelNum.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.fieldChannelNum.Name = "fieldChannelNum";
            this.fieldChannelNum.Size = new System.Drawing.Size(87, 20);
            this.fieldChannelNum.TabIndex = 7;
            this.fieldChannelNum.ValueChanged += new System.EventHandler(this.fieldChannelNum_ValueChanged);
            // 
            // LwDstControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.mainGroup);
            this.MinimumSize = new System.Drawing.Size(400, 0);
            this.Name = "LwDstControl";
            this.Size = new System.Drawing.Size(400, 59);
            this.Load += new System.EventHandler(this.LwDstControl_Load);
            this.mainGroup.ResumeLayout(false);
            this.mainGroup.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fieldChannelNum)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox mainGroup;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox fieldAddress;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox fieldName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox fieldChannelType;
        private System.Windows.Forms.NumericUpDown fieldChannelNum;
    }
}
