using System;
using System.Drawing;
using System.Windows.Forms;
using ImViewLite.Enums;


namespace ImViewLite.Forms
{
    // this is like 20x faster than filling a combobox with the enum so i'm using it instead
    public partial class FunctionSelectorForm : Form
    {
        public Command SelectedFunction;

        public FunctionSelectorForm()
        {
            InitializeComponent();
            this.Text = "";
            this.StartPosition = FormStartPosition.Manual;

            for (int i = Enum.GetValues(typeof(Command)).Length-1; i >= 0; i--)
            {
                Button tsmi = new Button();
                tsmi.TabIndex = i;
                tsmi.Tag = (Command)i;
                tsmi.Text = EnumToString.CommandToString((Command)i);

                tsmi.Dock = DockStyle.Top;
                tsmi.FlatStyle = FlatStyle.Flat;
                tsmi.TextAlign = ContentAlignment.MiddleLeft;
                tsmi.Margin = new Padding(0);
                tsmi.BackColor = SystemColors.Window;
                tsmi.Click += ButtonCallback;

                this.Controls.Add(tsmi);
            }
        }

        public void ItemSelected(Command c)
        {
            SelectedFunction = c;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ButtonCallback(object sender, EventArgs e)
        {
            Button b = sender as Button;
            if(b != null)
            {
                ItemSelected((Command)b.Tag);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            switch (e.KeyData)
            {
                case Keys.LControlKey | Keys.W:
                case Keys.Control | Keys.W:
                case Keys.Alt | Keys.F4:
                case Keys.Escape:
                    Close();
                    break;
            }
        }
    }
}
