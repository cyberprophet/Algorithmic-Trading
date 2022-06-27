namespace ShareInvest
{
    partial class Install
    {
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (timer is not null)
                {
                    timer.Stop();
                    timer.Tick -= TimerTick;
                    timer.Dispose();
                }
                if (Controls.Count > 0)
                {
                    foreach (Control control in Controls)
                        control.Dispose();

                    Controls.Clear();
                }
                if (components is not null)
                {
                    components.Dispose();
                    components = null;
                }
            }
            base.Dispose(disposing);
        }
        void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Install));
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.progress = new System.Windows.Forms.ProgressBar();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // notifyIcon
            // 
            this.notifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Warning;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "Algorithmic Trading";
            // 
            // progress
            // 
            this.progress.BackColor = System.Drawing.SystemColors.Control;
            this.progress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progress.Location = new System.Drawing.Point(0, 0);
            this.progress.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.progress.MarqueeAnimationSpeed = 1000;
            this.progress.Name = "progress";
            this.progress.Size = new System.Drawing.Size(505, 32);
            this.progress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progress.TabIndex = 0;
            // 
            // timer
            // 
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.TimerTick);
            // 
            // Install
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(505, 32);
            this.Controls.Add(this.progress);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Install";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Algorithmic Trading Installer by ShareInvest Corp.";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.JustBeforeFormClosing);
            this.Resize += new System.EventHandler(this.SecuritiesResize);
            this.ResumeLayout(false);

        }
        System.ComponentModel.IContainer components = null;
        System.Windows.Forms.Timer timer;
        ProgressBar progress;
        NotifyIcon notifyIcon;
    }
}