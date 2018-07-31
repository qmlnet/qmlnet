FROM ubuntu:16.04

RUN apt-get update
RUN apt-get install -y git

RUN apt-get install -y \
    build-essential \
    g++-mingw-w64-x86-64 \
    gcc-mingw-w64-x86-64

ENV QT_SRC_DIR=/qt-work
ENV QT_PREFIX=/usr/local

ADD qtbase.sh /
#RUN ./qtbase.sh && rm -rf $QT_SRC_DIR

