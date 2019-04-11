#!/usr/bin/env bash

SCRIPT_DIR=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )
QT_DIR=$SCRIPT_DIR/Qt

sudo apt-get install -y libgl1-mesa-dev

mkdir -p $QT_DIR
wget -O- -q https://github.com/qmlnet/qt-runtimes/releases/download/releases/qt-5.12.2-de3f7b1-linux-x64-dev.tar.gz | tar xpz -C $QT_DIR

export PATH=$QT_DIR/qt/bin:$PATH
export LD_LIBRARY_PATH=$TRAVIS_BUILD_DIR/src/native/output

$TRAVIS_BUILD_DIR/build.sh ci