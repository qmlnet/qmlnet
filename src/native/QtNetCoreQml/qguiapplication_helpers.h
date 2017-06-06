#ifndef QGUIAPPLICATION_HELPERS_H
#define QGUIAPPLICATION_HELPERS_H

#include "qtnetcoreqml_global.h"
#include <QGuiApplication>
#include <vector>
#include <string>

extern "C" {
QGuiApplication* new_QGuiApplication(std::vector<std::string> argv);
}
#endif // QGUIAPPLICATION_HELPERS_H
