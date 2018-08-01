#!/usr/bin/env bash

SCRIPT_DIR=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )

$SCRIPT_DIR/docker/build-linux-ci.sh

docker run -it --rm \
    -v $SCRIPT_DIR/../:/work \
    -w /work \
    -e LD_LIBRARY_PATH=/work/src/native/output \
    -e QT_QPA_PLATFORM=offscreen \
    net-core-qml-linux-ci \
    ./build.sh ci