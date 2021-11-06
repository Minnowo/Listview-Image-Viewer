using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Drawing.Design;
using System.Xml.Serialization;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;

using ImViewLite.Enums;
using ImViewLite.Misc;
using ImViewLite.Helpers;

namespace ImViewLite.Settings
{
    public static class InternalSettings
    {
        public const string User_Settings_Path = "usrConfig.xml";
        public static string Temp_Image_Folder = Path.Combine(AppContext.BaseDirectory, "tmp");


        public static List<string> Readable_Image_Formats_Dialog_Options = new List<string>
        { "*.png", "*.jpg", "*.jpeg", "*.jpe", "*.jfif", "*.gif", "*.bmp", "*.tif", "*.tiff", "*.ico", "*.wrm", "*.dwrm" };

        public static List<string> Readable_Image_Formats = new List<string>()
        { "png", "jpg", "jpeg", "jpe", "jfif", "gif", "bmp", "tif", "tiff", "ico", "wrm", "dwrm" };

        public static List<string> Readable_Color_Palette_Dialog_Options = new List<string>
        { "*.aco", "*.lbm", "*.bmm", "*.txt" };


        public const string Folder_Select_Dialog_Title = "Select a folder";
        public const string Image_Select_Dialog_Title = "Select an image";
        public const string Save_File_Dialog_Title = "Save Item";
        public const string Move_File_Dialog_Title = "Move Item";

        public const string All_Files_File_Dialog = "All Files (*.*)|*.*";

        public static string All_Image_Files_File_Dialog = string.Format(
            "Graphic Types ({0})|{1}",
            string.Join(", ", Readable_Image_Formats_Dialog_Options),
            string.Join(";", Readable_Image_Formats_Dialog_Options));


        // image formats
        public const string PNG_File_Dialog = "PNG (*.png)|*.png";
        public const string BMP_File_Dialog = "BMP (*.bmp)|*.bmp";
        public const string GIF_File_Dialog = "GIF (*.gif)|*.gif";
        public const string ICO_File_Dialog = "ICO (*.ico)|*.ico";
        public const string WEBP_File_Dialog = "WEBP (*.webp)|*.webp";
        public const string TIFF_File_Dialog = "TIFF (*.tif, *.tiff)|*.tif;*.tiff";
        public const string JPEG_File_Dialog = "JPEG (*.jpg, *.jpeg, *.jpe, *.jfif)|*.jpg;*.jpeg;*.jpe;*.jfif";
        public const string WRM_File_Dialog = "WRM (*.wrm, *.dwrm)|*.wrm;*.dwrm";


        public static string Image_Dialog_Filters = string.Join("|",
            new string[]
            {
                PNG_File_Dialog,
                JPEG_File_Dialog,
                BMP_File_Dialog,
                TIFF_File_Dialog,
                GIF_File_Dialog,
                ICO_File_Dialog,
                WRM_File_Dialog
            });


        #region message box 

        public const string Delete_Item_Messagebox_Title = "Delete Item?";
        public const string Delete_Item_Messagebox_Message = "Are you sure you would like to delete this item: ";

        public const string Item_Does_Not_Exist_Title = "Item not found";
        public const string Item_Does_Not_Exist_Message = "This item does not exist";

        public const string Invalid_Size_Messagebox_Title = "Invalid image size";
        public const string Invalid_Size_Messagebox_Message = "The image size is not valid";

        public const string Unable_To_Invert_Image_Title = "Unable to invert colors.";
        public const string Unable_To_Invert_Image_Message = "There was an error when trying to invert the colors of this image, most likely the pixel format is not 32bppArgb.";

        public const string Unable_To_Convert_To_Grey_Image_Title = "Unable to convert to grey.";
        public const string Unable_To_Convert_To_Grey_Image_Message = "There was an error when trying to convert to grey, most likely the pixel format is not 32bppArgb.";

        public const string Unable_To_Convert_To_Fill_Transparent_Title = "Unable to fill transparent pixels.";
        public const string Unable_To_Convert_To_Fill_Transparent_Message = "There was an error when trying to fill transparent pixels, most likely the pixel format is not 32bppArgb.";

        public const string No_Animation_Frames_Found_Title = "No animation frames found.";
        public const string No_Animation_Frames_Found_Message = "Unable to detect any animation frames to export.";

        public const string Error_Loading_Settings_Title = "Unable to load settings";
        public const string Error_Loading_Settings_Message = "There was an error trying to load the settings, the file will be deleted / overwritten when the program exits";

        public const string Gif_Export_Title = "Gif export";

        #endregion


        #region rate limits

        public static short Fit_To_Screen_On_Resize_Limit = 250;

        public static short Dither_Threshold_Update_Limit = 1000;

        public static short Folder_Watcher_Resort_Timer_Limit = 5000;

        #endregion

        public static Regex ReDigit = new Regex(@"\d+", RegexOptions.Compiled);

        public static Font CloseButtonFont = new Font(new Font("Consolas", 10), FontStyle.Bold);

        public static double[] Grey_Scale_Multipliers = new double[] { 0.11, 0.59, 0.3 };

        public static Size TSMI_Generated_Icon_Size = new Size(16, 16);

        public static int Grid_Cell_Size
        {
            get { return CurrentUserSettings.Grid_Cell_Size; }
            set { CurrentUserSettings.Grid_Cell_Size = value; }
        }

        public static Color Image_Box_Back_Color
        {
            get { return CurrentUserSettings.Image_Box_Back_Color; }
            set { CurrentUserSettings.Image_Box_Back_Color = value; }
        }

        public static Color Image_Box_Back_Color_Alternate
        {
            get { return CurrentUserSettings.Image_Box_Back_Color_Alternate; }
            set { CurrentUserSettings.Image_Box_Back_Color_Alternate = value; }
        }

        public static InterpolationMode Default_Interpolation_Mode
        {
            get { return CurrentUserSettings.Default_Interpolation_Mode; }
            set { CurrentUserSettings.Default_Interpolation_Mode = value; }
        }



        public static Color Fill_Transparency_On_Copy_Color
        {
            get { return CurrentUserSettings.Fill_Transparency_On_Copy_Color; }
            set { CurrentUserSettings.Fill_Transparency_On_Copy_Color = value; }
        }

        public static bool CenterChild_When_Parent_Following_Child
        {
            get { return CurrentUserSettings.CenterChild_When_Parent_Following_Child; }
            set { CurrentUserSettings.CenterChild_When_Parent_Following_Child = value; }
        }
        public static bool Parent_Follow_Child
        {
            get { return CurrentUserSettings.Parent_Follow_Child; }
            set { CurrentUserSettings.Parent_Follow_Child = value; }
        }

        public static bool Full_Row_Select
        {
            get { return CurrentUserSettings.Full_Row_Select; }
            set { CurrentUserSettings.Full_Row_Select = value; }
        }

        // because of how we load the image there is extra memory that doesn't get disposed
        // and calling GC.Collect removes that, but since garbage collection can cause problems
        // gonna allow the user to disable it as the please 
        // i also just hate when stuff doesn't get cleared from meory so i like to GC a lot 
        public static bool Garbage_Collect_On_Image_Unload
        {
            get { return CurrentUserSettings.Garbage_Collect_On_Image_Unload; }
            set { CurrentUserSettings.Garbage_Collect_On_Image_Unload = value; }
        }

        public static bool Garbage_Collect_After_Disposing_Gif
        {
            get { return CurrentUserSettings.Garbage_Collect_After_Disposing_Gif; }
            set { CurrentUserSettings.Garbage_Collect_After_Disposing_Gif = value; }
        }

        public static bool Delete_Temp_Directory
        {
            get { return CurrentUserSettings.Delete_Temp_Directory_On_Close; }
            set { CurrentUserSettings.Delete_Temp_Directory_On_Close = value; }
        }

        public static bool Use_Alternate_Copy_Method
        {
            get { return CurrentUserSettings.Use_Alternate_Copy_Method; }
            set { CurrentUserSettings.Use_Alternate_Copy_Method = value; }
        }

        public static bool Replace_Transparency_On_Copy
        {
            get { return CurrentUserSettings.Replace_Transparency_On_Copy; }
            set { CurrentUserSettings.Replace_Transparency_On_Copy = value; }
        }

        public static bool Always_On_Top
        {
            get { return CurrentUserSettings.Always_On_Top; }
            set { CurrentUserSettings.Always_On_Top = value; }
        }

        public static readonly HotkeyEx[] Default_Key_Binds = new HotkeyEx[6]
        {
            new HotkeyEx(Keys.Back, Command.UpDirectoryLevel),
            new HotkeyEx(Keys.Delete , Command.DeleteImage),
            new HotkeyEx(Keys.R, Command.RenameImage),
            new HotkeyEx(Keys.Space, Command.PauseGif),
            new HotkeyEx(Keys.I, Command.InvertColor),
            new HotkeyEx(Keys.G, Command.Grayscale)
        };

        public static bool WebP_Plugin_Exists = false;

        public static bool CPU_Type_x64 = IntPtr.Size == 8;

        public static UserControlledSettings CurrentUserSettings = new UserControlledSettings();
        public static SettingsProfiles SettingProfiles = new SettingsProfiles();


        public static void UpdateDialogFilters()
        {
            All_Image_Files_File_Dialog = string.Format(
             "Graphic types ({0})|{1}",
             string.Join(", ", Readable_Image_Formats_Dialog_Options),
             string.Join(";", Readable_Image_Formats_Dialog_Options));

            Image_Dialog_Filters = string.Join("|",
                new string[]
                {
                    PNG_File_Dialog,
                    JPEG_File_Dialog,
                    BMP_File_Dialog,
                    TIFF_File_Dialog,
                    GIF_File_Dialog,
                    ICO_File_Dialog,
                    WRM_File_Dialog
                });

            if (WebP_Plugin_Exists)
                Image_Dialog_Filters += "|" + WEBP_File_Dialog;
        }

        public static void EnableWebPIfPossible()
        {
            if (CPU_Type_x64)
            {
                if (File.Exists(Webp.libwebP_x64))
                {
                    WebP_Plugin_Exists = true;
                    Readable_Image_Formats_Dialog_Options.Add("*.webp");
                    Readable_Image_Formats.Add("webp");
                    UpdateDialogFilters();
                }
            }
            else
            {
                if (File.Exists(Webp.libwebP_x86))
                {
                    WebP_Plugin_Exists = true;
                    Readable_Image_Formats_Dialog_Options.Add("*.webp");
                    Readable_Image_Formats.Add("webp");
                    UpdateDialogFilters();
                }
            }

        }
    }

    [Serializable()]
    [XmlRoot(ElementName ="EventTrigger", Namespace ="", IsNullable =false)]
    public class CommandArg
    {
        public string[] Args;
        public Command Command;

        public CommandArg()
        {
            Args = null;
            Command = Command.Nothing;
        }

        public CommandArg(Command cmd, string[] args)
        {
            Command = cmd;
            Args = args;
        }
    }

    [Serializable()]
    [XmlRoot(ElementName = "SettingsProfiles", Namespace = "", IsNullable = false)]
    public class SettingsProfiles : List<UserControlledSettings>
    {
        public SettingsProfiles()
        {

        }

        public SettingsProfiles(List<UserControlledSettings> items)
        {
            this.AddRange(items);
        }
    }



    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class UserControlledSettings
    {
        public void UpdateBinds()
        {
            Binds.Clear();

            if (_Binds.Count < 1)
                return;

            foreach (HotkeyEx bk in _Binds)
            {
                if (Binds.ContainsKey(bk.Keys))
                {
                    Binds[bk.Keys] = bk;
                    continue;
                }
                Binds.Add(bk.Keys, bk);
            }
        }

        [Browsable(false)]
        public List<HotkeyEx> _Binds { get; set; } = new List<HotkeyEx>();//InternalSettings.Default_Key_Binds.ToList();

        [Browsable(false)]
        [XmlIgnore]
        public Dictionary<Keys, HotkeyEx> Binds = new Dictionary<Keys, HotkeyEx>();


        [Browsable(false)]
        [XmlIgnore]
        public Guid ID = Guid.NewGuid();

        [Description("The name of the profile."), DisplayName("Name")]
        public string ProfileName { get; set; } = "Nil";


        [Description("Should the garbage collecter be called after every image unloads."), DisplayName("Garbage Collect On Image Unload")]
        public bool Garbage_Collect_On_Image_Unload { get; set; } = false;

       

        [Description("Should the garbage collecter be called after a gif is disposed."), DisplayName("Garbage Collect After Gif Dispose")]
        public bool Garbage_Collect_After_Disposing_Gif { get; set; } = true;

   

        [Description("Should the tmp directory be deleted when the application closes."), DisplayName("Delete Temp Dir On Exit")]
        public bool Delete_Temp_Directory_On_Close { get; set; } = true;


        [Description("Should the mainform be a topmost window."), DisplayName("Always On Top")]
        public bool Always_On_Top { get; set; } = false;


        [Description("Enabled fullrow select in the listview"), DisplayName("Full Row Select")]
        public bool Full_Row_Select { get; set; } = false;


        [Description("Should the parent window follow children that take control."), DisplayName("Parent Follow Children")]
        public bool Parent_Follow_Child { get; set; } = true;


        [Description("Should the child window be centered on the parent when parent following child."), DisplayName("Center Child When Parent Following")]
        public bool CenterChild_When_Parent_Following_Child { get; set; } = true;



        [Description("Copy image in a way that keeps transparency."), DisplayName("Alternate Image Copy Method (keeps transparency)")]
        public bool Use_Alternate_Copy_Method
        {
            get { return use_Alternate_Copy_Method; }
            set
            {
                use_Alternate_Copy_Method = value;
                if (value)
                    Replace_Transparency_On_Copy = false;
            }
        }
        [Browsable(false)]
        [XmlIgnore]
        private bool use_Alternate_Copy_Method = true;

        [Description("Fill transparency when copying image."), DisplayName("Fill Transparency On Copy")]
        public bool Replace_Transparency_On_Copy
        {
            get { return replace_Transparency_On_Copy; }
            set
            {
                replace_Transparency_On_Copy = value;
                if (value)
                    use_Alternate_Copy_Method = false;
            }
        }
        [Browsable(false)]
        [XmlIgnore]
        private bool replace_Transparency_On_Copy = false;


        [Description("The size of the transparent back grid cells."), DisplayName("Transparent Grid Cell Size")]
        public int Grid_Cell_Size { get; set; } = 8;


        [XmlIgnore]
        [Description("The color that fills the transparent pixels when copying an image."), DisplayName("Fill Transparency On Copy")]
        [DefaultValue(typeof(Color), "White"),
        Editor(typeof(ColorEditor), typeof(UITypeEditor)),
        TypeConverter(typeof(_ColorConverter))]
        public Color Fill_Transparency_On_Copy_Color { get; set; } = Color.White;



        [XmlIgnore]
        [Description("The default interpolation mode."), DisplayName("Interpolation Mode")]
        public InterpolationMode Default_Interpolation_Mode { get; set; } = InterpolationMode.NearestNeighbor;



        [Browsable(false)]
        [XmlElement("Default_Interpolation_Mode")]
        public int Default_Interpolation_ModeAsInt
        {
            get { return (int)Default_Interpolation_Mode; }
            set { Default_Interpolation_Mode = (InterpolationMode)value; }
        }


        [XmlIgnore]
        [Description("The back color of the image display."), DisplayName("Image Display Back Color")]
        [DefaultValue(typeof(Color), "white"),
        Editor(typeof(ColorEditor), typeof(UITypeEditor)),
        TypeConverter(typeof(_ColorConverter))]
        public Color Image_Box_Back_Color { get; set; } = Color.White;

        [XmlIgnore]
        [Description("The back color of the image display."), DisplayName("Image Display Back Color Alternate")]
        [DefaultValue(typeof(Color), "white"),
        Editor(typeof(ColorEditor), typeof(UITypeEditor)),
        TypeConverter(typeof(_ColorConverter))]
        public Color Image_Box_Back_Color_Alternate { get; set; } = Color.White;


        [Browsable(false)]
        [XmlElement("Image_Box_Back_Color")]
        public int BackColorAsDecimal
        {
            get { return ColorHelper.ColorToDecimal(Image_Box_Back_Color, ColorFormat.ARGB); }
            set { Image_Box_Back_Color = ColorHelper.DecimalToColor(value, ColorFormat.ARGB); }
        }
        [Browsable(false)]
        [XmlElement("Image_Box_Back_Color_alternate")]
        public int BackColorAlternateAsDecimal
        {
            get { return ColorHelper.ColorToDecimal(Image_Box_Back_Color_Alternate, ColorFormat.ARGB); }
            set { Image_Box_Back_Color_Alternate = ColorHelper.DecimalToColor(value, ColorFormat.ARGB); }
        }

        [Browsable(false)]
        [XmlElement("Fill_Transparency_On_Copy_Color")]
        public int Fill_Transparency_On_Copy_ColorAsDecimal
        {
            get { return ColorHelper.ColorToDecimal(Fill_Transparency_On_Copy_Color, ColorFormat.ARGB); }
            set { Fill_Transparency_On_Copy_Color = ColorHelper.DecimalToColor(value, ColorFormat.ARGB); }
        }

        public override string ToString()
        {
            return this.ProfileName;
        }
    }
}
