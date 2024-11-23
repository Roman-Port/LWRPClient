namespace LWRPClient.Console.Panels
{
    partial class LwSourceControl
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
            this.label1 = new System.Windows.Forms.Label();
            this.fieldName = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.fieldAddress = new System.Windows.Forms.TextBox();
            this.fieldEnabled = new System.Windows.Forms.CheckBox();
            this.mainGroup.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
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
            this.mainGroup.Size = new System.Drawing.Size(300, 58);
            this.mainGroup.TabIndex = 0;
            this.mainGroup.TabStop = false;
            this.mainGroup.Text = "groupBox1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(21, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(132, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name";
            // 
            // fieldName
            // 
            this.fieldName.Dock = System.Windows.Forms.DockStyle.Top;
            this.fieldName.Location = new System.Drawing.Point(21, 16);
            this.fieldName.Name = "fieldName";
            this.fieldName.Size = new System.Drawing.Size(132, 20);
            this.fieldName.TabIndex = 1;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.fieldAddress, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.label2, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.fieldName, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.fieldEnabled, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(294, 39);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Location = new System.Drawing.Point(159, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(132, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Address";
            // 
            // fieldAddress
            // 
            this.fieldAddress.Dock = System.Windows.Forms.DockStyle.Top;
            this.fieldAddress.Location = new System.Drawing.Point(159, 16);
            this.fieldAddress.Name = "fieldAddress";
            this.fieldAddress.Size = new System.Drawing.Size(132, 20);
            this.fieldAddress.TabIndex = 3;
            // 
            // fieldEnabled
            // 
            this.fieldEnabled.AutoSize = true;
            this.fieldEnabled.Dock = System.Windows.Forms.DockStyle.Top;
            this.fieldEnabled.Location = new System.Drawing.Point(3, 18);
            this.fieldEnabled.Margin = new System.Windows.Forms.Padding(3, 5, 0, 5);
            this.fieldEnabled.Name = "fieldEnabled";
            this.fieldEnabled.Size = new System.Drawing.Size(15, 14);
            this.fieldEnabled.TabIndex = 4;
            this.fieldEnabled.UseVisualStyleBackColor = true;
            // 
            // LwSourceControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.mainGroup);
            this.MinimumSize = new System.Drawing.Size(300, 0);
            this.Name = "LwSourceControl";
            this.Size = new System.Drawing.Size(300, 58);
            this.Load += new System.EventHandler(this.LwSourceControl_Load);
            this.mainGroup.ResumeLayout(false);
            this.mainGroup.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox mainGroup;
        private System.Windows.Forms.TextBox fieldName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox fieldAddress;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox fieldEnabled;
    }
}
