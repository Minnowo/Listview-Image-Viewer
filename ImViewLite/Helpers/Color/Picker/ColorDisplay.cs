using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using ImViewLite.Helpers;

namespace ImViewLite.Controls
{
    public partial class ColorDisplay : UserControl
    {
        public Color CurrentColor
        {
            get
            {
                return currentColor;
            }
            set
            {
                lastColor = currentColor;
                currentColor = value;

                Invalidate();
            }
        }
        private Color currentColor = Color.White;

        public Color LastColor
        {
            get
            {
                return lastColor;
            }
            set
            {
                lastColor = value;

                Invalidate();
            }
        }
        private Color lastColor = Color.White;

        public Color BorderColor
        {
            get
            {
               return borderPen.Color;
            }
            set
            {
                borderPen?.Dispose();
                borderPen = new Pen(value);
                BackColor = value;

                Invalidate();
            }
        }

        public int ShowToolTipAfter
        {
            get
            {
                return toolTipTimer.Interval;
            }
            set
            {
                toolTipTimer.Interval = value;
            }
        }
        public bool ShowToolTip = true;
        public ToolTipLocation ShowToolTipAt = ToolTipLocation.ControlBottom;

        private Rectangle currentColorRect = new Rectangle(0, 0, 16, 16);
        private Rectangle lastColorRect = new Rectangle(16, 16, 16, 16);

        private Pen borderPen = new Pen(Color.Black);

        private Timer toolTipTimer = new Timer() { Interval = 255};

        public ColorDisplay()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            InitializeComponent();

            toolTipTimer.Tick += ToolTipTimer_Tick;

            CurrentColor = Color.FromArgb(255, 255, 255);
            LastColor = Color.FromArgb(255, 255, 255);
            BorderColor = Color.Black;
            BackColor = Color.Black;

            Size = new Size(32, 32);
        }

        private void ToolTipTimer_Tick(object sender, EventArgs e)
        {
            toolTipTimer.Stop();

            if (!ShowToolTip)
                return;
            
            Point p = new Point();
            string tooltipText = $"#{ColorHelper.ColorToHex(currentColor)}\n#{ColorHelper.ColorToHex(lastColor)}";

            switch (ShowToolTipAt)
            {
                case ToolTipLocation.Mouse:
                    p = PointToClient(Cursor.Position);

                    // need to offset to prevent the OnMouseLeave from removing tooltip
                    p.X += 1;
                    p.Y += 1;
                    break;

                case ToolTipLocation.ControlBottom:
                    p = new Point(ClientRectangle.X, ClientRectangle.Y + ClientSize.Height);
                    break;

                case ToolTipLocation.ControlTop:
                    // close enough idc enough to make it perfect cause idk what the
                    // tooltip font is and i don't know where to find it

                    p = new Point(ClientRectangle.X, ClientRectangle.Y - TextRenderer.MeasureText(tooltipText, this.Font).Height);
                    break;

                case ToolTipLocation.ControlLeft:
                    // close enough idc enough to make it perfect cause idk what the
                    // tooltip font is and i don't know where to find it

                    p = new Point(ClientRectangle.X - TextRenderer.MeasureText(tooltipText, this.Font).Width, ClientRectangle.Y);
                    break;

                case ToolTipLocation.ControlRight:
                    p = new Point(ClientRectangle.X + ClientSize.Width, ClientRectangle.Y);
                    break;
            }

            if (p != Point.Empty)
            {
                tt_Main.Show(tooltipText, this, p);
                return;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            using(SolidBrush cur = new SolidBrush(currentColor))
                g.FillRectangle(cur, currentColorRect);

            using (SolidBrush cur = new SolidBrush(lastColor))
                g.FillRectangle(cur, lastColorRect);

            g.DrawRectangle(borderPen, 0, 0, ClientSize.Width - 1, ClientSize.Height - 1);

            base.OnPaint(e);
        }

        protected override void OnClientSizeChanged(EventArgs e)
        {
            int halfHeight = (int)Math.Ceiling(ClientSize.Height / 2f);

            currentColorRect = new Rectangle(0, 0, ClientSize.Width, halfHeight);
            lastColorRect = new Rectangle(0, halfHeight + 1, ClientSize.Width, halfHeight);

            Invalidate();
            base.OnClientSizeChanged(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            toolTipTimer.Stop();
            tt_Main.Hide(this);

            base.OnMouseLeave(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            toolTipTimer.Start();
            base.OnMouseEnter(e);
        }
    }
}
