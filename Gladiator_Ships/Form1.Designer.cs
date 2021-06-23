namespace Gladiator_Ships
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
            this.components = new System.ComponentModel.Container();
            this.picturebox = new System.Windows.Forms.PictureBox();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.restarttimer = new System.Windows.Forms.Timer(this.components);
            this.stopfight = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.picturebox)).BeginInit();
            this.SuspendLayout();
            // 
            // picturebox
            // 
            this.picturebox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picturebox.Location = new System.Drawing.Point(0, 0);
            this.picturebox.Name = "picturebox";
            this.picturebox.Size = new System.Drawing.Size(804, 674);
            this.picturebox.TabIndex = 0;
            this.picturebox.TabStop = false;
            // 
            // timer
            // 
            this.timer.Interval = 16;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // restarttimer
            // 
            this.restarttimer.Interval = 4000;
            this.restarttimer.Tick += new System.EventHandler(this.restarttimer_Tick);
            // 
            // stopfight
            // 
            this.stopfight.Interval = 100000;
            this.stopfight.Tick += new System.EventHandler(this.stopfight_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(804, 674);
            this.Controls.Add(this.picturebox);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.picturebox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox picturebox;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Timer restarttimer;
        private System.Windows.Forms.Timer stopfight;
    }
}

