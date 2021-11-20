using System;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using ImViewLite.Helpers;

namespace ImViewLite.Forms
{
    public partial class OpenWithForm : Form
    {
        string TargetDirectory 
        { 
            get { return _TargetDirectory; } 
            set 
            {
                if (value == _TargetDirectory)
                    return;

                textBox1.Text = value;
                LoadDirectory(value);
                _TargetDirectory = value; 
            } 
        }
        string _TargetDirectory;

        public string FileToOpen;

        private bool _PreventOverflow = false;
        public OpenWithForm(string p )
        {
            if (!File.Exists(p) && !Directory.Exists(p))
                throw new FileNotFoundException("The given path does not exist: In [OpenWithForm.cs]");
            InitializeComponent();
            this.textBox1.ShortcutsEnabled = true;
            textBox1.AutoCompleteMode = AutoCompleteMode.Suggest;
            textBox1.AutoCompleteSource = AutoCompleteSource.FileSystemDirectories;
            
            FileToOpen = p;
            TargetDirectory = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu) + "\\Programs";
        }

        public void LoadDirectory(string path)
        {
            if (!Directory.Exists(path))
                return;

            this.panel1.Controls.Clear();
            foreach(string file in Directory.EnumerateFiles(path))
            {
                FileInfo info = new FileInfo(file);

                Button b = new Button();
                b.TextAlign = ContentAlignment.MiddleLeft;
                b.Text = info.Name;
                b.Tag = info;
                b.Dock = DockStyle.Top;
                b.Click += B_Click;

                this.panel1.Controls.Add(b);
            }
        }

        private void B_Click(object sender, EventArgs e)
        {
            Button b = sender as Button;

            if (b == null)
                return;

            FileInfo info = (FileInfo)b.Tag;

            if (!File.Exists(info.FullName))
                return;

            Process p = new Process();
            p.StartInfo.FileName = info.FullName;
            p.StartInfo.Arguments = "\"" + this.FileToOpen + "\"";
            p.Start();

            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string dir = PathHelper.SelectFolderDialog(_TargetDirectory);

            if (string.IsNullOrEmpty(dir))
                return;

            TargetDirectory = dir;
        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text) || _PreventOverflow)
                return;

            string text = textBox1.Text;

            DirectoryInfo info;
            if (PathHelper.IsValidDirectoryPath(text, out info))
            {
                if (info.Exists)
                {
                    _PreventOverflow = true;
                    this.TargetDirectory = info.FullName;
                    _PreventOverflow = false;
                }
            }
        }

        private void textBox1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if(e.KeyCode == Keys.Tab)
            {
                e.IsInputKey = true;
            }
        }
    }
}
