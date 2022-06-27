using AxKHOpenAPILib;

using Newtonsoft.Json;

using System.ComponentModel;
using System.Diagnostics;

namespace ShareInvest.Kiwoom
{
    partial class OpenAPI
    {
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Pipe is not null && server.IsConnected)
                {
                    if (Process.GetProcessesByName(nameof(OpenAPI)[^3..]).Length > 0)
                        Pipe?.WriteLine(JsonConvert.SerializeObject(new Interface.Hermes
                        {
                            Method = Interface.Method.Exit,
                            Parameter = string.Empty
                        }));
                    Pipe?.Close();
                    Pipe?.Dispose();
                    Pipe = null;
                }
                if (server is not null)
                {
                    if (server.IsConnected)
                        server.Disconnect();

                    server.Close();
                    server.Dispose();
                }
                if (axAPI is not null)
                {
                    Controls.Remove(axAPI);
                    axAPI.Dispose();
                    axAPI = null;
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
            components = new Container();
            this.resources = new ComponentResourceManager(typeof(OpenAPI));
            this.axAPI = new AxKHOpenAPILib.AxKHOpenAPI();
            ((ISupportInitialize)(this.axAPI)).BeginInit();
            this.SuspendLayout();
            this.axAPI.Location = new System.Drawing.Point(0, 0);
            this.axAPI.Dock = System.Windows.Forms.DockStyle.Fill;
            this.axAPI.Enabled = true;
            this.axAPI.Margin = new System.Windows.Forms.Padding(0);
            this.axAPI.Name = "axAPI";
            this.axAPI.OcxState = (System.Windows.Forms.AxHost.State)resources.GetObject("axAPI.OcxState");
            this.axAPI.Size = new System.Drawing.Size(188, 39);
            this.axAPI.TabIndex = 0;
            this.axAPI.TabStop = false;
            this.axAPI.Visible = false;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Transparent;
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "OpenAPI";
            this.Controls.Add(this.axAPI);
            ((ISupportInitialize)(this.axAPI)).EndInit();
            this.ResumeLayout(false);
        }
        AxKHOpenAPI axAPI;
        ComponentResourceManager resources;
        IContainer components = null;
    }
}