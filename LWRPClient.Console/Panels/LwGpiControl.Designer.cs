namespace LWRPClient.Console.Panels
{
    partial class LwGpiControl
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
            this.gpiIndex = new System.Windows.Forms.Label();
            this.gpioPinsControl1 = new LWRPClient.Console.Controls.GpioPinsControl();
            this.SuspendLayout();
            // 
            // gpiIndex
            // 
            this.gpiIndex.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gpiIndex.Location = new System.Drawing.Point(0, 0);
            this.gpiIndex.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.gpiIndex.Name = "gpiIndex";
            this.gpiIndex.Size = new System.Drawing.Size(33, 34);
            this.gpiIndex.TabIndex = 1;
            this.gpiIndex.Text = "#";
            this.gpiIndex.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // gpioPinsControl1
            // 
            this.gpioPinsControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gpioPinsControl1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.gpioPinsControl1.HighColor = System.Drawing.Color.White;
            this.gpioPinsControl1.Location = new System.Drawing.Point(36, 0);
            this.gpioPinsControl1.LowColor = System.Drawing.Color.Red;
            this.gpioPinsControl1.Name = "gpioPinsControl1";
            this.gpioPinsControl1.ReadOnly = false;
            this.gpioPinsControl1.Size = new System.Drawing.Size(431, 34);
            this.gpioPinsControl1.TabIndex = 0;
            this.gpioPinsControl1.Text = "gpioPinsControl1";
            // 
            // LwGpiControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gpiIndex);
            this.Controls.Add(this.gpioPinsControl1);
            this.Name = "LwGpiControl";
            this.Size = new System.Drawing.Size(467, 37);
            this.Load += new System.EventHandler(this.LwGpiControl_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.GpioPinsControl gpioPinsControl1;
        private System.Windows.Forms.Label gpiIndex;
    }
}
