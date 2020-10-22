using FluentAssertions;
using Xunit;

namespace Qml.Net.Tests.Qml
{
    public class SomeTestClass
    {
        public string PropertyWithoutSignal { get; set; }

        [NotifySignal] public string PropertyWithNotifySignal { get; set; }

        [NotifySignal("myCustomNotifySignal")] public string PropertyWithCustomNotifySignal { get; set; }

        public void TriggerDefaultSignal()
        {
            this.ActivateSignal("dynamic__PropertyWithoutSignalChanged");
        }

        public bool DefaultSignalReceived { get; set; } = false;

        public void TriggerNotifySignal()
        {
            this.ActivateNotifySignal(nameof(PropertyWithNotifySignal));
        }

        public bool NotifySignalReceived { get; set; } = false;

        public void TriggerCustomNotifySignal()
        {
            this.ActivateNotifySignal(nameof(PropertyWithCustomNotifySignal));
        }

        public bool CustomNotifySignalReceived { get; set; } = false;
    }

    public class AutoSignalTests : BaseQmlTestsWithInstance<SomeTestClass>
    {
        public AutoSignalTests()
            : base(true)
        {
        }

        [Fact]
        public void Does_register_autoSignals()
        {
            RunQmlTest(
                "testClass",
                @"
                testClass.dynamic__PropertyWithoutSignalChanged.connect(function() {
                    testClass.defaultSignalReceived = true;
                })
                testClass.propertyWithNotifySignalChanged.connect(function() {
                    testClass.notifySignalReceived = true;
                })
                testClass.myCustomNotifySignal.connect(function() {
                    testClass.customNotifySignalReceived = true;
                })
                testClass.triggerDefaultSignal();
                testClass.triggerNotifySignal();
                testClass.triggerCustomNotifySignal();
            ");

            Instance.DefaultSignalReceived.Should().Be(true);
            Instance.NotifySignalReceived.Should().Be(true);
            Instance.CustomNotifySignalReceived.Should().Be(true);
        }
    }
}