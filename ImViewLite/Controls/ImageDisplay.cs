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
        public bool ClearImagePathOnReplace = true;
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

                if (this.ClearImagePathOnReplace)
                {
                    this.ImagePath = null;
                }
                this._Image = value;
                this._drx = 0;
                this._dry = 0;
                Invalidate();
            }
        }
        private IMAGE _Image;

        public Brush BackgroundTileBrush;

        /// <summary>
        /// used when dragging the image when the <see cref="DrawMode"/> is set to actual size
        /// </summary>
        Point _lastClickedPoint;
        bool _isLeftClicking = false;

        /// <summary>
        /// x offset to draw the image when the image is not being centered 
        /// </summary>
        int _drx = 0;

        /// <summary>
        /// y offset to draw the image when the image is not being centered
        /// </summary>
        int _dry = 0;

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
        /// Reloads the current image, or does nothing if it cannot.
        /// </summary>
        public void ReloadImage()
        {
            if (this.ImagePath == null || !this.ImagePath.Exists)
                return;

            IMAGE i = ImageHelper.LoadImage(this.ImagePath.FullName);

            if (i == null)
                return;

            bool _ = this.DisposeImageOnReplace;
            bool __ = this.ClearImagePathOnReplace;

            this.DisposeImageOnReplace = true;
            this.ClearImagePathOnReplace = false;

            this.Image = i;

            this.DisposeImageOnReplace = _;
            this.ClearImagePathOnReplace = __;
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

            bool _ = this.DisposeImageOnReplace;
            this.DisposeImageOnReplace = true;

            this.Image = i;
            this.ImagePath = new FileInfo(path);

            this.DisposeImageOnReplace = _;
            Invalidate();
            return true;
        }

        /// <summary>
        /// Gets the control size.
        /// </summary>
        /// <param name="includePadding">Should padding be subtracted from the client size.</param>
        /// <returns>The size of the control with or without padding.</returns>
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

        /// <summary>
        /// sets the zoom level to fit the image within the control, while keeping aspect ratio
        /// </summary>
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

        
        /// <summary>
        /// Gets the destination rectangle to draw the image
        /// </summary>
        /// <returns></returns>
        private Rectangle GetImageViewPort()
        {
            if (this._Image == null)
                return Rectangle.Empty;

            int centerX = _drx;
            int centerY = _dry;
            int width = this._Image.Width;
            int height = this._Image.Height;
            switch (this.DrawMode)
            {
                case DrawMode.ActualSize:
                    if (this.CenterImage)
                    {
                        if (width < this.Width)
                        {
                            centerX = this.Width / 2 - width / 2;
                        }

                        if (height < this.Height)
                        {
                            centerY = this.Height / 2 - height / 2;
                        }
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

        
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            // if the drawmode is set to actual size, we need to allow the user to drag the image

            if (this.DrawMode != DrawMode.ActualSize)
                return;
         
            // if they left click, set variables which enable dragging
            if (e.Button == MouseButtons.Left)
            {
                _lastClickedPoint = e.Location;
                _isLeftClicking = true;
            }

            // on right click, we reset the offsets
            if (e.Button == MouseButtons.Right)
            {
                _drx = 0;
                _dry = 0;
                Invalidate();
            }
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button == MouseButtons.Left)
            {
                _isLeftClicking = false;
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            // if not in actual size mode, drag the image
            if (this.DrawMode != DrawMode.ActualSize)
                return;
         
            // if user is left clicking
            if (_isLeftClicking)
            {
                // adjust the offset x
                _drx -= _lastClickedPoint.X - e.X;
                if (_drx > this.Width)
                {
                    _drx = this.Width;
                }

                // adjust the offset y 
                _dry -= _lastClickedPoint.Y - e.Y;
                if (_dry > this.Height)
                {
                    _dry = this.Height;
                }

                // set the new last click pos and redraw the image
                _lastClickedPoint = e.Location;
                Invalidate();
            }
        }

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
