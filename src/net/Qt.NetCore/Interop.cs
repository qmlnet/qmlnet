using System;
using AdvancedDLSupport;

namespace Qt.NetCore
{
    public static class Interop
    {
        static Interop()
        {
            Environment.SetEnvironmentVariable("DYLD_LIBRARY_PATH", "/Users/pknopf/git/net-core-qml/src/native/build-QtNetCoreQml-Desktop_Qt_5_11_1_clang_64bit-Debug");
            Callbacks = NativeLibraryBuilder.Default.ActivateInterface<ICallbacks>("QtNetCoreQml");
        }
        
        public static ICallbacks Callbacks { get; }
    }
}