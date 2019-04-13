#include "QTest.h"

#include <QtTest/qtestsystem.h>

using waitForCb = int (*)();

extern "C" {

Q_DECL_EXPORT void qtest_qwait(int ms)
{
    QTest::qWait(ms);
}

Q_DECL_EXPORT int qtest_qWaitFor(waitForCb cb, int ms)
{
    auto result = QTest::qWaitFor([&]() {
        auto result = cb();
        if (result == 1) {
            return true;
        } else {
            return false;
        }
    }, ms);
    if (result) {
        return 1;
    } else {
        return 0;
    }
}

}
