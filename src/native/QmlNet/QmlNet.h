#ifndef QMLNET_GLOBAL_H
#define QMLNET_GLOBAL_H

#include <QtCore/qglobal.h>

#define NetGCHandle void

#if _MSC_VER
    #include <Windows.h>
    #include <stdint.h>
    #include <Tchar.h>

    typedef const wchar_t* BCSTR;
    typedef const char* LPCSTR;
    typedef const wchar_t* LPWCSTR;

    #if UNICODE
        #define LPTSTR(value) L ##value;
        typedef LPWCSTR LPTCSTR;
    #else
        #define LPTSTR(value) value;
        typedef LPCSTR LPTCSTR;
    #endif
#endif

#if !_MSC_VER
    #define __declspec(dllexport)
    #define __stdcall

    typedef char16_t* BSTR;
    typedef const char16_t* BCSTR;

    typedef char* LPSTR;
    typedef const char* LPCSTR;

    typedef char16_t* LPWSTR;
    typedef const char16_t* LPWCSTR;

    #if UNICODE
        #define LPTSTR(value) (LPTSTR)u ##value;
        typedef LPWSTR LPTSTR;
        typedef LPWCSTR LPTCSTR;
    #else
        #define LPTSTR(value) value;
        typedef LPSTR LPTSTR;
        typedef LPCSTR LPTCSTR;
    #endif
#endif

enum Q_DECL_EXPORT NetVariantTypeEnum {
    NetVariantTypeEnum_Invalid = 0,
    NetVariantTypeEnum_Bool = 1,
    NetVariantTypeEnum_Char = 2,
    NetVariantTypeEnum_Int = 3,
    NetVariantTypeEnum_UInt = 4,
    NetVariantTypeEnum_Double = 5,
    NetVariantTypeEnum_String = 6,
    NetVariantTypeEnum_DateTime = 7,
    NetVariantTypeEnum_Object = 8
};


#endif // QMLNET_GLOBAL_H
