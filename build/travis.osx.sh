#!/usr/bin/env bash
set -ex

SCRIPT_DIR=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )
QT_DIR=$SCRIPT_DIR/Qt

wget -O- -q https://github.com/qmlnet/qmlnet/releases/download/ci/qt-5.12-osx-x64.tar.gz | tar xpz -C $QT_DIR

export PATH=$PATH:$QT_DIR/Qt/5.12.0/clang_64/bin
export DYLD_LIBRARY_PATH=$TRAVIS_BUILD_DIR/src/native/output

# We need to source this script, so that DYLD_LIBRARY_PATH get's passed.
. $TRAVIS_BUILD_DIR/build.sh ci