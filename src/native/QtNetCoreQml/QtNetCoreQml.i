%module QtNetCoreQml

%include "std_vector.i"
%include "std_string.i"

%template(StringVector) std::vector< std::string >;

%include "QCoreApplication.i"
%include "QGuiApplication.i"
%include "QQmlApplicationEngine.i"
%include "QQmlRegisterType.i"