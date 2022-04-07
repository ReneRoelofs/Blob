
namespace BlobTester
{
    partial class BlobTestWriterFormcs
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
            this.rbTestBlob = new System.Windows.Forms.RadioButton();
            this.rbProdBlob = new System.Windows.Forms.RadioButton();
            this.button1 = new System.Windows.Forms.Button();
            this.dtSinceTime = new System.Windows.Forms.DateTimePicker();
            this.dtSince = new System.Windows.Forms.DateTimePicker();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // rbTestBlob
            // 
            this.rbTestBlob.AutoSize = true;
            this.rbTestBlob.Checked = true;
            this.rbTestBlob.Location = new System.Drawing.Point(12, 12);
            this.rbTestBlob.Name = "rbTestBlob";
            this.rbTestBlob.Size = new System.Drawing.Size(46, 17);
            this.rbTestBlob.TabIndex = 3;
            this.rbTestBlob.TabStop = true;
            this.rbTestBlob.Text = "Test";
            this.rbTestBlob.UseVisualStyleBackColor = true;
            // 
            // rbProdBlob
            // 
            this.rbProdBlob.AutoSize = true;
            this.rbProdBlob.Location = new System.Drawing.Point(64, 12);
            this.rbProdBlob.Name = "rbProdBlob";
            this.rbProdBlob.Size = new System.Drawing.Size(47, 17);
            this.rbProdBlob.TabIndex = 4;
            this.rbProdBlob.TabStop = true;
            this.rbProdBlob.Text = "Prod";
            this.rbProdBlob.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(331, 9);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(234, 23);
            this.button1.TabIndex = 13;
            this.button1.Text = "Download some blobs";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.bDownload_Click);
            // 
            // dtSinceTime
            // 
            this.dtSinceTime.Enabled = false;
            this.dtSinceTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtSinceTime.Location = new System.Drawing.Point(240, 10);
            this.dtSinceTime.Name = "dtSinceTime";
            this.dtSinceTime.Size = new System.Drawing.Size(66, 20);
            this.dtSinceTime.TabIndex = 15;
            // 
            // dtSince
            // 
            this.dtSince.Enabled = false;
            this.dtSince.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtSince.Location = new System.Drawing.Point(155, 9);
            this.dtSince.Name = "dtSince";
            this.dtSince.Size = new System.Drawing.Size(79, 20);
            this.dtSince.TabIndex = 14;
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(13, 88);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(775, 350);
            this.textBox1.TabIndex = 16;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(571, 7);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(113, 23);
            this.button2.TabIndex = 17;
            this.button2.Text = "Distribute";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(13, 50);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 18;
            this.button3.Text = "Check 2021";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(298, 50);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 19;
            this.button4.Text = "Check root";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(192, 50);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(100, 20);
            this.textBox2.TabIndex = 20;
            this.textBox2.Text = "2021/10/15";
            // 
            // BlobTestWriterFormcs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.dtSinceTime);
            this.Controls.Add(this.dtSince);
            this.Controls.Add(this.rbTestBlob);
            this.Controls.Add(this.rbProdBlob);
            this.Name = "BlobTestWriterFormcs";
            this.Text = "BlobTestWriterFormcs";
            this.Load += new System.EventHandler(this.BlobTestWriterFormcs_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton rbTestBlob;
        private System.Windows.Forms.RadioButton rbProdBlob;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DateTimePicker dtSinceTime;
        private System.Windows.Forms.DateTimePicker dtSince;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.TextBox textBox2;
    }
}