namespace ShareInvest
{
    partial class CoreAPI
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
                    timer = null;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CoreAPI));
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.strip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exit = new System.Windows.Forms.ToolStripMenuItem();
            this.reference = new System.Windows.Forms.ToolStripMenuItem();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.strip.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon
            // 
            this.notifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.notifyIcon.ContextMenuStrip = this.strip;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "Algorithmic Trading";
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
            // timer
            // 
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.TimerTick);
            // 
            // CoreAPI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(239, 0);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CoreAPI";
            this.Opacity = 0.15D;
            this.Text = "Algorithmic Trading";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.JustBeforeFormClosing);
            this.Resize += new System.EventHandler(this.SecuritiesResize);
            this.strip.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        ContextMenuStrip strip;
        ToolStripMenuItem reference;
        ToolStripMenuItem exit;
        NotifyIcon notifyIcon;
        System.Windows.Forms.Timer timer;
        System.ComponentModel.IContainer components = null;
    }
}