#!/usr/bin/env bash

set -x

SCRIPT_DIR=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )
QT_DIR=$SCRIPT_DIR/Qt

sudo apt-get install -y libgl1-mesa-dev

mkdir -p $QT_DIR
wget -O- -q https://github.com/qmlnet/qt-runtimes/releases/download/releases/qt-5.12.2-ad0689c-linux-x64-dev.tar.gz | tar xpz -C $QT_DIR

export PATH=$QT_DIR/qt/bin:$PATH
export LD_LIBRARY_PATH=$TRAVIS_BUILD_DIR/src/native/output:$QT_DIR/qt/lib
export QT_PLUGIN_PATH=$QT_DIR/qt/plugins
export QML2_IMPORT_PATH=$QT_DIR/qt/qml

$TRAVIS_BUILD_DIR/build.sh ci