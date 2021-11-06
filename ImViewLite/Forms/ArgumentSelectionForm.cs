using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ImViewLite.Helpers;
using ImViewLite.Enums;

namespace ImViewLite.Forms
{
    public partial class ArgumentSelectionForm : Form
    {
        public int NumberOfArgs
        {
            get { return _NumberOfArgs; }
            set 
            {
                if (_NumberOfArgs == value)
                    return;

                _NumberOfArgs = value;
                UpdateArgumentBoxes();
                UpdateText();
            }
        }
        private int _NumberOfArgs = 0;

        public string ToolTipText = "";

        public string[] Args
        {
            get
            {
                if (_NumberOfArgs < 1)
                    return new string[] { };

                string[] args = new string[_NumberOfArgs];
                for(int i = 0; i < _NumberOfArgs; i++)
                {
                    args[i] = panel1.Controls[i].Text;
                }
                return args;
            }
            set
            {
                if (value.Length != _NumberOfArgs)
                    return;

                for (int i = 0; i < _NumberOfArgs; i++)
                {
                    panel1.Controls[i].Text = value[i];
                }
            }
        }

        public ArgumentSelectionForm()
        {
            InitializeComponent();
            UpdateText();
        }

        public ArgumentSelectionForm(Command cmd) : this()
        {
            NumberOfArgs = GetNumberOfArgs(cmd);   
        }

        public ArgumentSelectionForm(int numberOfArgs) : this()
        {
            NumberOfArgs = numberOfArgs;
        }

        public void UpdateText()
        {
            this.label1.Text = $"Number of arguments: {_NumberOfArgs}";
        }

        public void UpdateArgumentBoxes()
        {
            this.panel1.Controls.Clear();

            for (int i = 0; i < NumberOfArgs; i++)
            {
                TextBox entry = new TextBox();
                entry.Name = $"arg{i}";
                entry.Tag = i;
                entry.Margin = new Padding(0, 0, 0, 0);
                entry.Dock = DockStyle.Top;
                this.panel1.Controls.Add(entry);
            }
        }

        public int GetNumberOfArgs(Command cmd) 
        {
            switch (cmd)
            {
                case Command.UpDirectoryLevel:
                case Command.MoveImage:
                case Command.RenameImage:
                    return 1;
                case Command.Nothing:
                case Command.CopyImage:
                case Command.OpenSelectedDirectory:
                case Command.PauseGif:
                case Command.NextFrame:
                case Command.PreviousFrame:
                case Command.ToggleAlwaysOnTop:
                case Command.DeleteImage:
                case Command.InvertColor:
                case Command.Grayscale:
                case Command.OpenColorPicker:
                case Command.OpenSettings:
                    return 0;
            }
            return 0;
        }

        private void Done_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
