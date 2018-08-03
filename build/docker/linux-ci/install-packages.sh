#!/usr/bin/env bash
set -e

apt-get update
apt-get install -y build-essential \
    apt-transport-https \
    git \
    p7zip-full \
    curl \
    wget \
    libgl1-mesa-dev \
    libfontconfig1 \
    libglib2.0-0 \
    libgtk-3-0
apt-get clean
