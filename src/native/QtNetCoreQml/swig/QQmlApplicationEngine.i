%{
#include <QQmlApplicationEngine>
%}

%typemap(csclassmodifiers) QQmlApplicationEngine "public partial class"

class QQmlApplicationEngine
{
    public:
    %extend {
        void loadFile(std::string filePath);
    }
};