namespace RWSpreader
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
            this.pgInstrumentA = new System.Windows.Forms.PropertyGrid();
            this.pgInstrumentB = new System.Windows.Forms.PropertyGrid();
            this.btnShiftUpB = new System.Windows.Forms.Button();
            this.btnShiftDownB = new System.Windows.Forms.Button();
            this.btnShiftDownA = new System.Windows.Forms.Button();
            this.btnShiftUpA = new System.Windows.Forms.Button();
            this.listView1 = new System.Windows.Forms.ListView();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.pgSpread = new System.Windows.Forms.PropertyGrid();
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
            this.pgInstrumentB.Location = new System.Drawing.Point(345, 12);
            this.pgInstrumentB.Name = "pgInstrumentB";
            this.pgInstrumentB.Size = new System.Drawing.Size(130, 130);
            this.pgInstrumentB.TabIndex = 1;
            this.pgInstrumentB.ToolbarVisible = false;
            // 
            // btnShiftUpB
            // 
            this.btnShiftUpB.Location = new System.Drawing.Point(400, 148);
            this.btnShiftUpB.Name = "btnShiftUpB";
            this.btnShiftUpB.Size = new System.Drawing.Size(75, 23);
            this.btnShiftUpB.TabIndex = 2;
            this.btnShiftUpB.Text = "ShiftUp";
            this.btnShiftUpB.UseVisualStyleBackColor = true;
            // 
            // btnShiftDownB
            // 
            this.btnShiftDownB.Location = new System.Drawing.Point(400, 177);
            this.btnShiftDownB.Name = "btnShiftDownB";
            this.btnShiftDownB.Size = new System.Drawing.Size(75, 23);
            this.btnShiftDownB.TabIndex = 3;
            this.btnShiftDownB.Text = "ShiftDown";
            this.btnShiftDownB.UseVisualStyleBackColor = true;
            // 
            // btnShiftDownA
            // 
            this.btnShiftDownA.Location = new System.Drawing.Point(12, 177);
            this.btnShiftDownA.Name = "btnShiftDownA";
            this.btnShiftDownA.Size = new System.Drawing.Size(75, 23);
            this.btnShiftDownA.TabIndex = 5;
            this.btnShiftDownA.Text = "ShiftDown";
            this.btnShiftDownA.UseVisualStyleBackColor = true;
            // 
            // btnShiftUpA
            // 
            this.btnShiftUpA.Location = new System.Drawing.Point(12, 148);
            this.btnShiftUpA.Name = "btnShiftUpA";
            this.btnShiftUpA.Size = new System.Drawing.Size(75, 23);
            this.btnShiftUpA.TabIndex = 4;
            this.btnShiftUpA.Text = "ShiftUp";
            this.btnShiftUpA.UseVisualStyleBackColor = true;
            // 
            // listView1
            // 
            this.listView1.Location = new System.Drawing.Point(21, 289);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(454, 97);
            this.listView1.TabIndex = 6;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(204, 12);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
            this.btnConnect.TabIndex = 7;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(204, 41);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 8;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // pgSpread
            // 
            this.pgSpread.HelpVisible = false;
            this.pgSpread.Location = new System.Drawing.Point(179, 70);
            this.pgSpread.Name = "pgSpread";
            this.pgSpread.Size = new System.Drawing.Size(130, 130);
            this.pgSpread.TabIndex = 9;
            this.pgSpread.ToolbarVisible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(487, 398);
            this.Controls.Add(this.pgSpread);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.btnShiftDownA);
            this.Controls.Add(this.btnShiftUpA);
            this.Controls.Add(this.btnShiftDownB);
            this.Controls.Add(this.btnShiftUpB);
            this.Controls.Add(this.pgInstrumentB);
            this.Controls.Add(this.pgInstrumentA);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PropertyGrid pgInstrumentA;
        private System.Windows.Forms.PropertyGrid pgInstrumentB;
        private System.Windows.Forms.Button btnShiftUpB;
        private System.Windows.Forms.Button btnShiftDownB;
        private System.Windows.Forms.Button btnShiftDownA;
        private System.Windows.Forms.Button btnShiftUpA;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.PropertyGrid pgSpread;
    }
}

