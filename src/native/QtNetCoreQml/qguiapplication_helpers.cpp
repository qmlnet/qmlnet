#include "qguiapplication_helpers.h"
#include <iostream>

QGuiApplication* new_QGuiApplication(std::vector<std::string> argv)
{
    int size = argv.size();

    if(size == 0)
        return new QGuiApplication(size, NULL);

    std::vector<const char*> newArgs;
    for(int x = 0; x < argv.size(); x++)
    {
        newArgs.push_back(argv.at(x).c_str());
    }

        // // append the pointer to our interop structure.
        // wchar_t interopPointerBuffer[256];
        // swprintf_s(interopPointerBuffer, L"%llu", (unsigned long long)&interopData);
        // newArgs.push_back(const_cast<wchar_t*>(&interopPointerBuffer[0]));

        // std::wcout << (unsigned long long)&interopData;
        // std::wcout << newArgs.at(newArgs.size() - 1) << "\n";

        // int runResult = RunCoreCLR(newArgs.size(), &newArgs[0]);

        // qDebug() << "Finished running the corclr with a result of " << runResult;

    auto app = new QGuiApplication(size, (char**)&newArgs[0]);

    std::cout << "working\n";


    for(int x = 0; x < app->arguments().size(); x++)
    {
        //QString t = app->arguments().at(x);
        //std::cout << t.Data.data() << "\n";
    }

    app->exec();

    std::cout << "done\n";

    return app;
}
