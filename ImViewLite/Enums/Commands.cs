using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImViewLite.Enums
{

    public enum Command
    {
        Nothing,
        CopyImage,
        OpenSelectedDirectory,
        UpDirectoryLevel,
        PauseGif,
        NextFrame,
        PreviousFrame,
        ToggleAlwaysOnTop,
        MoveImage,
        RenameImage,
        DeleteImage,
        InvertColor,
        Grayscale,
        OpenColorPicker,
        OpenSettings,
    }
}
