namespace ShareInvest
{
    partial class Server
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
                if (strip is not null)
                {
                    strip.ItemClicked -= StripItemClicked;
                    strip.Dispose();
                    strip = null;
                }
                if (notifyIcon is not null)
                {
                    if (notifyIcon.Icon is not null)
                    {
                        notifyIcon.Icon.Dispose();
                        notifyIcon.Icon = null;
                    }
                    notifyIcon.Dispose();
                    notifyIcon = null;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Server));
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.strip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.reference = new System.Windows.Forms.ToolStripMenuItem();
            this.exit = new System.Windows.Forms.ToolStripMenuItem();
            this.progress = new System.Windows.Forms.ProgressBar();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.strip.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon
            // 
            this.notifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Warning;
            this.notifyIcon.ContextMenuStrip = this.strip;
            this.notifyIcon.Text = "Algorithmic Trading";
            // 
            // strip
            // 
            this.strip.AllowMerge = false;
            this.strip.AutoSize = false;
            this.strip.DropShadowEnabled = false;
            this.strip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.reference,
            this.exit});
            this.strip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
            this.strip.Name = "strip";
            this.strip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.strip.ShowImageMargin = false;
            this.strip.ShowItemToolTips = false;
            this.strip.Size = new System.Drawing.Size(48, 47);
            this.strip.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.StripItemClicked);
            // 
            // reference
            // 
            this.reference.Name = "reference";
            this.reference.Size = new System.Drawing.Size(73, 22);
            this.reference.Text = "연결";
            // 
            // exit
            // 
            this.exit.Name = "exit";
            this.exit.Size = new System.Drawing.Size(73, 22);
            this.exit.Text = "종료";
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
            // Server
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
            this.Name = "Server";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Algorithmic Trading Installer by ShareInvest Corp.";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.JustBeforeFormClosing);
            this.Resize += new System.EventHandler(this.SecuritiesResize);
            this.strip.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        ContextMenuStrip strip;
        ToolStripMenuItem reference;
        ToolStripMenuItem exit;
        System.ComponentModel.IContainer components = null;
        System.Windows.Forms.Timer timer;
        ProgressBar progress;
        NotifyIcon notifyIcon;
    }
}