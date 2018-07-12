%{
#include <QGuiApplication>
#include "qguiapplication_helpers.h"
%}

%feature("director") GuiThreadContextTriggerCallback;

class GuiThreadContextTriggerCallback {
public:
    virtual ~GuiThreadContextTriggerCallback() {}
	virtual void onGuiThreadContextTrigger();
};

%typemap(csclassmodifiers) QGuiApplication "public partial class"
%typemap(csconstruct, excode=SWIGEXCODE) QGuiApplication %{: this($imcall, true) {$excode
    OnCreate();
  }
%}
%typemap(cscode) QGuiApplication %{
  partial void OnCreate();
%}

class QGuiApplication
{
    public:
    %extend {
        QGuiApplication(std::vector<std::string> argv);
        void setGuiThreadContextTriggerCallback(GuiThreadContextTriggerCallback* callback);
        void requestGuiThreadContextTrigger();
    }
    int exec();
};