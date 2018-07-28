FROM microsoft/dotnet:2.1.302-sdk-bionic

RUN apt-get update
RUN apt-get install -y qt5-default \
    qtbase5-private-dev \
    qtdeclarative5-private-dev \
    qtdeclarative5-qtquick2-plugin \
    qml-module-qtquick-layouts \
    qml-module-qtquick-controls2 \
    qml-module-qtquick-window2 \
    qtquickcontrols2-5-dev

RUN mkdir /work
WORKDIR /work