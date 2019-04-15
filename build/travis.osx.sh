#!/usr/bin/env bash
set -e

exit 1

SCRIPT_DIR=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )
QT_DIR=$SCRIPT_DIR/Qt

# Our Qt dev environment.
mkdir -p $QT_DIR
wget -O- -q https://github.com/qmlnet/qt-runtimes/releases/download/releases/qt-5.12.2-ad0689c-osx-x64-dev.tar.gz | tar xpz -C $QT_DIR

# Needed to reference net472 in our native packages.
wget --retry-connrefused --waitretry=1 -q -O /tmp/mono.pkg https://download.mono-project.com/archive/5.20.1/macos-10-universal/MonoFramework-MDK-5.20.1.19.macos10.xamarin.universal.pkg
sudo installer -pkg /tmp/mono.pkg -target /

export PATH=$QT_DIR/qt/bin:$PATH
export DYLD_LIBRARY_PATH=$TRAVIS_BUILD_DIR/src/native/output:$QT_DIR/qt/lib
export QT_PLUGIN_PATH=$QT_DIR/qt/plugins
export QML2_IMPORT_PATH=$QT_DIR/qt/qml

# We need to source this script, so that DYLD_LIBRARY_PATH get's passed.
. $TRAVIS_BUILD_DIR/build.sh ci