FROM ubuntu:xenial

RUN apt-get update

# install build tools and windows cross compiler
RUN set -x \
  && apt-get install -y -q swig3.0 \
  && mkdir /work

RUN apt-get clean

WORKDIR "/work"