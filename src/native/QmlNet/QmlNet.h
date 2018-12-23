#ifndef QMLNET_GLOBAL_H
#define QMLNET_GLOBAL_H

#include <QtCore/qglobal.h>

#define NetGCHandle void

#if _MSC_VER
    typedef const wchar_t* BCSTR;
    typedef const char* LPCSTR;
    typedef const char16_t* LPWCSTR;
    typedef char16_t* LPWSTR;
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

enum NetVariantTypeEnum {
    NetVariantTypeEnum_Invalid = 0,
    NetVariantTypeEnum_Bool,
    NetVariantTypeEnum_Char,
    NetVariantTypeEnum_Int,
    NetVariantTypeEnum_UInt,
    NetVariantTypeEnum_Long,
    NetVariantTypeEnum_ULong,
    NetVariantTypeEnum_Float,
    NetVariantTypeEnum_Double,
    NetVariantTypeEnum_String,
    NetVariantTypeEnum_DateTime,
    NetVariantTypeEnum_Object,
    NetVariantTypeEnum_JSValue,
};


#endif // QMLNET_GLOBAL_H
