﻿namespace OpenStreetMapStaticMaps
{
    partial class MainWindow
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            picMapImage = new PictureBox();
            txtZoom = new TextBox();
            txtLon1 = new TextBox();
            txtLat1 = new TextBox();
            btnDrawIt = new Button();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            txtLat2 = new TextBox();
            txtLon2 = new TextBox();
            txtLon3 = new TextBox();
            txtLat3 = new TextBox();
            txtLon4 = new TextBox();
            txtLat4 = new TextBox();
            txtLon5 = new TextBox();
            txtLat5 = new TextBox();
            txtLon6 = new TextBox();
            txtLat6 = new TextBox();
            txtLon7 = new TextBox();
            txtLat7 = new TextBox();
            flowLayoutPanel1 = new FlowLayoutPanel();
            label5 = new Label();
            label6 = new Label();
            txtMapWidth = new TextBox();
            txtMapHeight = new TextBox();
            label1 = new Label();
            numericMaxZoom = new NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)picMapImage).BeginInit();
            flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericMaxZoom).BeginInit();
            SuspendLayout();
            // 
            // picMapImage
            // 
            picMapImage.Anchor = AnchorStyles.None;
            picMapImage.Location = new Point(3, 3);
            picMapImage.Name = "picMapImage";
            picMapImage.Size = new Size(75, 41);
            picMapImage.SizeMode = PictureBoxSizeMode.AutoSize;
            picMapImage.TabIndex = 0;
            picMapImage.TabStop = false;
            // 
            // txtZoom
            // 
            txtZoom.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            txtZoom.Enabled = false;
            txtZoom.Location = new Point(79, 367);
            txtZoom.Name = "txtZoom";
            txtZoom.Size = new Size(50, 23);
            txtZoom.TabIndex = 0;
            // 
            // txtLon1
            // 
            txtLon1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            txtLon1.Location = new Point(79, 425);
            txtLon1.Name = "txtLon1";
            txtLon1.Size = new Size(78, 23);
            txtLon1.TabIndex = 5;
            txtLon1.Text = "-122.09056272817681";
            // 
            // txtLat1
            // 
            txtLat1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            txtLat1.Location = new Point(79, 396);
            txtLat1.Name = "txtLat1";
            txtLat1.Size = new Size(78, 23);
            txtLat1.TabIndex = 4;
            txtLat1.Text = "37.387879045618661";
            // 
            // btnDrawIt
            // 
            btnDrawIt.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnDrawIt.Location = new Point(12, 454);
            btnDrawIt.Name = "btnDrawIt";
            btnDrawIt.Size = new Size(650, 23);
            btnDrawIt.TabIndex = 18;
            btnDrawIt.Text = "Draw It";
            btnDrawIt.UseVisualStyleBackColor = true;
            btnDrawIt.Click += btnDrawIt_Click;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label2.AutoSize = true;
            label2.Location = new Point(34, 370);
            label2.Name = "label2";
            label2.Size = new Size(39, 15);
            label2.TabIndex = 7;
            label2.Text = "Zoom";
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label3.AutoSize = true;
            label3.Location = new Point(12, 428);
            label3.Name = "label3";
            label3.Size = new Size(61, 15);
            label3.TabIndex = 8;
            label3.Text = "Longitude";
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label4.AutoSize = true;
            label4.Location = new Point(23, 399);
            label4.Name = "label4";
            label4.Size = new Size(50, 15);
            label4.TabIndex = 9;
            label4.Text = "Latitude";
            // 
            // txtLat2
            // 
            txtLat2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            txtLat2.Location = new Point(163, 396);
            txtLat2.Name = "txtLat2";
            txtLat2.Size = new Size(78, 23);
            txtLat2.TabIndex = 6;
            txtLat2.Text = "37.391793559006139";
            // 
            // txtLon2
            // 
            txtLon2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            txtLon2.Location = new Point(163, 425);
            txtLon2.Name = "txtLon2";
            txtLon2.Size = new Size(78, 23);
            txtLon2.TabIndex = 7;
            txtLon2.Text = "-122.09128697590819";
            // 
            // txtLon3
            // 
            txtLon3.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            txtLon3.Location = new Point(247, 425);
            txtLon3.Name = "txtLon3";
            txtLon3.Size = new Size(78, 23);
            txtLon3.TabIndex = 9;
            txtLon3.Text = "-122.07572972671227";
            // 
            // txtLat3
            // 
            txtLat3.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            txtLat3.Location = new Point(247, 396);
            txtLat3.Name = "txtLat3";
            txtLat3.Size = new Size(78, 23);
            txtLat3.TabIndex = 8;
            txtLat3.Text = "37.383503387444222";
            // 
            // txtLon4
            // 
            txtLon4.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            txtLon4.Location = new Point(331, 425);
            txtLon4.Name = "txtLon4";
            txtLon4.Size = new Size(78, 23);
            txtLon4.TabIndex = 11;
            // 
            // txtLat4
            // 
            txtLat4.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            txtLat4.Location = new Point(331, 396);
            txtLat4.Name = "txtLat4";
            txtLat4.Size = new Size(78, 23);
            txtLat4.TabIndex = 10;
            // 
            // txtLon5
            // 
            txtLon5.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            txtLon5.Location = new Point(415, 425);
            txtLon5.Name = "txtLon5";
            txtLon5.Size = new Size(78, 23);
            txtLon5.TabIndex = 13;
            // 
            // txtLat5
            // 
            txtLat5.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            txtLat5.Location = new Point(415, 396);
            txtLat5.Name = "txtLat5";
            txtLat5.Size = new Size(78, 23);
            txtLat5.TabIndex = 12;
            // 
            // txtLon6
            // 
            txtLon6.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            txtLon6.Location = new Point(499, 425);
            txtLon6.Name = "txtLon6";
            txtLon6.Size = new Size(78, 23);
            txtLon6.TabIndex = 15;
            // 
            // txtLat6
            // 
            txtLat6.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            txtLat6.Location = new Point(499, 396);
            txtLat6.Name = "txtLat6";
            txtLat6.Size = new Size(78, 23);
            txtLat6.TabIndex = 14;
            // 
            // txtLon7
            // 
            txtLon7.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            txtLon7.Location = new Point(583, 425);
            txtLon7.Name = "txtLon7";
            txtLon7.Size = new Size(78, 23);
            txtLon7.TabIndex = 17;
            // 
            // txtLat7
            // 
            txtLat7.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            txtLat7.Location = new Point(583, 396);
            txtLat7.Name = "txtLat7";
            txtLat7.Size = new Size(78, 23);
            txtLat7.TabIndex = 16;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            flowLayoutPanel1.AutoScroll = true;
            flowLayoutPanel1.BorderStyle = BorderStyle.Fixed3D;
            flowLayoutPanel1.Controls.Add(picMapImage);
            flowLayoutPanel1.Location = new Point(12, 12);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(649, 349);
            flowLayoutPanel1.TabIndex = 22;
            // 
            // label5
            // 
            label5.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label5.AutoSize = true;
            label5.Location = new Point(370, 370);
            label5.Name = "label5";
            label5.Size = new Size(39, 15);
            label5.TabIndex = 24;
            label5.Text = "Width";
            // 
            // label6
            // 
            label6.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label6.AutoSize = true;
            label6.Location = new Point(534, 370);
            label6.Name = "label6";
            label6.Size = new Size(43, 15);
            label6.TabIndex = 25;
            label6.Text = "Height";
            // 
            // txtMapWidth
            // 
            txtMapWidth.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            txtMapWidth.Location = new Point(415, 367);
            txtMapWidth.Name = "txtMapWidth";
            txtMapWidth.Size = new Size(78, 23);
            txtMapWidth.TabIndex = 2;
            txtMapWidth.Text = "456";
            // 
            // txtMapHeight
            // 
            txtMapHeight.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            txtMapHeight.Location = new Point(583, 367);
            txtMapHeight.Name = "txtMapHeight";
            txtMapHeight.Size = new Size(78, 23);
            txtMapHeight.TabIndex = 3;
            txtMapHeight.Text = "343";
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label1.AutoSize = true;
            label1.Location = new Point(179, 369);
            label1.Name = "label1";
            label1.Size = new Size(62, 15);
            label1.TabIndex = 28;
            label1.Text = "MaxZoom";
            // 
            // numericMaxZoom
            // 
            numericMaxZoom.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            numericMaxZoom.Location = new Point(247, 367);
            numericMaxZoom.Maximum = new decimal(new int[] { 19, 0, 0, 0 });
            numericMaxZoom.Minimum = new decimal(new int[] { 8, 0, 0, 0 });
            numericMaxZoom.Name = "numericMaxZoom";
            numericMaxZoom.Size = new Size(78, 23);
            numericMaxZoom.TabIndex = 1;
            numericMaxZoom.Value = new decimal(new int[] { 14, 0, 0, 0 });
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(674, 489);
            Controls.Add(numericMaxZoom);
            Controls.Add(label1);
            Controls.Add(txtMapHeight);
            Controls.Add(txtMapWidth);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(flowLayoutPanel1);
            Controls.Add(txtLon7);
            Controls.Add(txtLat7);
            Controls.Add(txtLon6);
            Controls.Add(txtLat6);
            Controls.Add(txtLon5);
            Controls.Add(txtLat5);
            Controls.Add(txtLon4);
            Controls.Add(txtLat4);
            Controls.Add(txtLon3);
            Controls.Add(txtLat3);
            Controls.Add(txtLon2);
            Controls.Add(txtLat2);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(btnDrawIt);
            Controls.Add(txtLat1);
            Controls.Add(txtLon1);
            Controls.Add(txtZoom);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Name = "MainWindow";
            SizeGripStyle = SizeGripStyle.Hide;
            Text = "MapImageDisplayer";
            ((System.ComponentModel.ISupportInitialize)picMapImage).EndInit();
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericMaxZoom).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox picMapImage;
        private TextBox txtZoom;
        private TextBox txtLon1;
        private TextBox txtLat1;
        private Button btnDrawIt;
        private Label label2;
        private Label label3;
        private Label label4;
        private TextBox txtLat2;
        private TextBox txtLon2;
        private TextBox txtLon3;
        private TextBox txtLat3;
        private TextBox txtLon4;
        private TextBox txtLat4;
        private TextBox txtLon5;
        private TextBox txtLat5;
        private TextBox txtLon6;
        private TextBox txtLat6;
        private TextBox txtLon7;
        private TextBox txtLat7;
        private FlowLayoutPanel flowLayoutPanel1;
        private Label label5;
        private Label label6;
        private TextBox txtMapWidth;
        private TextBox txtMapHeight;
        private Label label1;
        private NumericUpDown numericMaxZoom;
    }
}
