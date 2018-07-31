#!/usr/bin/env bash
set -e
set -x

mkdir -p $QT_SRC_DIR

cd $QT_SRC_DIR
git clone --branch v5.11.1 --depth 1 https://github.com/qt/qtbase.git
cd $QT_SRC_DIR/qtbase

./configure -release \
    -c++std c++11 \
    -release \
    -static \
    -opensource \
    -confirm-license \
    -opengl desktop \
    -xplatform win32-g++ \
    -prefix $QT_PREFIX \
    -device-option CROSS_COMPILE=/usr/bin/x86_64-w64-mingw32- \
    -nomake examples \
    -nomake tests \
    -skip qtwebengine \
    -verbose

make -j 4
make install