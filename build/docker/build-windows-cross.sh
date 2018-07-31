#!/usr/bin/env bash
set -e

SCRIPT_DIR=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )

docker build $SCRIPT_DIR/windows-cross \
    -t net-core-qml-windows-cross