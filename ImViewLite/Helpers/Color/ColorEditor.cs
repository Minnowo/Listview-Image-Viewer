using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace ImViewLite.Helpers
{
    class ColorEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (value.GetType() != typeof(Color))
            {
                return value;
            }

            IWindowsFormsEditorService svc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

            if (svc != null)
            {
                Color color = (Color)value;

                using (ColorPickerForm form = new ColorPickerForm(color))
                {
                    if (svc.ShowDialog(form) == DialogResult.OK)
                    {
                        return form.GetCurrentColor().ARGB.ToColor();
                    }
                }
            }

            return value;
        }

        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override void PaintValue(PaintValueEventArgs e)
        {
            Graphics g = e.Graphics;
            Color color = (Color)e.Value;

            if (color.A < 255)
            {
                using(Bitmap bmp = ImageProcessor.GetCheckeredBitmap(e.Bounds.Width, e.Bounds.Height, e.Bounds.Height/2, SystemColors.ControlLight, SystemColors.ControlLightLight))
                {
                    g.DrawImage(bmp, e.Bounds);
                }
            }

            using (SolidBrush brush = new SolidBrush(color))
            {
                e.Graphics.FillRectangle(brush, e.Bounds);
            }

            e.Graphics.DrawRectangleProper(Pens.Black, e.Bounds);
        }
    }
   
}
