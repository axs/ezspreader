namespace EZSpreader
{
    partial class EZSpreader
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
            this.pgInstrumentA = new System.Windows.Forms.PropertyGrid();
            this.pgInstrumentB = new System.Windows.Forms.PropertyGrid();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.pgSpread = new System.Windows.Forms.PropertyGrid();
            this.btnBuy = new System.Windows.Forms.Button();
            this.Sellbtn = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnApplyStrat = new System.Windows.Forms.Button();
            this.btnOpen = new System.Windows.Forms.Button();
            this.textBoxBid = new System.Windows.Forms.TextBox();
            this.textBoxAsk = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // pgInstrumentA
            // 
            this.pgInstrumentA.HelpVisible = false;
            this.pgInstrumentA.Location = new System.Drawing.Point(12, 12);
            this.pgInstrumentA.Name = "pgInstrumentA";
            this.pgInstrumentA.Size = new System.Drawing.Size(130, 130);
            this.pgInstrumentA.TabIndex = 0;
            this.pgInstrumentA.ToolbarVisible = false;
            // 
            // pgInstrumentB
            // 
            this.pgInstrumentB.HelpVisible = false;
            this.pgInstrumentB.Location = new System.Drawing.Point(288, 12);
            this.pgInstrumentB.Name = "pgInstrumentB";
            this.pgInstrumentB.Size = new System.Drawing.Size(130, 130);
            this.pgInstrumentB.TabIndex = 1;
            this.pgInstrumentB.ToolbarVisible = false;
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(12, 148);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(62, 23);
            this.btnConnect.TabIndex = 7;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnStart
            // 
            this.btnStart.Enabled = false;
            this.btnStart.Location = new System.Drawing.Point(12, 177);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(62, 23);
            this.btnStart.TabIndex = 8;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // pgSpread
            // 
            this.pgSpread.HelpVisible = false;
            this.pgSpread.Location = new System.Drawing.Point(149, 12);
            this.pgSpread.Name = "pgSpread";
            this.pgSpread.Size = new System.Drawing.Size(133, 130);
            this.pgSpread.TabIndex = 9;
            this.pgSpread.ToolbarVisible = false;
            // 
            // btnBuy
            // 
            this.btnBuy.Location = new System.Drawing.Point(167, 150);
            this.btnBuy.Name = "btnBuy";
            this.btnBuy.Size = new System.Drawing.Size(47, 23);
            this.btnBuy.TabIndex = 10;
            this.btnBuy.Text = "buy";
            this.btnBuy.UseVisualStyleBackColor = true;
            this.btnBuy.Click += new System.EventHandler(this.btnBuy_Click);
            // 
            // Sellbtn
            // 
            this.Sellbtn.Location = new System.Drawing.Point(217, 150);
            this.Sellbtn.Name = "Sellbtn";
            this.Sellbtn.Size = new System.Drawing.Size(47, 23);
            this.Sellbtn.TabIndex = 11;
            this.Sellbtn.Text = "sell";
            this.Sellbtn.UseVisualStyleBackColor = true;
            this.Sellbtn.Click += new System.EventHandler(this.Sellbtn_Click);
            // 
            // btnStop
            // 
            this.btnStop.BackColor = System.Drawing.Color.Red;
            this.btnStop.ForeColor = System.Drawing.Color.Black;
            this.btnStop.Location = new System.Drawing.Point(343, 178);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 12;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = false;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnApplyStrat
            // 
            this.btnApplyStrat.Enabled = false;
            this.btnApplyStrat.Location = new System.Drawing.Point(343, 148);
            this.btnApplyStrat.Name = "btnApplyStrat";
            this.btnApplyStrat.Size = new System.Drawing.Size(75, 23);
            this.btnApplyStrat.TabIndex = 13;
            this.btnApplyStrat.Text = "ApplyStrat";
            this.btnApplyStrat.UseVisualStyleBackColor = true;
            this.btnApplyStrat.Click += new System.EventHandler(this.btnApplyStrat_Click);
            // 
            // btnOpen
            // 
            this.btnOpen.Location = new System.Drawing.Point(80, 149);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(62, 23);
            this.btnOpen.TabIndex = 14;
            this.btnOpen.Text = "Config";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // textBoxBid
            // 
            this.textBoxBid.BackColor = System.Drawing.Color.Black;
            this.textBoxBid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxBid.ForeColor = System.Drawing.Color.Yellow;
            this.textBoxBid.Location = new System.Drawing.Point(149, 180);
            this.textBoxBid.Name = "textBoxBid";
            this.textBoxBid.ReadOnly = true;
            this.textBoxBid.Size = new System.Drawing.Size(65, 20);
            this.textBoxBid.TabIndex = 15;
            // 
            // textBoxAsk
            // 
            this.textBoxAsk.BackColor = System.Drawing.Color.Black;
            this.textBoxAsk.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxAsk.ForeColor = System.Drawing.Color.Yellow;
            this.textBoxAsk.Location = new System.Drawing.Point(217, 180);
            this.textBoxAsk.Name = "textBoxAsk";
            this.textBoxAsk.ReadOnly = true;
            this.textBoxAsk.Size = new System.Drawing.Size(65, 20);
            this.textBoxAsk.TabIndex = 16;
            // 
            // EZSpreader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(430, 204);
            this.Controls.Add(this.textBoxAsk);
            this.Controls.Add(this.textBoxBid);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.btnApplyStrat);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.Sellbtn);
            this.Controls.Add(this.btnBuy);
            this.Controls.Add(this.pgSpread);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.pgInstrumentB);
            this.Controls.Add(this.pgInstrumentA);
            this.ForeColor = System.Drawing.Color.Black;
            this.Name = "EZSpreader";
            this.Text = "EZSpreader";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PropertyGrid pgInstrumentA;
        private System.Windows.Forms.PropertyGrid pgInstrumentB;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.PropertyGrid pgSpread;
        private System.Windows.Forms.Button btnBuy;
        private System.Windows.Forms.Button Sellbtn;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnApplyStrat;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.TextBox textBoxBid;
        private System.Windows.Forms.TextBox textBoxAsk;
    }
}

