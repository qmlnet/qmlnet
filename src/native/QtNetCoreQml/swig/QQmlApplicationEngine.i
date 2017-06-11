%{
#include <QQmlApplicationEngine>
%}

class QQmlApplicationEngine
{
    public:
    %extend {
        void loadFile(std::string filePath);
    }
};