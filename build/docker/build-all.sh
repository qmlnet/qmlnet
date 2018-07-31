#!/usr/bin/env bash
set -e

SCRIPT_DIR=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )

$SCRIPT_DIR/build-linux-ci.sh
$SCRIPT_DIR/build-windows-cross.sh