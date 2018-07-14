#!/usr/bin/env bash
set -e
set -x

SCRIPT_DIR=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )

docker build $SCRIPT_DIR/docker \
    -f $SCRIPT_DIR/docker/Dockerfile.build \
    -t net-core-qml-build

docker run -it --rm \
    -v $SCRIPT_DIR/../:/work \
    -w /work \
    -e LD_LIBRARY_PATH=/work/src/native/output \
    -e QT_QPA_PLATFORM=offscreen \
    net-core-qml-build \
    ./build.sh ci