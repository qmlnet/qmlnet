%insert(runtime) %{
#ifdef _WIN32
typedef wchar_t TCHAR;
#else
typedef char TCHAR;
#endif
%}

%insert(runtime) %{
/* Callback for returning strings to C# without leaking memory */
typedef void * (SWIGSTDCALL* SWIG_CSharpQStringHelperCallback)(const TCHAR *);
static SWIG_CSharpQStringHelperCallback SWIG_csharp_qstring_callback = NULL;
%}

%pragma(csharp) imclasscode=%{
    protected class SWIGWQStringHelper {

        public delegate global::System.IntPtr SWIGQStringDelegate(global::System.IntPtr message);
        static SWIGQStringDelegate qstringDelegate = new SWIGQStringDelegate(CreateQString);

        [global::System.Runtime.InteropServices.DllImport("$dllimport", EntryPoint="SWIGRegisterQStringCallback_$module")]
        public static extern void SWIGRegisterQStringCallback_$module(SWIGQStringDelegate qstringDelegate);

        static global::System.IntPtr CreateQString([global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPTStr)]global::System.IntPtr cString) {
            if (cString == global::System.IntPtr.Zero) return global::System.IntPtr.Zero;
            var str = global::System.Runtime.InteropServices.Marshal.PtrToStringUni(cString);
            var handle = System.Runtime.InteropServices.GCHandle.Alloc(str);
            return System.Runtime.InteropServices.GCHandle.ToIntPtr(handle);
        }

        static SWIGWQStringHelper() {
            SWIGRegisterQStringCallback_$module(qstringDelegate);
        }
    }

    static protected SWIGWQStringHelper swigQStringHelper = new SWIGWQStringHelper();
%}

%insert(runtime) %{
#ifdef __cplusplus
extern "C"
#endif
SWIGEXPORT void SWIGSTDCALL SWIGRegisterQStringCallback_$module(SWIG_CSharpQStringHelperCallback callback) {
    SWIG_csharp_qstring_callback = callback;
}
%}

// Reference
//%typemap(ctype) QString& "TCHAR*"
%typemap(imtype,
    out="global::System.IntPtr",
    inattributes="[System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPTStr)]")
    QString& "string"
%typemap(cstype) QString& "string"
%typemap(in) QString& (QString temp) %{
    if($input) {
        temp = QString::fromWCharArray((wchar_t*)$input);
    }
    $1 = &temp;
%}
%typemap(out) QString& %{
    if(!$1->isNull()) {
        #ifdef _WIN32
        $result = (TCHAR*)SWIG_csharp_qstring_callback($1->toStdWString().c_str());
        #else
        $result = (TCHAR*)SWIG_csharp_qtring_callback($1->toUtf8().data());
        #endif
    } else {
        $result = NULL;
    }
%}
%typemap(csin) QString& "$csinput"
%typemap(csout) QString& {
    var pointer = $imcall;
    if (pointer == global::System.IntPtr.Zero) return null;
    var gcHandle =  System.Runtime.InteropServices.GCHandle.FromIntPtr(pointer);
    var ret = (string)gcHandle.Target;
    gcHandle.Free();
    return ret;
  }

// Value
//%typemap(ctype) QString "TCHAR*"
%typemap(imtype,
    out="global::System.IntPtr",
    inattributes="[System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPTStr)]")
    QString "string"
%typemap(cstype) QString "string"
%typemap(in) QString (QString temp) %{
    if($input) {
        $1 = QString::fromWCharArray((wchar_t*)$input);
    }
%}
%typemap(out) QString %{
    if(!$1.isNull()) {
        #ifdef _WIN32
        $result = (TCHAR*)SWIG_csharp_qstring_callback($1.toStdWString().c_str());
        #else
        $result = (TCHAR*)SWIG_csharp_qtring_callback($1.toUtf8().data());
        #endif
    } else {
        $result = NULL;
    }
%}
%typemap(csin) QString "$csinput"
%typemap(csout) QString {
    var pointer = $imcall;
    if (pointer == global::System.IntPtr.Zero) return null;
    var gcHandle =  System.Runtime.InteropServices.GCHandle.FromIntPtr(pointer);
    var ret = (string)gcHandle.Target;
    gcHandle.Free();
    return ret;
  }

// Pointer
//%typemap(ctype) QString* "TCHAR*"
%typemap(imtype,
    out="global::System.IntPtr",
    inattributes="[System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPTStr)]")
    QString* "string"
%typemap(cstype) QString* "string"
%typemap(in) QString* (QString temp) %{
    if($input) {
        temp = QString::fromWCharArray((wchar_t*)$input);
    }
    $1 = &temp;
%}
%typemap(out) QString* %{
    if(!$1->isNull()) {
        #ifdef _WIN32
        $result = (TCHAR*)SWIG_csharp_qstring_callback($1->toStdWString().c_str());
        #else
        $result = (TCHAR*)SWIG_csharp_qstring_callback($1->toUtf8().data());
        #endif
    } else {
        $result = NULL;
    }
%}
%typemap(csin) QString* "$csinput"
%typemap(csout) QString* {
    var pointer = $imcall;
    if (pointer == global::System.IntPtr.Zero) return null;
    var gcHandle =  System.Runtime.InteropServices.GCHandle.FromIntPtr(pointer);
    var ret = (string)gcHandle.Target;
    gcHandle.Free();
    return ret;
  }