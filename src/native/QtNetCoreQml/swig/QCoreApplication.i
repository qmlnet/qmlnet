%{
#include <QCoreApplication>
%}

namespace Qt
{
enum ApplicationAttribute
{
    AA_ImmediateWidgetCreation = 0,
    AA_MSWindowsUseDirect3DByDefault = 1, // Win only
    AA_DontShowIconsInMenus = 2,
    AA_NativeWindows = 3,
    AA_DontCreateNativeWidgetSiblings = 4,
    AA_PluginApplication = 5,
    AA_MacPluginApplication = AA_PluginApplication,  // ### Qt 6: remove me
    AA_DontUseNativeMenuBar = 6,
    AA_MacDontSwapCtrlAndMeta = 7,
    AA_Use96Dpi = 8,
    AA_X11InitThreads = 10,
    AA_SynthesizeTouchForUnhandledMouseEvents = 11,
    AA_SynthesizeMouseForUnhandledTouchEvents = 12,
    AA_UseHighDpiPixmaps = 13,
    AA_ForceRasterWidgets = 14,
    AA_UseDesktopOpenGL = 15,
    AA_UseOpenGLES = 16,
    AA_UseSoftwareOpenGL = 17,
    AA_ShareOpenGLContexts = 18,
    AA_SetPalette = 19,
    AA_EnableHighDpiScaling = 20,
    AA_DisableHighDpiScaling = 21,
    AA_UseStyleSheetPropagationInWidgetStyles = 22, // ### Qt 6: remove me
    AA_DontUseNativeDialogs = 23,
    AA_SynthesizeMouseForUnhandledTabletEvents = 24,
    AA_CompressHighFrequencyEvents = 25,
    AA_DontCheckOpenGLContextThreadAffinity = 26,

    // Add new attributes before this line
    AA_AttributeCount
};
}

%nodefaultctor;
class QCoreApplication
{
    public:
    static void setAttribute(Qt::ApplicationAttribute attribute, bool on = true);
    int exec();
};
%clearnodefaultctor; 