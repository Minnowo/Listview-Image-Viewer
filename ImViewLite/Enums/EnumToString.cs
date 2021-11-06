using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImViewLite.Enums
{
    public static class EnumToString
    {
        public static string CommandToString(Command cmd)
        {
            switch (cmd)
            {
                case Command.Nothing: return "Nothing";
                case Command.CopyImage:  return "Copy Image";
                case Command.PauseGif:  return "Pause Animation";
                case Command.InvertColor:  return "Invert Image Color";
                case Command.Grayscale: return "Make Image Grayscale";
                case Command.NextFrame: return "Next Image Frame";
                case Command.PreviousFrame: return "Previous Image Frame";
                case Command.UpDirectoryLevel: return "Up Directory Level";
                case Command.OpenSelectedDirectory: return "Open Selected Directory";
                case Command.MoveImage: return "Move Selected File(s)";
                case Command.RenameImage:  return "Rename Selected File";
                case Command.DeleteImage: return "Deleted Selected File(s)";
                case Command.ToggleAlwaysOnTop:return "Toggle Always On Top";
                case Command.OpenColorPicker:  return "Open Color Picker";
                case Command.OpenSettings: return "Open Settings";
            }
            return string.Empty;
        }
    }
}
