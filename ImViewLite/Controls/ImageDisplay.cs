using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.IO;
using ImViewLite.Helpers;

namespace ImViewLite.Controls
{
    public enum DrawMode
    {
        /// <summary>
        /// Always scale the image to fit the maximum possible size.
        /// </summary>
        FitImage,

        /// <summary>
        /// Only show the image as default size.
        /// </summary>
        ActualSize,

        /// <summary>
        /// Scale the image when it doesn't fit on the control, otherwise show the default image size
        /// </summary>
        ScaleImage
    }
    public partial class ImageDisplay : UserControl
    {
        public InterpolationMode InterpolationMode
        {
            get { return _InterpolationMode; }
            set 
            {
                if (_InterpolationMode == value)
                    return;
                _InterpolationMode = value;
                Invalidate();
            }
        }
        private InterpolationMode _InterpolationMode = InterpolationMode.NearestNeighbor;
        public int CellSize
        {
            get { return _CellSize; }
            set
            {
                if (this._CellSize == value)
                    return;

                this._CellSize = value;
                this.InitTileBrush((int)(this.CellSize * this.CellScale), this.CellColor1, this.CellColor2);
            }
        }
        private int _CellSize = 32;
        
        public float CellScale
        {
            get { return _CellScale; }
            set 
            { 
                if (this._CellScale == value) 
                    return;

                this._CellScale = value;
                this.InitTileBrush((int)(this.CellSize * this.CellScale), this.CellColor1, this.CellColor2);
            }
        }
        private float _CellScale = 2;
        
        public Color CellColor1
        {
            get { return _CellColor1; }
            set
            {
                if (this._CellColor1 == value)
                    return;

                this._CellColor1 = value;
                this.InitTileBrush((int)(this.CellSize * this.CellScale), this.CellColor1, this.CellColor2);
            }
        }
        private Color _CellColor1 = Color.FromArgb(32, 32, 32);
        
        public Color CellColor2
        {
            get { return _CellColor2; }
            set
            {
                if (this._CellColor2 == value)
                    return;

                this._CellColor2 = value;
                this.InitTileBrush((int)(this.CellSize * this.CellScale), this.CellColor1, this.CellColor2);
            }
        }
        private Color _CellColor2 = Color.FromArgb(64, 64, 64);

        public DrawMode DrawMode
        {
            get { return this._DrawMode; }
            set
            {
                this._DrawMode = value;
                Invalidate();
            }
        }
        private DrawMode _DrawMode = DrawMode.FitImage;

        public FileInfo ImagePath;

        public bool Display = true;
        public bool IsAnimating = false;
        public bool AnimationPaused = false;
        public bool CenterImage = true;
        public bool DisposeImageOnReplace = true;
        public double Zoom = 1;

        public IMAGE Image
        {
            get { return this._Image; }
            set 
            {
                if (value != null)
                {
                    if (value.GetImageFormat() == ImgFormat.gif)
                    {
                        Gif g = value as Gif;
                        g.Animate(OnFrameChangedHandler);
                        this.IsAnimating = true;
                    }
                    else
                    {
                        this.IsAnimating = false;
                    }
                }
                if (DisposeImageOnReplace)
                {
                    this._Image?.Dispose();
                    if (Settings.InternalSettings.Garbage_Collect_On_Image_Unload)
                    {
                        GC.Collect();
                    }
                }
                this.ImagePath = null;
                this._Image = value;
                Invalidate();
            }
        }
        private IMAGE _Image;

        public Brush BackgroundTileBrush;
        public ImageDisplay()
        {
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | 
                ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            InitializeComponent();
            this.Width = 50;
            this.Height = 50;
            this.InitTileBrush((int)(this.CellSize * this.CellScale), this.CellColor1, this.CellColor2);
        }

        /// <summary>
        /// Load the next frame of the image;
        /// </summary>
        public void NextImageFrame()
        {
            if (_Image == null)
                return;

            ImgFormat fmt = _Image.GetImageFormat();

            if(fmt == ImgFormat.gif)
            {
                (_Image as Gif).NextFrame();
            }
            else if(fmt == ImgFormat.ico)
            {
                (_Image as ICO).SelectedImageIndex++;
            }
        }

        /// <summary>
        /// Loads the previous image frame.
        /// </summary>
        public void PreviousImageFrame()
        {
            if (_Image == null)
                return;

            ImgFormat fmt = _Image.GetImageFormat();

            if (fmt == ImgFormat.gif)
            {
                (_Image as Gif).PreviousFrame();
            }
            else if (fmt == ImgFormat.ico)
            {
                (_Image as ICO).SelectedImageIndex--;
            }
        }

        /// <summary>
        /// Copies the current image to the clipboard.
        /// </summary>
        public void CopyImage()
        {
            if (_Image == null)
                return;

            ClipboardHelper.CopyImage(_Image);
        }

        /// <summary>
        /// Inverts the color of the current image.
        /// </summary>
        public void InvertColor()
        {
            if (_Image == null)
                return;

            _Image.InvertColor();
            Invalidate();
        }

        /// <summary>
        /// Converts the current image to grayscale.
        /// </summary>
        public void ConvertGrayscale()
        {
            if (_Image == null)
                return;

            _Image.ConvertGrayscale();
            Invalidate();
        }

        /// <summary>
        /// Tries to load an image from the given path.
        /// </summary>
        /// <param name="path">The path of the image.</param>
        /// <returns>true if the image was loaded, else false.</returns>
        public bool TryLoadImage(string path)
        {
            if (!File.Exists(path))
                return false;

            IMAGE i = ImageHelper.LoadImage(path);

            if (i == null)
                return false;

            if (this._Image != null)
                _Image.Dispose();

            this.Image = i;
            this.ImagePath = new FileInfo(path);
            Invalidate();
            return true;
        }

        private Rectangle GetInsideViewPort(bool includePadding)
        {
            int left = 0;
            int top = 0;
            int width = this.ClientSize.Width;
            int height = this.ClientSize.Height;

            if (includePadding)
            {
                left += this.Padding.Left;
                top += this.Padding.Top;
                width -= this.Padding.Horizontal;
                height -= this.Padding.Vertical;
            }

            return new Rectangle(left, top, width, height);
        }


        private void FitImage()
        {
            if (this._Image == null)
                return;
            Rectangle innerRectangle;
            double zoom;
            double aspectRatio;

            this.AutoScrollMinSize = Size.Empty;

            innerRectangle = this.GetInsideViewPort(true);

            if (this._Image.Width > this._Image.Height)
            {
                aspectRatio = (double)innerRectangle.Width / this._Image.Width;
                zoom = aspectRatio;

                if (innerRectangle.Height < this._Image.Height * zoom)
                {
                    aspectRatio = (double)innerRectangle.Height / this._Image.Height;
                    zoom = aspectRatio;
                }
            }
            else
            {
                aspectRatio = (double)innerRectangle.Height / this._Image.Height;
                zoom = aspectRatio;

                if (innerRectangle.Width < this._Image.Width * zoom)
                {
                    aspectRatio = (double)innerRectangle.Width / this._Image.Width;
                    zoom = aspectRatio;
                }
            }
            this.Zoom = zoom;
        }

        int drx = 0;
        int dry = 0;
        private Rectangle GetImageViewPort()
        {
            if (this._Image == null)
                return Rectangle.Empty;

            int centerX = drx;
            int centerY = dry;
            int width = this._Image.Width;
            int height = this._Image.Height;
            switch (this.DrawMode)
            {
                case DrawMode.ActualSize:
                    if (this.CenterImage)
                    {
                        centerX = this.Width / 2 - width / 2;
                        centerY = this.Height / 2 - height / 2;
                    }
                    return new Rectangle(centerX, centerY, width, height);

                case DrawMode.FitImage:
                    FitImage();

                    width = (int)Math.Round(this._Image.Width * this.Zoom);
                    height = (int)Math.Round(this._Image.Height * this.Zoom);

                    if (this.CenterImage)
                    {
                        centerX = this.Width / 2 - width / 2;
                        centerY = this.Height / 2 - height / 2;
                    }
                    return new Rectangle(centerX, centerY, width, height);

                case DrawMode.ScaleImage:

                    
                    if (this._Image.Width > this.Width || this._Image.Height > this.Height)
                    {
                        FitImage();
                        
                        width = (int)Math.Round(this._Image.Width * this.Zoom);
                        height = (int)Math.Round(this._Image.Height * this.Zoom);
                    }

                    if (this.CenterImage)
                    {
                        centerX = this.Width / 2 - width / 2;
                        centerY = this.Height / 2 - height / 2;
                    }
                    return new Rectangle(centerX, centerY, width, height);
            }
            return Rectangle.Empty;
        }

        private RectangleF GetSourceImageRegion()
        {
            if (this._Image == null)
                return RectangleF.Empty;
            return new RectangleF(0, 0, this._Image.Width, this._Image.Height);
        }

        /*Point cs;
        bool isLeftClicking = false;
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if(e.Button == MouseButtons.Left)
            {
                cs = e.Location;
                isLeftClicking = true;
            }
            if(e.Button == MouseButtons.Right)
            {
                drx = 0;
                dry = 0;
                Invalidate();
            }
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button == MouseButtons.Left)
            {
                isLeftClicking = false;
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (isLeftClicking)
            {
                drx -= cs.X - e.X;
                if (drx > this.Width)
                    drx = this.Width;

                dry -= cs.Y - e.Y;
                if (dry > this.Height)
                    dry = this.Height;

                cs = e.Location;
                Invalidate();
            }
        }*/

        private void DrawImage(Graphics g)
        {
            InterpolationMode currentInterpolationMode = g.InterpolationMode;
            PixelOffsetMode currentPixelOffsetMode = g.PixelOffsetMode;

            g.InterpolationMode = this.InterpolationMode;

            // disable pixel offsets. Thanks to Rotem for the info.
            // http://stackoverflow.com/questions/14070311/why-is-graphics-drawimage-cropping-part-of-my-image/14070372#14070372
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            try
            {
                if (this.IsAnimating && !AnimationPaused)
                {
                    ImageAnimator.UpdateFrames(this._Image);
                }

                g.DrawImage(this._Image, this.GetImageViewPort(), this.GetSourceImageRegion(), GraphicsUnit.Pixel);
            }
            catch
            {
            }

            g.PixelOffsetMode = currentPixelOffsetMode;
            g.InterpolationMode = currentInterpolationMode;
        }

        private void DrawBackground(Graphics g)
        {
            g.FillRectangle(this.BackgroundTileBrush, this.GetInsideViewPort(false));
        }

        public void InitTileBrush(int cellSize, Color cellColor, Color alternateCellColor)
        {
            Bitmap result;
            int width;
            int height;

            // draw the tile
            width = cellSize * 2;
            height = cellSize * 2;
            result = new Bitmap(width, height);

            using (Graphics g = Graphics.FromImage(result))
            {
                using (Brush brush = new SolidBrush(cellColor))
                {
                    g.FillRectangle(brush, new Rectangle(cellSize, 0, cellSize, cellSize));
                    g.FillRectangle(brush, new Rectangle(0, cellSize, cellSize, cellSize));
                }

                using (Brush brush = new SolidBrush(alternateCellColor))
                {
                    g.FillRectangle(brush, new Rectangle(0, 0, cellSize, cellSize));
                    g.FillRectangle(brush, new Rectangle(cellSize, cellSize, cellSize, cellSize));
                }
            }

            this.BackgroundTileBrush?.Dispose();
            this.BackgroundTileBrush = new TextureBrush(result);
            this.Invalidate();
        }

        private void OnFrameChangedHandler(object sender, EventArgs eventArgs)
        {
            if (this.AnimationPaused)
                return;
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.Display)
            {
                /*e.Graphics.InterpolationMode = this.InterpolationMode;*/
                this.DrawBackground(e.Graphics);
                this.DrawImage(e.Graphics);
            }
            base.OnPaint(e);
        }
    }
}
