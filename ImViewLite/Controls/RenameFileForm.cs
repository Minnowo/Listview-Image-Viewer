using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using ImViewLite.Helpers;

namespace ImViewLite.Controls
{
    public partial class RenameFileForm : Form
    {
        public string NewName
        {
            get { return textBox1.Text; }
        }
        public string Extension = "";
        public bool isDirectory = false;
        public bool ForceExtension = true;
        private bool preventOverflow = false;

        public RenameFileForm(string file) : this()
        {
            this.Text = file;
        }

        public RenameFileForm()
        {
            InitializeComponent();
            this.Text = "";
            textBox2.Focus();
        }

        public static string RenamePath(string path)
        {
            if (File.Exists(path))
                return RenameFile(path);
            if (Directory.Exists(path))
                return RenameDirectory(path);
            return string.Empty;
        }

        public static string RenameDirectory(string path)
        {
            if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
                return string.Empty;

            using(RenameFileForm rnf = new RenameFileForm(path))
            {
                rnf.Extension = "";
                rnf.isDirectory = true;

                if (rnf.ShowDialog() == DialogResult.OK)
                {
                    if (PathHelper.MoveDirectory(path, rnf.NewName))
                    {
                        return rnf.NewName;
                    }
                }
            }
            return string.Empty;
        }

        public static string RenameFile(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
                return string.Empty;
            
            using (RenameFileForm rnf = new RenameFileForm(path))
            {
                rnf.Extension = Helper.GetFilenameExtension(path);

                if (rnf.ShowDialog() == DialogResult.OK)
                {
                    if (PathHelper.MoveFile(path, rnf.NewName))
                        return rnf.NewName;
                }
            }
            return string.Empty;
        }

        private void _Close()
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void DoneButton_Click(object sender, EventArgs e)
        {
            Done();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            _Close();
        }

        private void Done()
        {
            if (string.IsNullOrEmpty(textBox1.Text))
                _Close();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            this.ForceExtension = !checkBox1.Checked;
            preventOverflow = true;

            if (ForceExtension && !isDirectory)
            {
                textBox1.Text = textBox2.Text + "." + Extension;
            }
            else
            {
                textBox1.Text = textBox2.Text;
            }

            preventOverflow = false;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (preventOverflow)
                return;

            preventOverflow = true;
            string t = textBox2.Text;
            
            foreach(string c in Helpers.Helper.IllegalPathCharacters)
            {
                t = t.Replace(c,"");
            }

            if (ForceExtension && !isDirectory)
            {
                textBox1.Text = t + "." + Extension;
            }
            else
            {
                textBox1.Text = t;
            }
            textBox2.Text = t;
            preventOverflow = false;
        }

        private void RenameFileForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Escape:
                    _Close();
                    break;

                case Keys.Enter:
                    Done();
                    break;
            }
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Escape:
                    _Close();
                    break;

                case Keys.Enter:
                    Done();
                    break;
            }
        }
    }
}
