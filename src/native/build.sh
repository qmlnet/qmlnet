#!/usr/bin/env bash
mkdir -p build

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

BUILD_DIR=$DIR/build
OUTPUT_DIR=$DIR/output

mkdir -p $BUILD_DIR
cd $BUILD_DIR
PREFIX=$OUTPUT_DIR qmake ../QmlNet
make
make install