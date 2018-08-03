#!/usr/bin/env bash
set -e

curl -L https://github.com/pauldotknopf/Qml.Net/releases/download/ci/qt-${QT_VERSION}-linux-x64.tar.gz -o - \
    | tar -xzpf - -C /

echo -e '#!/usr/bin/env bash\nexec /Qt/'"${QT_VERSION}"'/gcc_64/bin/qmake $*' > /usr/bin/qmake
chmod +x /usr/bin/qmake