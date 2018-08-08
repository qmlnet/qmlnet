#!/usr/bin/env bash
set -e

SCRIPT_DIR=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )

# Before we run the build, get gitversion and generate a version.json.
nuget install GitVersion.CommandLine -Version 4.0.0-beta0012
mono ./GitVersion.CommandLine.4.0.0-beta0012/tools/GitVersion.exe > version.json

if [ "$TRAVIS_OS_NAME" == "linux" ]; then

$SCRIPT_DIR/travis.linux.sh

elif [ "$TRAVIS_OS_NAME" == "osx" ]; then

$SCRIPT_DIR/travis.osx.sh

else

echo "Unsupported os."
exit 1

fi