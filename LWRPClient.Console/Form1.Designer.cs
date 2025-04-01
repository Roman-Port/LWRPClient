namespace LWRPClient.Console
{
    partial class Form1
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.btnConnect = new System.Windows.Forms.Button();
            this.ipBox = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.infoDeviceName = new System.Windows.Forms.Label();
            this.infoDeviceModel = new System.Windows.Forms.Label();
            this.infoLwrpVer = new System.Windows.Forms.Label();
            this.infoDeviceVer = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.infoSrcNum = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.infoDstNum = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.infoGpiNum = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.infoGpoNum = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.sourcesPanel = new System.Windows.Forms.Panel();
            this.btnApply = new System.Windows.Forms.Button();
            this.dstPanel = new System.Windows.Forms.Panel();
            this.statusLabel = new System.Windows.Forms.Label();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.sourcesTab = new System.Windows.Forms.TabPage();
            this.destinationsTab = new System.Windows.Forms.TabPage();
            this.gpiTab = new System.Windows.Forms.TabPage();
            this.gpiPanel = new System.Windows.Forms.Panel();
            this.gpoTab = new System.Windows.Forms.TabPage();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.sourcesTab.SuspendLayout();
            this.destinationsTab.SuspendLayout();
            this.gpiTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnConnect
            // 
            this.btnConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConnect.Location = new System.Drawing.Point(430, 12);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(111, 23);
            this.btnConnect.TabIndex = 0;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // ipBox
            // 
            this.ipBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ipBox.Location = new System.Drawing.Point(130, 14);
            this.ipBox.Name = "ipBox";
            this.ipBox.Size = new System.Drawing.Size(294, 20);
            this.ipBox.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.tableLayoutPanel1);
            this.groupBox1.Location = new System.Drawing.Point(12, 40);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(529, 78);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Device Info";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.infoDeviceName, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.infoDeviceModel, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.infoLwrpVer, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.infoDeviceVer, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.infoSrcNum, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label4, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.infoDstNum, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.label10, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.infoGpiNum, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.label12, 3, 2);
            this.tableLayoutPanel1.Controls.Add(this.infoGpoNum, 3, 3);
            this.tableLayoutPanel1.Controls.Add(this.label14, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label15, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.label16, 3, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(6, 19);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(517, 53);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Device Name";
            // 
            // infoDeviceName
            // 
            this.infoDeviceName.AutoSize = true;
            this.infoDeviceName.Location = new System.Drawing.Point(3, 13);
            this.infoDeviceName.Name = "infoDeviceName";
            this.infoDeviceName.Size = new System.Drawing.Size(13, 13);
            this.infoDeviceName.TabIndex = 4;
            this.infoDeviceName.Text = "?";
            // 
            // infoDeviceModel
            // 
            this.infoDeviceModel.AutoSize = true;
            this.infoDeviceModel.Location = new System.Drawing.Point(132, 13);
            this.infoDeviceModel.Name = "infoDeviceModel";
            this.infoDeviceModel.Size = new System.Drawing.Size(13, 13);
            this.infoDeviceModel.TabIndex = 5;
            this.infoDeviceModel.Text = "?";
            // 
            // infoLwrpVer
            // 
            this.infoLwrpVer.AutoSize = true;
            this.infoLwrpVer.Location = new System.Drawing.Point(261, 13);
            this.infoLwrpVer.Name = "infoLwrpVer";
            this.infoLwrpVer.Size = new System.Drawing.Size(13, 13);
            this.infoLwrpVer.TabIndex = 6;
            this.infoLwrpVer.Text = "?";
            // 
            // infoDeviceVer
            // 
            this.infoDeviceVer.AutoSize = true;
            this.infoDeviceVer.Location = new System.Drawing.Point(390, 13);
            this.infoDeviceVer.Name = "infoDeviceVer";
            this.infoDeviceVer.Size = new System.Drawing.Size(13, 13);
            this.infoDeviceVer.TabIndex = 7;
            this.infoDeviceVer.Text = "?";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Src #";
            // 
            // infoSrcNum
            // 
            this.infoSrcNum.AutoSize = true;
            this.infoSrcNum.Location = new System.Drawing.Point(3, 39);
            this.infoSrcNum.Name = "infoSrcNum";
            this.infoSrcNum.Size = new System.Drawing.Size(13, 13);
            this.infoSrcNum.TabIndex = 9;
            this.infoSrcNum.Text = "?";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(132, 26);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Dst #";
            // 
            // infoDstNum
            // 
            this.infoDstNum.AutoSize = true;
            this.infoDstNum.Location = new System.Drawing.Point(132, 39);
            this.infoDstNum.Name = "infoDstNum";
            this.infoDstNum.Size = new System.Drawing.Size(13, 13);
            this.infoDstNum.TabIndex = 11;
            this.infoDstNum.Text = "?";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(261, 26);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(40, 13);
            this.label10.TabIndex = 12;
            this.label10.Text = "GPI #";
            // 
            // infoGpiNum
            // 
            this.infoGpiNum.AutoSize = true;
            this.infoGpiNum.Location = new System.Drawing.Point(261, 39);
            this.infoGpiNum.Name = "infoGpiNum";
            this.infoGpiNum.Size = new System.Drawing.Size(13, 13);
            this.infoGpiNum.TabIndex = 13;
            this.infoGpiNum.Text = "?";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(390, 26);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(45, 13);
            this.label12.TabIndex = 14;
            this.label12.Text = "GPO #";
            // 
            // infoGpoNum
            // 
            this.infoGpoNum.AutoSize = true;
            this.infoGpoNum.Location = new System.Drawing.Point(390, 39);
            this.infoGpoNum.Name = "infoGpoNum";
            this.infoGpoNum.Size = new System.Drawing.Size(13, 13);
            this.infoGpoNum.TabIndex = 15;
            this.infoGpoNum.Text = "?";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(132, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(85, 13);
            this.label14.TabIndex = 16;
            this.label14.Text = "Device Model";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(261, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(66, 13);
            this.label15.TabIndex = 17;
            this.label15.Text = "LWRP Ver";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(390, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(70, 13);
            this.label16.TabIndex = 18;
            this.label16.Text = "Device Ver";
            // 
            // sourcesPanel
            // 
            this.sourcesPanel.AutoScroll = true;
            this.sourcesPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sourcesPanel.Location = new System.Drawing.Point(3, 3);
            this.sourcesPanel.Name = "sourcesPanel";
            this.sourcesPanel.Size = new System.Drawing.Size(515, 368);
            this.sourcesPanel.TabIndex = 0;
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.Enabled = false;
            this.btnApply.Location = new System.Drawing.Point(466, 530);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 4;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // dstPanel
            // 
            this.dstPanel.AutoScroll = true;
            this.dstPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dstPanel.Location = new System.Drawing.Point(3, 3);
            this.dstPanel.Name = "dstPanel";
            this.dstPanel.Size = new System.Drawing.Size(515, 368);
            this.dstPanel.TabIndex = 0;
            // 
            // statusLabel
            // 
            this.statusLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.statusLabel.Location = new System.Drawing.Point(12, 14);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(112, 20);
            this.statusLabel.TabIndex = 5;
            this.statusLabel.Text = "DISCONNECTED";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.sourcesTab);
            this.tabControl.Controls.Add(this.destinationsTab);
            this.tabControl.Controls.Add(this.gpiTab);
            this.tabControl.Controls.Add(this.gpoTab);
            this.tabControl.Location = new System.Drawing.Point(12, 124);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(529, 400);
            this.tabControl.TabIndex = 6;
            // 
            // sourcesTab
            // 
            this.sourcesTab.Controls.Add(this.sourcesPanel);
            this.sourcesTab.Location = new System.Drawing.Point(4, 22);
            this.sourcesTab.Name = "sourcesTab";
            this.sourcesTab.Padding = new System.Windows.Forms.Padding(3);
            this.sourcesTab.Size = new System.Drawing.Size(521, 374);
            this.sourcesTab.TabIndex = 0;
            this.sourcesTab.Text = "Sources";
            this.sourcesTab.UseVisualStyleBackColor = true;
            // 
            // destinationsTab
            // 
            this.destinationsTab.Controls.Add(this.dstPanel);
            this.destinationsTab.Location = new System.Drawing.Point(4, 22);
            this.destinationsTab.Name = "destinationsTab";
            this.destinationsTab.Padding = new System.Windows.Forms.Padding(3);
            this.destinationsTab.Size = new System.Drawing.Size(521, 374);
            this.destinationsTab.TabIndex = 1;
            this.destinationsTab.Text = "Destinations";
            this.destinationsTab.UseVisualStyleBackColor = true;
            // 
            // gpiTab
            // 
            this.gpiTab.Controls.Add(this.gpiPanel);
            this.gpiTab.Location = new System.Drawing.Point(4, 22);
            this.gpiTab.Name = "gpiTab";
            this.gpiTab.Size = new System.Drawing.Size(521, 374);
            this.gpiTab.TabIndex = 2;
            this.gpiTab.Text = "GPI";
            this.gpiTab.UseVisualStyleBackColor = true;
            // 
            // gpiPanel
            // 
            this.gpiPanel.AutoScroll = true;
            this.gpiPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gpiPanel.Location = new System.Drawing.Point(0, 0);
            this.gpiPanel.Name = "gpiPanel";
            this.gpiPanel.Size = new System.Drawing.Size(521, 374);
            this.gpiPanel.TabIndex = 0;
            // 
            // gpoTab
            // 
            this.gpoTab.Location = new System.Drawing.Point(4, 22);
            this.gpoTab.Name = "gpoTab";
            this.gpoTab.Size = new System.Drawing.Size(521, 374);
            this.gpoTab.TabIndex = 3;
            this.gpoTab.Text = "GPO";
            this.gpoTab.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(553, 565);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.ipBox);
            this.Controls.Add(this.btnConnect);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.sourcesTab.ResumeLayout(false);
            this.destinationsTab.ResumeLayout(false);
            this.gpiTab.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.TextBox ipBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label infoDeviceName;
        private System.Windows.Forms.Label infoDeviceModel;
        private System.Windows.Forms.Label infoLwrpVer;
        private System.Windows.Forms.Label infoDeviceVer;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label infoSrcNum;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label infoDstNum;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label infoGpiNum;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label infoGpoNum;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Panel sourcesPanel;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Panel dstPanel;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage sourcesTab;
        private System.Windows.Forms.TabPage destinationsTab;
        private System.Windows.Forms.TabPage gpiTab;
        private System.Windows.Forms.Panel gpiPanel;
        private System.Windows.Forms.TabPage gpoTab;
    }
}

