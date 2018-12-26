#!/usr/bin/env bash
set -e

apt-get update
wget -q https://packages.microsoft.com/config/ubuntu/16.04/packages-microsoft-prod.deb
dpkg -i packages-microsoft-prod.deb
apt-get install -y apt-transport-https
apt-get update
apt-get install -y dotnet-sdk-2.2
apt-get clean