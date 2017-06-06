%module QtNetCoreQml
%{
#include <QGuiApplication>
#include "qguiapplication_helpers.h"
%}
%include "std_vector.i"
%include "std_string.i"

%template(StringVector) std::vector< std::string >;

class QGuiApplication
{
    public:
    %extend {
        QGuiApplication(std::vector<std::string> argv);
    }
    int exec();
};