%pragma(csharp) imclasscode=%{
    protected class QmlCallbackHelper {
        static Callback _callback;

        static QmlCallbackHelper() {
            _callback = new Callback();
            NetTypeInfoManager.setCallbacks(_callback);
        }
    }

    static protected QmlCallbackHelper qmlCallbackHelper = new QmlCallbackHelper();
%}