namespace Ambiance
{
    partial class mainWindow
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
                this.trayIcon.Dispose();
                regMon.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(mainWindow));
            this.setKeyboardButton = new System.Windows.Forms.Button();
            this.trayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.trayMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoColorCheckBox = new System.Windows.Forms.CheckBox();
            this.colorPreviewBox = new System.Windows.Forms.PictureBox();
            this.redLabel = new System.Windows.Forms.Label();
            this.greenLabel = new System.Windows.Forms.Label();
            this.blueLabel = new System.Windows.Forms.Label();
            this.sdkVersionLabelText = new System.Windows.Forms.Label();
            this.sdkVersionLabel = new System.Windows.Forms.Label();
            this.redValueBox = new System.Windows.Forms.NumericUpDown();
            this.greenValueBox = new System.Windows.Forms.NumericUpDown();
            this.blueValueBox = new System.Windows.Forms.NumericUpDown();
            this.colorPreviewLabel = new System.Windows.Forms.Label();
            this.trayMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.colorPreviewBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.redValueBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.greenValueBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.blueValueBox)).BeginInit();
            this.SuspendLayout();
            // 
            // setKeyboardButton
            // 
            this.setKeyboardButton.Location = new System.Drawing.Point(23, 95);
            this.setKeyboardButton.Name = "setKeyboardButton";
            this.setKeyboardButton.Size = new System.Drawing.Size(95, 23);
            this.setKeyboardButton.TabIndex = 0;
            this.setKeyboardButton.Text = "Set Keyboard";
            this.setKeyboardButton.UseVisualStyleBackColor = true;
            this.setKeyboardButton.Click += new System.EventHandler(this.setKeyboardButton_Click);
            // 
            // trayIcon
            // 
            this.trayIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.trayIcon.BalloonTipText = "Ambiance is still running.";
            this.trayIcon.BalloonTipTitle = "Ambiance";
            this.trayIcon.ContextMenuStrip = this.trayMenu;
            this.trayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("trayIcon.Icon")));
            this.trayIcon.Text = "Ambiance";
            this.trayIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.trayIcon_DoubleClick);
            // 
            // trayMenu
            // 
            this.trayMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.trayMenu.Name = "trayMenu";
            this.trayMenu.Size = new System.Drawing.Size(93, 26);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitMenuItem_Click);
            // 
            // autoColorCheckBox
            // 
            this.autoColorCheckBox.AutoSize = true;
            this.autoColorCheckBox.Location = new System.Drawing.Point(23, 124);
            this.autoColorCheckBox.Name = "autoColorCheckBox";
            this.autoColorCheckBox.Size = new System.Drawing.Size(190, 17);
            this.autoColorCheckBox.TabIndex = 3;
            this.autoColorCheckBox.Text = "Pick Color from Background Image";
            this.autoColorCheckBox.UseVisualStyleBackColor = true;
            this.autoColorCheckBox.CheckedChanged += new System.EventHandler(this.autoColorCheckBox_CheckedChanged);
            // 
            // colorPreviewBox
            // 
            this.colorPreviewBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.colorPreviewBox.Location = new System.Drawing.Point(90, 34);
            this.colorPreviewBox.Name = "colorPreviewBox";
            this.colorPreviewBox.Size = new System.Drawing.Size(72, 50);
            this.colorPreviewBox.TabIndex = 4;
            this.colorPreviewBox.TabStop = false;
            // 
            // redLabel
            // 
            this.redLabel.AutoSize = true;
            this.redLabel.Location = new System.Drawing.Point(21, 14);
            this.redLabel.Name = "redLabel";
            this.redLabel.Size = new System.Drawing.Size(15, 13);
            this.redLabel.TabIndex = 8;
            this.redLabel.Text = "R";
            // 
            // greenLabel
            // 
            this.greenLabel.AutoSize = true;
            this.greenLabel.Location = new System.Drawing.Point(21, 40);
            this.greenLabel.Name = "greenLabel";
            this.greenLabel.Size = new System.Drawing.Size(15, 13);
            this.greenLabel.TabIndex = 9;
            this.greenLabel.Text = "G";
            // 
            // blueLabel
            // 
            this.blueLabel.AutoSize = true;
            this.blueLabel.Location = new System.Drawing.Point(22, 66);
            this.blueLabel.Name = "blueLabel";
            this.blueLabel.Size = new System.Drawing.Size(14, 13);
            this.blueLabel.TabIndex = 10;
            this.blueLabel.Text = "B";
            // 
            // sdkVersionLabelText
            // 
            this.sdkVersionLabelText.AutoSize = true;
            this.sdkVersionLabelText.Location = new System.Drawing.Point(20, 144);
            this.sdkVersionLabelText.Name = "sdkVersionLabelText";
            this.sdkVersionLabelText.Size = new System.Drawing.Size(114, 13);
            this.sdkVersionLabelText.TabIndex = 11;
            this.sdkVersionLabelText.Text = "Logitech SDK Version:";
            // 
            // sdkVersionLabel
            // 
            this.sdkVersionLabel.AutoSize = true;
            this.sdkVersionLabel.Location = new System.Drawing.Point(130, 144);
            this.sdkVersionLabel.Name = "sdkVersionLabel";
            this.sdkVersionLabel.Size = new System.Drawing.Size(0, 13);
            this.sdkVersionLabel.TabIndex = 12;
            // 
            // redValueBox
            // 
            this.redValueBox.Location = new System.Drawing.Point(42, 12);
            this.redValueBox.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.redValueBox.Name = "redValueBox";
            this.redValueBox.Size = new System.Drawing.Size(42, 20);
            this.redValueBox.TabIndex = 13;
            this.redValueBox.ValueChanged += new System.EventHandler(this.redValueBox_ValueChanged);
            // 
            // greenValueBox
            // 
            this.greenValueBox.Location = new System.Drawing.Point(42, 38);
            this.greenValueBox.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.greenValueBox.Name = "greenValueBox";
            this.greenValueBox.Size = new System.Drawing.Size(42, 20);
            this.greenValueBox.TabIndex = 14;
            this.greenValueBox.ValueChanged += new System.EventHandler(this.greenValueBox_ValueChanged);
            // 
            // blueValueBox
            // 
            this.blueValueBox.Location = new System.Drawing.Point(42, 64);
            this.blueValueBox.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.blueValueBox.Name = "blueValueBox";
            this.blueValueBox.Size = new System.Drawing.Size(42, 20);
            this.blueValueBox.TabIndex = 15;
            this.blueValueBox.ValueChanged += new System.EventHandler(this.blueValueBox_ValueChanged);
            // 
            // colorPreviewLabel
            // 
            this.colorPreviewLabel.AutoSize = true;
            this.colorPreviewLabel.Location = new System.Drawing.Point(90, 14);
            this.colorPreviewLabel.Name = "colorPreviewLabel";
            this.colorPreviewLabel.Size = new System.Drawing.Size(72, 13);
            this.colorPreviewLabel.TabIndex = 16;
            this.colorPreviewLabel.Text = "Color Preview";
            // 
            // mainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(226, 166);
            this.Controls.Add(this.colorPreviewLabel);
            this.Controls.Add(this.blueValueBox);
            this.Controls.Add(this.greenValueBox);
            this.Controls.Add(this.redValueBox);
            this.Controls.Add(this.sdkVersionLabel);
            this.Controls.Add(this.sdkVersionLabelText);
            this.Controls.Add(this.blueLabel);
            this.Controls.Add(this.greenLabel);
            this.Controls.Add(this.redLabel);
            this.Controls.Add(this.colorPreviewBox);
            this.Controls.Add(this.autoColorCheckBox);
            this.Controls.Add(this.setKeyboardButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "mainWindow";
            this.Text = "Ambiance";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Resize += new System.EventHandler(this.minimizeButton_Click);
            this.trayMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.colorPreviewBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.redValueBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.greenValueBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.blueValueBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.NotifyIcon trayIcon;
        private System.Windows.Forms.Button setKeyboardButton;
        private System.Windows.Forms.CheckBox autoColorCheckBox;
        private System.Windows.Forms.PictureBox colorPreviewBox;
        private System.Windows.Forms.Label redLabel;
        private System.Windows.Forms.Label greenLabel;
        private System.Windows.Forms.Label blueLabel;
        private System.Windows.Forms.ContextMenuStrip trayMenu;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Label sdkVersionLabelText;
        private System.Windows.Forms.Label sdkVersionLabel;
        private System.Windows.Forms.NumericUpDown redValueBox;
        private System.Windows.Forms.NumericUpDown greenValueBox;
        private System.Windows.Forms.NumericUpDown blueValueBox;
        private System.Windows.Forms.Label colorPreviewLabel;
    }
}

