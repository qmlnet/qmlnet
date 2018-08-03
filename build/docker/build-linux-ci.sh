#!/usr/bin/env bash
set -e

SCRIPT_DIR=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )

docker build \
    --build-arg QT_VERSION=5.11.1 \
    -t qmlnet/linux-ci:qt-5.11.1 \
    $SCRIPT_DIR/linux-ci