using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using ImViewLite.Helpers;

namespace ImViewLite.Controls
{
    [DefaultEvent("ColorChanged")]
    public partial class ColorPicker : UserControl
    {
        public event ColorEventHandler ColorChanged;

        public COLOR SelectedColor
        {
            get
            {
                return selectedColor;
            }
            set
            {
                selectedColor = value;
                colorBox.SelectedColor = selectedColor;
                colorSlider.SelectedColor = selectedColor;
                OnColorChanged();
            }
        }


        public DrawStyles DrawStyle
        {
            get
            {
                return drawStyle;
            }
            set
            {
                drawStyle = value;
                colorBox.DrawStyle = value;
                colorSlider.DrawStyle = value;
                Invalidate();
            }
        }

        [DefaultValue(DrawStyles.HSBHue)]
        private DrawStyles drawStyle = DrawStyles.HSBHue;
        private COLOR selectedColor;
        private ColorPickerBox colorBox;
        private ColorPickerSlider colorSlider;

        public ColorPicker()
        {
            InitializeComponent();
            Size = new Size(290, 516);
            colorBox.ColorChanged += ColorBox_ColorChanged;
            colorSlider.ColorChanged += ColorSlider_ColorChanged;
            selectedColor = Color.FromArgb(255, 0, 0);
        }

        private void ColorSlider_ColorChanged(object sender, ColorEventArgs e)
        {
            SelectedColor = e.Color;
        }

        private void ColorBox_ColorChanged(object sender, ColorEventArgs e)
        {
            SelectedColor = e.Color;
        }

        private void OnColorChanged()
        {
            if (ColorChanged != null)
                ColorChanged(this, new ColorEventArgs(selectedColor, DrawStyle));
        }

        private void InitializeComponent()
        {
            this.colorBox = new ColorPickerBox();
            this.colorSlider = new ColorPickerSlider();
            this.SuspendLayout();
            // 
            // colorBox
            // 
            this.colorBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.colorBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.colorBox.CrosshairVisible = true;
            this.colorBox.DrawStyle = DrawStyles.HSBHue;
            this.colorBox.Location = new System.Drawing.Point(0, 0);
            this.colorBox.Name = "colorBox";
            this.colorBox.Size = new System.Drawing.Size(258, 258);
            this.colorBox.TabIndex = 0;
            // 
            // colorSlider
            // 
            this.colorSlider.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.colorSlider.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.colorSlider.CrosshairVisible = true;
            this.colorSlider.DrawStyle = DrawStyles.HSBHue;
            this.colorSlider.Location = new System.Drawing.Point(257, 0);
            this.colorSlider.Name = "colorSlider";
            this.colorSlider.Size = new System.Drawing.Size(32, 258);
            this.colorSlider.TabIndex = 1;
            // 
            // ColorPicker
            // 
            this.AutoSize = true;
            this.Controls.Add(this.colorBox);
            this.Controls.Add(this.colorSlider);
            this.Name = "ColorPicker";
            this.Size = new System.Drawing.Size(292, 261);
            this.ResumeLayout(false);

        }
    }
}
