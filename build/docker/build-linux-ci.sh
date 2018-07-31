#!/usr/bin/env bash
set -e

SCRIPT_DIR=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )

docker build $SCRIPT_DIR/linux-ci \
    -t net-core-qml-linux-ci