#ifndef QGUIAPPLICATION_HELPERS_H
#define QGUIAPPLICATION_HELPERS_H

#include "qtnetcoreqml_global.h"
#include <QGuiApplication>
#include <QQmlApplicationEngine>
#include <vector>
#include <string>

extern "C" {
QGuiApplication* new_QGuiApplication(std::vector<std::string> argv);

void QQmlApplicationEngine_loadFile(QQmlApplicationEngine* instance, std::string filePath);

}
#endif // QGUIAPPLICATION_HELPERS_H
