#!/usr/bin/env bash
set -e

ls /usr/lib/mono/
exit 1

SCRIPT_DIR=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )
QT_DIR=$SCRIPT_DIR/Qt

# Need to build Qt.
sudo apt-get install -y libgl1-mesa-dev

# Our Qt dev environment.
mkdir -p $QT_DIR
wget -O- -q https://github.com/qmlnet/qt-runtimes/releases/download/releases/qt-5.12.2-ad0689c-linux-x64-dev.tar.gz | tar xpz -C $QT_DIR

# Needed to reference net472 in our native packages.
sudo apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF
sudo apt-get install -y apt-transport-https ca-certificates
"deb https://download.mono-project.com/repo/ubuntu stable-xenial main" | sudo tee /etc/apt/sources.list.d/mono-official-stable.list
sudo apt-get update
sudo apt-get install -y mono-complete

export PATH=$QT_DIR/qt/bin:$PATH
export LD_LIBRARY_PATH=$TRAVIS_BUILD_DIR/src/native/output:$QT_DIR/qt/lib
export QT_PLUGIN_PATH=$QT_DIR/qt/plugins
export QML2_IMPORT_PATH=$QT_DIR/qt/qml

$TRAVIS_BUILD_DIR/build.sh ci