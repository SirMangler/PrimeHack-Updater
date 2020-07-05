namespace PrimeHack_Updater.Source.WinForms
{
    partial class UpdateUI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateUI));
            this.UpdatePanel = new System.Windows.Forms.SplitContainer();
            this.console = new System.Windows.Forms.TextBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.SelectionPanel = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.browse_button = new System.Windows.Forms.Button();
            this.path_box = new System.Windows.Forms.TextBox();
            this.Never = new System.Windows.Forms.Button();
            this.Later = new System.Windows.Forms.Button();
            this.Yes = new System.Windows.Forms.Button();
            this.immersiveMode = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.UpdatePanel)).BeginInit();
            this.UpdatePanel.Panel1.SuspendLayout();
            this.UpdatePanel.Panel2.SuspendLayout();
            this.UpdatePanel.SuspendLayout();
            this.SelectionPanel.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // UpdatePanel
            // 
            this.UpdatePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UpdatePanel.Location = new System.Drawing.Point(0, 0);
            this.UpdatePanel.Name = "UpdatePanel";
            this.UpdatePanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // UpdatePanel.Panel1
            // 
            this.UpdatePanel.Panel1.Controls.Add(this.console);
            // 
            // UpdatePanel.Panel2
            // 
            this.UpdatePanel.Panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(39)))), ((int)(((byte)(39)))));
            this.UpdatePanel.Panel2.Controls.Add(this.progressBar);
            this.UpdatePanel.Size = new System.Drawing.Size(460, 150);
            this.UpdatePanel.SplitterDistance = 104;
            this.UpdatePanel.TabIndex = 0;
            // 
            // console
            // 
            this.console.BackColor = System.Drawing.Color.Black;
            this.console.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.console.Dock = System.Windows.Forms.DockStyle.Fill;
            this.console.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(214)))), ((int)(((byte)(255)))));
            this.console.Location = new System.Drawing.Point(0, 0);
            this.console.Multiline = true;
            this.console.Name = "console";
            this.console.ReadOnly = true;
            this.console.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.console.Size = new System.Drawing.Size(460, 104);
            this.console.TabIndex = 0;
            // 
            // progressBar
            // 
            this.progressBar.BackColor = System.Drawing.Color.Black;
            this.progressBar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(102)))), ((int)(((byte)(0)))));
            this.progressBar.Location = new System.Drawing.Point(12, 11);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(436, 19);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar.TabIndex = 0;
            // 
            // SelectionPanel
            // 
            this.SelectionPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(39)))), ((int)(((byte)(39)))));
            this.SelectionPanel.Controls.Add(this.groupBox1);
            this.SelectionPanel.Controls.Add(this.Never);
            this.SelectionPanel.Controls.Add(this.Later);
            this.SelectionPanel.Controls.Add(this.Yes);
            this.SelectionPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SelectionPanel.Location = new System.Drawing.Point(0, 0);
            this.SelectionPanel.Name = "SelectionPanel";
            this.SelectionPanel.Size = new System.Drawing.Size(460, 150);
            this.SelectionPanel.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.immersiveMode);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.browse_button);
            this.groupBox1.Controls.Add(this.path_box);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(214)))), ((int)(((byte)(255)))));
            this.groupBox1.Location = new System.Drawing.Point(13, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(435, 105);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "QuickLaunch (Optional)";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(102)))), ((int)(((byte)(0)))));
            this.label1.Location = new System.Drawing.Point(12, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(340, 26);
            this.label1.TabIndex = 6;
            this.label1.Text = "Do you want the PrimeHack Updater to automatically boot \r\ninto Metroid Prime: Tri" +
    "logy?";
            // 
            // browse_button
            // 
            this.browse_button.BackColor = System.Drawing.Color.DimGray;
            this.browse_button.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(214)))), ((int)(((byte)(255)))));
            this.browse_button.Location = new System.Drawing.Point(334, 51);
            this.browse_button.Name = "browse_button";
            this.browse_button.Size = new System.Drawing.Size(87, 24);
            this.browse_button.TabIndex = 4;
            this.browse_button.Text = "Browse";
            this.browse_button.UseVisualStyleBackColor = false;
            this.browse_button.Click += new System.EventHandler(this.browse_button_Click);
            // 
            // path_box
            // 
            this.path_box.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(102)))), ((int)(((byte)(0)))));
            this.path_box.Font = new System.Drawing.Font("SansSerif", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.path_box.Location = new System.Drawing.Point(14, 51);
            this.path_box.Name = "path_box";
            this.path_box.ReadOnly = true;
            this.path_box.Size = new System.Drawing.Size(314, 23);
            this.path_box.TabIndex = 3;
            // 
            // Never
            // 
            this.Never.BackColor = System.Drawing.Color.DimGray;
            this.Never.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(214)))), ((int)(((byte)(255)))));
            this.Never.Location = new System.Drawing.Point(169, 114);
            this.Never.Name = "Never";
            this.Never.Size = new System.Drawing.Size(121, 27);
            this.Never.TabIndex = 8;
            this.Never.Text = "Never";
            this.Never.UseVisualStyleBackColor = false;
            this.Never.Click += new System.EventHandler(this.Never_Click);
            // 
            // Later
            // 
            this.Later.BackColor = System.Drawing.Color.DimGray;
            this.Later.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(214)))), ((int)(((byte)(255)))));
            this.Later.Location = new System.Drawing.Point(327, 114);
            this.Later.Name = "Later";
            this.Later.Size = new System.Drawing.Size(121, 27);
            this.Later.TabIndex = 7;
            this.Later.Text = "Ask Later";
            this.Later.UseVisualStyleBackColor = false;
            this.Later.Click += new System.EventHandler(this.Later_Click);
            // 
            // Yes
            // 
            this.Yes.BackColor = System.Drawing.Color.DimGray;
            this.Yes.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(214)))), ((int)(((byte)(255)))));
            this.Yes.Location = new System.Drawing.Point(13, 114);
            this.Yes.Name = "Yes";
            this.Yes.Size = new System.Drawing.Size(121, 27);
            this.Yes.TabIndex = 6;
            this.Yes.Text = "Yes";
            this.Yes.UseVisualStyleBackColor = false;
            this.Yes.Click += new System.EventHandler(this.Yes_Click);
            // 
            // immersiveMode
            // 
            this.immersiveMode.AutoSize = true;
            this.immersiveMode.Location = new System.Drawing.Point(14, 80);
            this.immersiveMode.Name = "immersiveMode";
            this.immersiveMode.Size = new System.Drawing.Size(288, 17);
            this.immersiveMode.TabIndex = 7;
            this.immersiveMode.Text = "Run Game Immersively (Hide Dolphin Window)";
            this.immersiveMode.UseVisualStyleBackColor = true;
            this.immersiveMode.CheckedChanged += new System.EventHandler(this.ImmersiveChecked);
            // 
            // UpdateUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(460, 150);
            this.Controls.Add(this.UpdatePanel);
            this.Controls.Add(this.SelectionPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "UpdateUI";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "PrimeHack Updater";
            this.UpdatePanel.Panel1.ResumeLayout(false);
            this.UpdatePanel.Panel1.PerformLayout();
            this.UpdatePanel.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.UpdatePanel)).EndInit();
            this.UpdatePanel.ResumeLayout(false);
            this.SelectionPanel.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer UpdatePanel;
        private System.Windows.Forms.TextBox console;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Panel SelectionPanel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button browse_button;
        private System.Windows.Forms.TextBox path_box;
        private System.Windows.Forms.Button Never;
        private System.Windows.Forms.Button Later;
        private System.Windows.Forms.Button Yes;
        private System.Windows.Forms.CheckBox immersiveMode;
    }
}