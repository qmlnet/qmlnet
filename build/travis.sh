#!/usr/bin/env bash
set -e

SCRIPT_DIR=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )

if [ "$TRAVIS_OS_NAME" == "linux" ]; then

$SCRIPT_DIR/travis.linux.sh

elif [ "$TRAVIS_OS_NAME" == "osx" ]; then

$SCRIPT_DIR/travis.osx.sh

else

echo "Unsupported os."
exit 1

fi