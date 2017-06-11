%{
#include <QGuiApplication>
#include "qguiapplication_helpers.h"
%}

class QGuiApplication
{
    public:
    %extend {
        QGuiApplication(std::vector<std::string> argv);
    }
    int exec();
};