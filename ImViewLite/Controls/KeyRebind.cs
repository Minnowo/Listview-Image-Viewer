using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using ImViewLite.Helpers;
using ImViewLite.Misc;
using ImViewLite.Enums;

namespace ImViewLite.Controls
{
    public partial class KeyRebind : UserControl
    {
        public delegate void KeyBindingChangedEvent(Hotkey keys);
        public event KeyBindingChangedEvent KeyBindingChanged;

        public delegate void KeyFunctionChangedEvent(Command function);
        public event KeyFunctionChangedEvent KeyFunctionChanged;

        public delegate void SelectionChangedEvent(object sender, bool IsSelected);
        public event SelectionChangedEvent SelectionChanged;


        public HotkeyEx KeyBind { get; set; }
        public Command Function 
        {
            get 
            { 
                return m_Function; 
            }
            set 
            {
                m_Function = value;
                btnFunction.Text = value.ToString();
            } 
        }
        private Command m_Function = Command.Nothing;

        public bool IsEditingKeybind { get; private set; }
        public bool IsSelected 
        {
            get 
            { 
                return this.m_IsSelected; 
            }
            set 
            { 
                if (this.m_IsSelected == value)
                    return;

                if (value)
                    this.BackColor = Color.AliceBlue;
                else
                    this.BackColor = Color.White;

                this.m_IsSelected = value;
                preventOverflow = true;
                checkBox1.Checked = this.m_IsSelected;
                preventOverflow = false;
                OnSelectionChanged();
            } 
        }
        private bool m_IsSelected = false;
        private Button button1;
        private bool preventOverflow = false;
        public KeyRebind()
        {
            InitializeComponent();
            btnFunction.Text = m_Function.ToString();
            KeyBind = new HotkeyEx();
            IsEditingKeybind = false;

            SuspendLayout();

            MouseDown += KeyRebind_MouseDown;
            
            ResumeLayout();
        }

        public void StartEditing()
        {
            this.IsEditingKeybind = true;
            this.IsSelected = true;
            UpdateText("Select A Hotkey");
            this.BackColor = Color.White;

            KeyBind.Keys = Keys.None;
            KeyBind.Win = false;
        }

        public void StopEditing()
        {
            this.IsEditingKeybind = false;

            if (this.KeyBind.IsOnlyModifiers)
                this.KeyBind.Keys = Keys.None;

            OnKeyBindingChanged();
            UpdateText();
            this.BackColor = Color.AliceBlue;
        }

        public void UpdateText(string text = "")
        {
            if (string.IsNullOrEmpty(text))
            {
                this.btnInputButton.Text = KeyBind.ToString();
            }
            else
            {
                this.btnInputButton.Text = text;
            }
        }


        private void KeyRebind_MouseDown(object sender, MouseEventArgs e)
        {
            this.IsSelected = true;
        }


        

        private void InputButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.IsEditingKeybind)
                StopEditing();
            else
                StartEditing();
        }

        private void InputButton_Leave(object sender, EventArgs e)
        {
            if (this.IsEditingKeybind)
                StopEditing();
        }


        private void InputButton_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;

            if (!this.IsEditingKeybind)
                return;
            
            if (e.KeyData == Keys.Escape)
            {
                KeyBind.Keys = Keys.None;
                StopEditing();
            }
            else if (e.KeyCode == Keys.LWin || e.KeyCode == Keys.RWin)
            {
                KeyBind.Win = !KeyBind.Win;
                UpdateText();
            }
            else if (new Hotkey(e.KeyData).IsValidHotkey)
            {
                KeyBind.Keys = e.KeyData;
                StopEditing();
            }
            else
            {
                KeyBind.Keys = e.KeyData;
                UpdateText();
            }
        }

        private void OnKeyBindingChanged()
        {
            if (KeyBindingChanged != null)
            {
                this.Invoke(KeyBindingChanged, KeyBind);
            }
        }

        private void OnKeyFunctionChanged()
        {
            if (KeyFunctionChanged != null)
            {
                this.Invoke(KeyFunctionChanged, m_Function);
            }
        }

        private void OnSelectionChanged()
        {
            if (SelectionChanged != null)
            {
                this.Invoke(SelectionChanged, this, this.m_IsSelected);
            }
        }

        private void FunctionButton_MouseClick(object sender, MouseEventArgs e)
        {
            Forms.FunctionSelectorForm f = new Forms.FunctionSelectorForm();
            f.Location = Cursor.Position;
            f.Size = new Size(200, 300);
            f.MaximumSize = new Size(350, 500);
            f.MinimumSize = new Size(50, 100);
            if (f.ShowDialog(this) == DialogResult.OK)
            {
                Function = f.SelectedFunction;
                this.OnKeyFunctionChanged();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Forms.ArgumentSelectionForm f = new Forms.ArgumentSelectionForm(this.Function);
            f.Args = this.KeyBind.Args;
            f.Location = Cursor.Position;
            f.Size = new Size(500, 200);
            f.MaximumSize = new Size(550, 500);
            f.MinimumSize = new Size(100, 100);
            if (f.ShowDialog(this) == DialogResult.OK)
            {
                KeyBind.Args = f.Args;
            }
        }

        private void Checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if (preventOverflow)
                return;

            this.IsSelected = checkBox1.Checked;
        }

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



        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnInputButton = new System.Windows.Forms.Button();
            this.btnFunction = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnInputButton
            // 
            this.btnInputButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnInputButton.Location = new System.Drawing.Point(263, 2);
            this.btnInputButton.Margin = new System.Windows.Forms.Padding(0);
            this.btnInputButton.Name = "btnInputButton";
            this.btnInputButton.Size = new System.Drawing.Size(213, 21);
            this.btnInputButton.TabIndex = 0;
            this.btnInputButton.Text = "None";
            this.btnInputButton.UseVisualStyleBackColor = true;
            this.btnInputButton.KeyDown += new System.Windows.Forms.KeyEventHandler(this.InputButton_KeyDown);
            this.btnInputButton.Leave += new System.EventHandler(this.InputButton_Leave);
            this.btnInputButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.InputButton_MouseClick);
            // 
            // btnFunction
            // 
            this.btnFunction.Location = new System.Drawing.Point(34, 2);
            this.btnFunction.Name = "btnFunction";
            this.btnFunction.Size = new System.Drawing.Size(227, 21);
            this.btnFunction.TabIndex = 3;
            this.btnFunction.Text = "button1";
            this.btnFunction.UseVisualStyleBackColor = true;
            this.btnFunction.MouseClick += new System.Windows.Forms.MouseEventHandler(this.FunctionButton_MouseClick);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(9, 6);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(15, 14);
            this.checkBox1.TabIndex = 4;
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.Checkbox_CheckedChanged);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(479, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(41, 21);
            this.button1.TabIndex = 5;
            this.button1.Text = "Args";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // KeyRebind
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.button1);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.btnFunction);
            this.Controls.Add(this.btnInputButton);
            this.Name = "KeyRebind";
            this.Size = new System.Drawing.Size(523, 25);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnInputButton;
        private System.Windows.Forms.Button btnFunction;
        private System.Windows.Forms.CheckBox checkBox1;

    }
}
