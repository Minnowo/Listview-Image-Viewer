using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System;

using ImViewLite.Helpers;

namespace ImViewLite.Controls
{
    public abstract partial class ColorPickerBase : UserControl
    {
        public event ColorEventHandler ColorChanged;

        public bool CrosshairVisible
        {
            get
            {
                return crosshairVisible;
            }
            set
            {
                crosshairVisible = value;
                Invalidate();
            }
        }

        public COLOR SelectedColor
        {
            get
            {
                return selectedColor;
            }

            set
            {
                selectedColor = value;

                if (this is ColorPickerBox)
                {
                    SetBoxMarker();
                }
                else
                {
                    SetSliderMarker();
                }

                Invalidate();
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

                if (this is ColorPickerBox)
                {
                    SetBoxMarker();
                }
                else
                {
                    SetSliderMarker();
                }

                Invalidate();
            }
        }

        protected DrawStyles drawStyle;
        protected COLOR selectedColor;
        protected COLOR absoluteColor = Color.White;
        protected Point lastClicked;
        protected int clientWidth, clientHeight;
        protected bool isLeftClicking;
        protected bool crosshairVisible = false;
        protected Timer mouseMoveTimer;
        protected Bitmap bmp;

        protected void Init()
        {
            SuspendLayout();
            this.DoubleBuffered = true;

            this.clientWidth = ClientRectangle.Width;
            this.clientHeight = ClientRectangle.Height;

            this.selectedColor = Color.Red;
            this.drawStyle = DrawStyles.HSBHue;

            bmp = new Bitmap(clientWidth, clientHeight, PixelFormat.Format32bppArgb);

            mouseMoveTimer = new Timer();
            mouseMoveTimer.Interval = 10;
            mouseMoveTimer.Tick += new EventHandler(MouseMoveTimer_Tick);

            ClientSizeChanged += new EventHandler(ClientSizeChanged_Event);
            MouseDown += new MouseEventHandler(MouseDown_Event);
            MouseEnter += new EventHandler(MouseEnter_Event);
            MouseUp += new MouseEventHandler(MouseUp_Event);
            Paint += new PaintEventHandler(Paint_Event);

            ResumeLayout(false);
        }

        public ColorPickerBase()
        {
            InitializeComponent();
            Init();
        }

        private void MouseMoveTimer_Tick(object sender, EventArgs e)
        {
            if (!isLeftClicking)
                return;

            Point mousePosition = PointToClient(MousePosition);

            if (lastClicked == mousePosition)
                return;
            
            lastClicked = GetPoint(mousePosition);

            if (this is ColorPickerBox)
            {
                GetBoxColor();
            }
            else
            {
                GetSliderColor();
            }

            OnColorChanged();
            Refresh();
        }

        private void ClientSizeChanged_Event(object sender, EventArgs e)
        {
            clientWidth = ClientRectangle.Width;
            clientHeight = ClientRectangle.Height;
            bmp.Dispose();
            bmp = new Bitmap(clientWidth, clientHeight, PixelFormat.Format32bppArgb);
            DrawColors();
        }

        private void MouseDown_Event(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    crosshairVisible = true;
                    isLeftClicking = true;
                    mouseMoveTimer.Start();

                    break;
                case MouseButtons.Right:
                    break;
                case MouseButtons.Middle:
                    break;
            }
        }

        private void MouseUp_Event(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    isLeftClicking = false;
                    mouseMoveTimer.Stop();
                    break;
                case MouseButtons.Right:
                    break;
                case MouseButtons.Middle:
                    break;
            }
        }
        private void MouseEnter_Event(object sender, EventArgs e)
        {

        }
        public static Bitmap CreateCheckerPattern(int width, int height, Color checkerColor1, Color checkerColor2)
        {
            Bitmap bmp = new Bitmap(width * 2, height * 2);

            using (Graphics g = Graphics.FromImage(bmp))
            using (Brush brush1 = new SolidBrush(checkerColor1))
            using (Brush brush2 = new SolidBrush(checkerColor2))
            {
                g.FillRectangle(brush1, 0, 0, width, height);
                g.FillRectangle(brush1, width, height, width, height);
                g.FillRectangle(brush2, width, 0, width, height);
                g.FillRectangle(brush2, 0, height, width, height);
            }

            return bmp;
        }
        private void Paint_Event(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            if (!isLeftClicking)
            {
                if (selectedColor.isTransparent)
                {
                    if (bmp != null) 
                        bmp.Dispose();

                    bmp = new Bitmap(clientWidth, clientHeight, PixelFormat.Format24bppRgb);

                    ImageProcessor.DrawCheckers(bmp,32, 
                        SystemColors.ControlLight,
                        SystemColors.ControlLightLight);

                }
                DrawColors();
            }

            g.DrawImage(bmp, ClientRectangle);

            if (CrosshairVisible)
            {
                DrawCrosshair(g);
            }
        }

        protected void DrawColors()
        {
            switch (drawStyle)
            {
                // HSB Color Space
                case DrawStyles.HSBHue:
                    DrawHSBHue();
                    break;
                case DrawStyles.HSBSaturation:
                    DrawHSBSaturation();
                    break;
                case DrawStyles.HSBBrightness:
                    DrawHSBBrightness();
                    break;

                // HSL Color Space
                case DrawStyles.HSLHue:
                    DrawHSLHue();
                    break;
                case DrawStyles.HSLSaturation:
                    DrawHSLSaturation();
                    break;
                case DrawStyles.HSLLightness:
                    DrawHSLLightness();
                    break;

                // RGB Color Space
                case DrawStyles.Red:
                    DrawRed();
                    break;
                case DrawStyles.Green:
                    DrawGreen();
                    break;
                case DrawStyles.Blue:
                    DrawBlue();
                    break;
            }
        }
        protected abstract void DrawCrosshair(Graphics g);

        protected abstract void DrawHSBHue();
        protected abstract void DrawHSBSaturation();
        protected abstract void DrawHSBBrightness();

        protected abstract void DrawHSLHue();
        protected abstract void DrawHSLSaturation();
        protected abstract void DrawHSLLightness();

        protected abstract void DrawRed();
        protected abstract void DrawGreen();
        protected abstract void DrawBlue();



        protected void GetBoxColor()
        {
            switch (DrawStyle)
            {
                // HSB Color Space
                case DrawStyles.HSBHue:
                    selectedColor.HSB.Saturation = (float)lastClicked.X / clientWidth;
                    selectedColor.HSB.Brightness = (float)(1.0 - ((double)lastClicked.Y / clientHeight));
                    selectedColor.UpdateHSB();
                    break;
                case DrawStyles.HSBSaturation:
                    selectedColor.HSB.Hue = (float)lastClicked.X / clientWidth;
                    selectedColor.HSB.Brightness = (float)(1.0 - ((double)lastClicked.Y / clientHeight));
                    selectedColor.UpdateHSB();
                    break;
                case DrawStyles.HSBBrightness:
                    selectedColor.HSB.Hue = (float)lastClicked.X / clientWidth;
                    selectedColor.HSB.Saturation = (float)(1.0 - ((double)lastClicked.Y / clientHeight));
                    selectedColor.UpdateHSB();
                    break;

                // HSL Color Space
                case DrawStyles.HSLHue:
                    selectedColor.HSL.Saturation = (float)lastClicked.X / clientWidth;
                    selectedColor.HSL.Lightness = (float)(1.0 - ((double)lastClicked.Y / clientHeight));
                    selectedColor.UpdateHSL();
                    break;
                case DrawStyles.HSLSaturation:
                    selectedColor.HSL.Hue = (float)lastClicked.X / clientWidth;
                    selectedColor.HSL.Lightness = (float)(1.0 - ((double)lastClicked.Y / clientHeight));
                    selectedColor.UpdateHSL();
                    break;
                case DrawStyles.HSLLightness:
                    selectedColor.HSL.Hue = (float)lastClicked.X / clientWidth;
                    selectedColor.HSL.Saturation = (float)(1.0 - ((double)lastClicked.Y / clientHeight));
                    selectedColor.UpdateHSL();
                    break;

                // RGB Color Space
                case DrawStyles.Red:
                    selectedColor.ARGB.B = (byte)Math.Round(255 * (double)lastClicked.X / (clientWidth));
                    selectedColor.ARGB.G = (byte)Math.Round(255 * (1.0 - ((double)lastClicked.Y / (clientHeight))));
                    selectedColor.UpdateARGB();
                    break;
                case DrawStyles.Green:
                    selectedColor.ARGB.B = (byte)Math.Round(255 * (double)lastClicked.X / (clientWidth));
                    selectedColor.ARGB.R = (byte)Math.Round(255 * (1.0 - ((double)lastClicked.Y / (clientHeight))));
                    selectedColor.UpdateARGB();
                    break;
                case DrawStyles.Blue:
                    selectedColor.ARGB.R = (byte)Math.Round(255 * (double)lastClicked.X / (clientWidth));
                    selectedColor.ARGB.G = (byte)Math.Round(255 * (1.0 - ((double)lastClicked.Y / (clientHeight))));
                    selectedColor.UpdateARGB();
                    break;
            }
        }

        protected void GetSliderColor()
        {
            switch (DrawStyle)
            {
                // HSB Color Space
                case DrawStyles.HSBHue:
                    selectedColor.HSB.Hue = (float)(1.0 - ((double)lastClicked.Y / clientHeight));
                    selectedColor.UpdateHSB();
                    break;
                case DrawStyles.HSBSaturation:
                    selectedColor.HSB.Saturation = (float)(1.0 - ((double)lastClicked.Y / clientHeight));
                    selectedColor.UpdateHSB();
                    break;
                case DrawStyles.HSBBrightness:
                    selectedColor.HSB.Brightness = (float)(1.0 - ((double)lastClicked.Y / clientHeight));
                    selectedColor.UpdateHSB();
                    break;

                // HSL Color Space
                case DrawStyles.HSLHue:
                    selectedColor.HSL.Hue = (float)(1.0 - ((double)lastClicked.Y / clientHeight));
                    selectedColor.UpdateHSL();
                    break;
                case DrawStyles.HSLSaturation:
                    selectedColor.HSL.Saturation = (float)(1.0 - ((double)lastClicked.Y / clientHeight));
                    selectedColor.UpdateHSL();
                    break;
                case DrawStyles.HSLLightness:
                    selectedColor.HSL.Lightness = (float)(1.0 - ((double)lastClicked.Y / clientHeight));
                    selectedColor.UpdateHSL();
                    break;

                // RGB Color Space
                case DrawStyles.Red:
                    selectedColor.ARGB.R = (byte)(255 - Math.Round(255 * (double)lastClicked.Y / clientHeight));
                    selectedColor.UpdateARGB();
                    break;
                case DrawStyles.Green:
                    selectedColor.ARGB.G = (byte)(255 - Math.Round(255 * (double)lastClicked.Y / clientHeight));
                    selectedColor.UpdateARGB();
                    break;
                case DrawStyles.Blue:
                    selectedColor.ARGB.B = (byte)(255 - Math.Round(255 * (double)lastClicked.Y / clientHeight));
                    selectedColor.UpdateARGB();
                    break;
            }
        }

        protected void SetBoxMarker()
        {
            switch (drawStyle)
            {
                // HSB Color Space
                case DrawStyles.HSBHue:
                    lastClicked.X = (int)Math.Round(clientWidth * selectedColor.HSB.Saturation);
                    lastClicked.Y = (int)Math.Round(clientHeight * (1.0 - selectedColor.HSB.Brightness));
                    break;
                case DrawStyles.HSBSaturation:
                    lastClicked.X = (int)Math.Round(clientWidth * selectedColor.HSB.Hue);
                    lastClicked.Y = (int)Math.Round(clientHeight * (1.0 - selectedColor.HSB.Brightness));
                    break;
                case DrawStyles.HSBBrightness:
                    lastClicked.X = (int)Math.Round(clientWidth * selectedColor.HSB.Hue);
                    lastClicked.Y = (int)Math.Round(clientHeight * (1.0 - selectedColor.HSB.Saturation));
                    break;

                // HSL Color Space
                case DrawStyles.HSLHue:
                    lastClicked.X = (int)Math.Round(clientWidth * selectedColor.HSL.Saturation);
                    lastClicked.Y = (int)Math.Round(clientHeight * (1.0 - selectedColor.HSL.Lightness));
                    break;
                case DrawStyles.HSLSaturation:
                    lastClicked.X = (int)Math.Round(clientWidth * selectedColor.HSL.Hue);
                    lastClicked.Y = (int)Math.Round(clientHeight * (1.0 - selectedColor.HSL.Lightness));
                    break;
                case DrawStyles.HSLLightness:
                    lastClicked.X = (int)Math.Round(clientWidth * selectedColor.HSL.Hue);
                    lastClicked.Y = (int)Math.Round(clientHeight * (1.0 - selectedColor.HSL.Saturation));
                    break;

                // RGB Color Space
                case DrawStyles.Red:
                    lastClicked.X = (int)Math.Round(clientWidth * (double)selectedColor.ARGB.B / 255);
                    lastClicked.Y = (int)Math.Round(clientHeight * (1.0 - ((double)selectedColor.ARGB.G / 255)));
                    break;
                case DrawStyles.Green:
                    lastClicked.X = (int)Math.Round(clientWidth * (double)selectedColor.ARGB.B / 255);
                    lastClicked.Y = (int)Math.Round(clientHeight * (1.0 - ((double)selectedColor.ARGB.R / 255)));
                    break;
                case DrawStyles.Blue:
                    lastClicked.X = (int)Math.Round(clientWidth * (double)selectedColor.ARGB.R / 255);
                    lastClicked.Y = (int)Math.Round(clientHeight * (1.0 - ((double)selectedColor.ARGB.G / 255)));
                    break;
            }

            lastClicked = GetPoint(lastClicked);
        }

        protected void SetSliderMarker()
        {
            switch (DrawStyle)
            {
                // HSB Color Space
                case DrawStyles.HSBHue:
                    lastClicked.Y = (clientHeight) - (int)Math.Round(((clientHeight) * SelectedColor.HSB.Hue));
                    break;
                case DrawStyles.HSBSaturation:
                    lastClicked.Y = (clientHeight) - (int)Math.Round(((clientHeight) * SelectedColor.HSB.Saturation));
                    break;
                case DrawStyles.HSBBrightness:
                    lastClicked.Y = (clientHeight) - (int)Math.Round(((clientHeight) * SelectedColor.HSB.Brightness));
                    break;

                // HSL Color Space
                case DrawStyles.HSLHue:
                    lastClicked.Y = (clientHeight) - (int)Math.Round(((clientHeight) * SelectedColor.HSL.Hue));
                    break;
                case DrawStyles.HSLSaturation:
                    lastClicked.Y = (clientHeight) - (int)Math.Round(((clientHeight) * SelectedColor.HSL.Saturation));
                    break;
                case DrawStyles.HSLLightness:
                    lastClicked.Y = (clientHeight) - (int)Math.Round(((clientHeight) * SelectedColor.HSL.Lightness));
                    break;

                // RGB Color Space
                case DrawStyles.Red:
                    lastClicked.Y = (clientHeight) - (int)Math.Round(((clientHeight) * (double)SelectedColor.ARGB.R / 255));
                    break;
                case DrawStyles.Green:
                    lastClicked.Y = (clientHeight) - (int)Math.Round(((clientHeight) * (double)SelectedColor.ARGB.G / 255));
                    break;
                case DrawStyles.Blue:
                    lastClicked.Y = (clientHeight) - (int)Math.Round(((clientHeight) * (double)SelectedColor.ARGB.B / 255));
                    break;
            }

            lastClicked = GetPoint(lastClicked);
        }

        protected Point GetPoint(Point point)
        {
            return new Point(point.X.Clamp(0, clientWidth), point.Y.Clamp(0, clientHeight));
        }

        private void OnColorChanged()
        {
            //Console.WriteLine(this.absoluteColor.argb);
            if (ColorChanged != null)
                ColorChanged(this, new ColorEventArgs(this.selectedColor, this.drawStyle));
        }
    }
}
