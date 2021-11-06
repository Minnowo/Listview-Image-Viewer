namespace ImViewLite.Forms
{
    partial class SettingsForm
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
            this.cbProfiles = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.tcMain = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.pgMain = new System.Windows.Forms.PropertyGrid();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.hcMain = new ImViewLite.Controls.HotkeyControl();
            this.btnRestoreDefaultHotkeys = new System.Windows.Forms.Button();
            this.btnRemoveHotkey = new System.Windows.Forms.Button();
            this.btnAddHotkey = new System.Windows.Forms.Button();
            this.tcMain.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbProfiles
            // 
            this.cbProfiles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbProfiles.FormattingEnabled = true;
            this.cbProfiles.Location = new System.Drawing.Point(13, 33);
            this.cbProfiles.Name = "cbProfiles";
            this.cbProfiles.Size = new System.Drawing.Size(198, 21);
            this.cbProfiles.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Profile:";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(301, 31);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 6;
            this.button3.Text = "Remove";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.RemoveButton_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(217, 31);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 7;
            this.button4.Text = "Add";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // tcMain
            // 
            this.tcMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tcMain.Controls.Add(this.tabPage1);
            this.tcMain.Controls.Add(this.tabPage2);
            this.tcMain.Location = new System.Drawing.Point(3, 60);
            this.tcMain.Name = "tcMain";
            this.tcMain.SelectedIndex = 0;
            this.tcMain.Size = new System.Drawing.Size(524, 386);
            this.tcMain.TabIndex = 8;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.pgMain);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(516, 360);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Internals";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // pgMain
            // 
            this.pgMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgMain.HelpVisible = false;
            this.pgMain.Location = new System.Drawing.Point(3, 3);
            this.pgMain.Name = "pgMain";
            this.pgMain.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.pgMain.Size = new System.Drawing.Size(510, 354);
            this.pgMain.TabIndex = 0;
            this.pgMain.ToolbarVisible = false;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.hcMain);
            this.tabPage2.Controls.Add(this.btnRestoreDefaultHotkeys);
            this.tabPage2.Controls.Add(this.btnRemoveHotkey);
            this.tabPage2.Controls.Add(this.btnAddHotkey);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(375, 364);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "KeyBinds";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // hcMain
            // 
            this.hcMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hcMain.Location = new System.Drawing.Point(8, 35);
            this.hcMain.Name = "hcMain";
            this.hcMain.Size = new System.Drawing.Size(361, 321);
            this.hcMain.TabIndex = 4;
            // 
            // btnRestoreDefaultHotkeys
            // 
            this.btnRestoreDefaultHotkeys.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRestoreDefaultHotkeys.Location = new System.Drawing.Point(236, 6);
            this.btnRestoreDefaultHotkeys.Name = "btnRestoreDefaultHotkeys";
            this.btnRestoreDefaultHotkeys.Size = new System.Drawing.Size(133, 23);
            this.btnRestoreDefaultHotkeys.TabIndex = 3;
            this.btnRestoreDefaultHotkeys.Text = "Restore Default";
            this.btnRestoreDefaultHotkeys.UseVisualStyleBackColor = true;
            this.btnRestoreDefaultHotkeys.Click += new System.EventHandler(this.RestoreDefaultHotkeys_Click);
            // 
            // btnRemoveHotkey
            // 
            this.btnRemoveHotkey.Location = new System.Drawing.Point(89, 6);
            this.btnRemoveHotkey.Name = "btnRemoveHotkey";
            this.btnRemoveHotkey.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveHotkey.TabIndex = 2;
            this.btnRemoveHotkey.Text = "Remove";
            this.btnRemoveHotkey.UseVisualStyleBackColor = true;
            this.btnRemoveHotkey.Click += new System.EventHandler(this.RemoveHotkey_Click);
            // 
            // btnAddHotkey
            // 
            this.btnAddHotkey.Location = new System.Drawing.Point(8, 6);
            this.btnAddHotkey.Name = "btnAddHotkey";
            this.btnAddHotkey.Size = new System.Drawing.Size(75, 23);
            this.btnAddHotkey.TabIndex = 1;
            this.btnAddHotkey.Text = "Add";
            this.btnAddHotkey.UseVisualStyleBackColor = true;
            this.btnAddHotkey.Click += new System.EventHandler(this.AddHotkey_Click);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(529, 446);
            this.Controls.Add(this.tcMain);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbProfiles);
            this.Name = "SettingsForm";
            this.Text = "SettingsForm";
            this.tcMain.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox cbProfiles;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.TabControl tcMain;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.PropertyGrid pgMain;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button btnRestoreDefaultHotkeys;
        private System.Windows.Forms.Button btnRemoveHotkey;
        private System.Windows.Forms.Button btnAddHotkey;
        private ImViewLite.Controls.HotkeyControl hcMain;
    }
}