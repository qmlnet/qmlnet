%module(directors="1") QtNetCoreQml

%include "std_vector.i"
%include "std_string_custom.i"

%template(StringVector) std::vector< std::string >;

%include "void.i"
%include "Global.i"
%include "NetInstance.i"
%include "NetVariant.i"
%include "NetTypeInfo.i"
%include "NetTypeInfoMethod.i"
%include "NetTypeInfoProperty.i"
%include "NetTypeInfoManager.i"
%include "QCoreApplication.i"
%include "QGuiApplication.i"
%include "QQmlApplicationEngine.i"
%include "QQmlRegisterType.i"