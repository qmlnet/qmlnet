#!/usr/bin/env bash
set -ex

SCRIPT_DIR=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )
QT_DIR=$SCRIPT_DIR/Qt

if [ ! -e $QT_DIR/5.11.1 ]; then

    # Checkout some helper scripts for installing Qt in ci.
    if [ ! -e $SCRIPT_DIR/qtci ]; then
        git clone https://github.com/benlau/qtci $SCRIPT_DIR/qtci
    fi
    export PATH=$PATH:$SCRIPT_DIR/qtci/bin:$SCRIPT_DIR/qtci/recipes

    DOWNLOAD_URL="https://download.qt.io/archive/qt/5.11/5.11.1/qt-opensource-mac-x64-5.11.1.dmg"
    INSTALLER=`basename $DOWNLOAD_URL`
    INSTALLER_NAME=${INSTALLER%.*}
    APPFILE=/Volumes/${INSTALLER_NAME}/${INSTALLER_NAME}.app/Contents/MacOS/${INSTALLER_NAME}

    if [ ! -e ${INSTALLER} ]; then
        echo "Downloading ${DOWNLOAD_URL}..."
        wget -c $DOWNLOAD_URL > /dev/null 2>&1
    fi

    echo "Mounting and extracting Qt..."
    hdiutil mount ${INSTALLER}
    export QT_CI_PACKAGES=qt.qt5.5111.qtvirtualkeyboard.clang_64,qt.qt5.5111.clang_64
    extract-qt-installer $APPFILE $QT_DIR

fi

export PATH=$PATH:$QT_DIR/5.11.1/clang_64/bin
export DYLD_LIBRARY_PATH=$TRAVIS_BUILD_DIR/src/native/output

# We need to source this script, so that DYLD_LIBRARY_PATH get's passed.
. $TRAVIS_BUILD_DIR/build.sh ci