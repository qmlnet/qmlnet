#!/usr/bin/env bash
set -e

SCRIPT_DIR=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )
QT_DIR=$SCRIPT_DIR/Qt

mkdir -p $QT_DIR
wget -O- -q https://github.com/qmlnet/qt-runtimes/releases/download/releases/qt-5.12.2-de3f7b1-osx-x64-dev.tar.gz | tar xpz -C $QT_DIR

export PATH=$QT_DIR/qt/bin:$PATH
export DYLD_LIBRARY_PATH=$TRAVIS_BUILD_DIR/src/native/output

# We need to source this script, so that DYLD_LIBRARY_PATH get's passed.
. $TRAVIS_BUILD_DIR/build.sh ci