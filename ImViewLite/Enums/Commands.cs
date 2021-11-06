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
        NextImage,
        PreviousImage,
        ToggleAlwaysOnTop,
        OpenNewInstance,
        MoveImage,
        RenameImage,
        DeleteImage,
        InvertColor,
        Grayscale,
        OpenColorPicker,
        OpenSettings,
    }
}
