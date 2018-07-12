%insert(runtime) %{
#if defined(Q_OS_WIN)
typedef wchar_t TCHAR;
#define _WCHAR_MODE
#elif defined(Q_OS_LINUX)
typedef char16_t TCHAR;
#define _UTF16_MODE
#else
typedef char TCHAR;
#define _CHAR_MODE
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
        #if defined(_WCHAR_MODE)
        temp = QString::fromWCharArray((TCHAR*)$input);
        #elif defined(_UTF16_MODE)
        temp = QString::fromUtf16((TCHAR*)$input);
        #else
        temp = QString::fromUtf8((TCHAR*)$input);
        #endif
    }
    $1 = &temp;
%}
%typemap(out) QString& %{
    if(!$1->isNull()) {
        #if defined(_WCHAR_MODE)
        $result = (TCHAR*)SWIG_csharp_qstring_callback($1->toStdWString().c_str());
        #elif defined(_UTF16_MODE)
        $result = (TCHAR*)SWIG_csharp_qstring_callback($1->toStdU16String().c_str());
        #elif defined(_CHAR_MODE)
        $result = (TCHAR*)SWIG_csharp_qstring_callback($1->toUtf8().data());
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
        #if defined(_WCHAR_MODE)
        $1 = QString::fromWCharArray((TCHAR*)$input);
        #elif defined(_UTF16_MODE)
        $1 = QString::fromUtf16((TCHAR*)$input);
        #else
        $1 = QString::fromUtf8((TCHAR*)$input);
        #endif
    }
%}
%typemap(out) QString %{
    if(!$1.isNull()) {
        #if defined(_WCHAR_MODE)
        $result = (TCHAR*)SWIG_csharp_qstring_callback($1.toStdWString().c_str());
        #elif defined(_UTF16_MODE)
        $result = (TCHAR*)SWIG_csharp_qstring_callback($1.toStdU16String().c_str());
        #elif defined(_CHAR_MODE)
        $result = (TCHAR*)SWIG_csharp_qstring_callback($1.toUtf8().data());
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
        #if defined(_WCHAR_MODE)
        temp = QString::fromWCharArray((TCHAR*)$input);
        #elif defined(_UTF16_MODE)
        temp = QString::fromUtf16((TCHAR*)$input);
        #else
        temp = QString::fromUtf8((TCHAR*)$input);
        #endif
    }
    $1 = &temp;
%}
%typemap(out) QString* %{
    if(!$1->isNull()) {
        #if defined(_WCHAR_MODE)
        $result = (TCHAR*)SWIG_csharp_qstring_callback($1->toStdWString().c_str());
        #elif defined(_UTF16_MODE)
        $result = (TCHAR*)SWIG_csharp_qstring_callback($1->toStdU16String().c_str());
        #elif defined(_CHAR_MODE)
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