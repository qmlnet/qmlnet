FROM ubuntu:16.04

ARG QT_VERSION

ADD install-packages.sh /
RUN /install-packages.sh && rm /install-packages.sh

ADD install-dotnet.sh /
RUN /install-dotnet.sh && rm /install-dotnet.sh

ADD install-qt.sh /
RUN /install-qt.sh && rm /install-qt.sh

ENV QT_PLUGIN_PATH=/Qt/$QT_VERSION/gcc_64/plugins
ENV QML2_IMPORT_PATH=/Qt/$QT_VERSION/gcc_64/qml

RUN mkdir /work
WORKDIR /work