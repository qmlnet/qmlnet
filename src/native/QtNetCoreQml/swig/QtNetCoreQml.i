%module(directors="1") QtNetCoreQml
%{
#include <QDebug>
%}
%include "Initialization.i"

%include "std_vector.i"
%include "std_string_custom.i"

%template(StringVector) std::vector< std::string >;

%include "void.i"
%include "wchar.i"
%include "Global.i"
%include "QString.i"
%include "QDateTime.i"
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
%include "NetTestHelper.i"