%module(directors="1") QtNetCoreQml

%include "std_vector.i"
%include "std_string_custom.i"

%template(StringVector) std::vector< std::string >;

%include "NetInstance.i"
%include "NetTypeInfo.i"
%include "QCoreApplication.i"
%include "QGuiApplication.i"
%include "QQmlApplicationEngine.i"
%include "QQmlRegisterType.i"