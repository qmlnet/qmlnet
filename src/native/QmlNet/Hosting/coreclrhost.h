// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// APIs for hosting CoreCLR
//

#ifndef CORECLR_HOST_H
#define CORECLR_HOST_H

#ifdef _WIN32
typedef unsigned short CORECLR_CHAR_TYPE;
#else
typedef char CORECLR_CHAR_TYPE;
#endif

// For each hosting API, we define a function prototype and a function pointer
// The prototype is useful for implicit linking against the dynamic coreclr
// library and the pointer for explicit dynamic loading (dlopen, LoadLibrary)
#define CORECLR_HOSTING_API(function, ...) \
    extern "C" int function(__VA_ARGS__); \
    typedef int (*function##_ptr)(__VA_ARGS__)

CORECLR_HOSTING_API(coreclr_initialize,
            const char* exePath,
            const char* appDomainFriendlyName,
            int propertyCount,
            const char** propertyKeys,
            const char** propertyValues,
            void** hostHandle,
            unsigned int* domainId);

CORECLR_HOSTING_API(coreclr_shutdown,
            void* hostHandle,
            unsigned int domainId);

CORECLR_HOSTING_API(coreclr_shutdown_2,
            void* hostHandle,
            unsigned int domainId,
            int* latchedExitCode);

CORECLR_HOSTING_API(coreclr_create_delegate,
            void* hostHandle,
            unsigned int domainId,
            const char* entryPointAssemblyName,
            const char* entryPointTypeName,
            const char* entryPointMethodName,
            void** delegate);

CORECLR_HOSTING_API(coreclr_execute_assembly,
            void* hostHandle,
            unsigned int domainId,
            int argc,
            const char** argv,
            const char* managedAssemblyPath,
            unsigned int* exitCode);

// hostfxr
CORECLR_HOSTING_API(hostfxr_get_native_search_directories,
            const int argc,
            const char* argv[],
            char buffer[],
            int buffer_size,
            int* required_buffer_size);

CORECLR_HOSTING_API(hostfxr_main_startupinfo,
            const int argc,
            const char* argv[],
            const char* host_path,
            const char* dotnet_root,
            const char* app_path);

CORECLR_HOSTING_API(hostfxr_main,
            const int argc,
            const CORECLR_CHAR_TYPE* argv[]);

#undef CORECLR_HOSTING_API
                      
#endif // CORECLR_HOST_H
