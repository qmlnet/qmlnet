using System;

namespace Qml.Net
{
    [Flags]
    public enum ApplicationAttribute
    {
        // ReSharper disable UnusedMember.Global
        ImmediateWidgetCreation = 0,
        MSWindowsUseDirect3DByDefault = 1, // Win only
        DontShowIconsInMenus = 2,
        NativeWindows = 3,
        DontCreateNativeWidgetSiblings = 4,
        PluginApplication = 5,
        MacPluginApplication = PluginApplication,  // ### Qt 6: remove me
        DontUseNativeMenuBar = 6,
        MacDontSwapCtrlAndMeta = 7,
        Use96Dpi = 8,
        X11InitThreads = 10,
        SynthesizeTouchForUnhandledMouseEvents = 11,
        SynthesizeMouseForUnhandledTouchEvents = 12,
        UseHighDpiPixmaps = 13,
        ForceRasterWidgets = 14,
        UseDesktopOpenGL = 15,
        UseOpenGLES = 16,
        UseSoftwareOpenGL = 17,
        ShareOpenGLContexts = 18,
        SetPalette = 19,
        EnableHighDpiScaling = 20,
        DisableHighDpiScaling = 21,
        UseStyleSheetPropagationInWidgetStyles = 22, // ### Qt 6: remove me
        DontUseNativeDialogs = 23,
        SynthesizeMouseForUnhandledTabletEvents = 24,
        CompressHighFrequencyEvents = 25,
        DontCheckOpenGLContextThreadAffinity = 26,
        DisableShaderDiskCache = 27,
        DontShowShortcutsInContextMenus = 28,
        CompressTabletEvents = 29,
        DisableWindowContextHelpButton = 30, // ### Qt 6: remove me
        // ReSharper restore UnusedMember.Global
    }
}