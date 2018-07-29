using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Xunit;
using FluentAssertions;
using Qml.Net.Internal;
using Qml.Net.Internal.Behaviors;
using Qml.Net.Internal.Qml;

namespace Qml.Net.Tests.Qml
{
    public class ViewModelContainer
    {
        public ViewModel ViewModel { get; set; } = new ViewModel();

        public void ChangeStringPropertyTo(string newValue)
        {
            ViewModel.StringProperty = newValue;
        }

        public void ChangeIntPropertyTo(int newValue)
        {
            ViewModel.IntProperty = newValue;
        }

        public bool? TestResult { get; set; }
    }

    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _stringProperty = "";
        public string StringProperty
        {
            get
            {
                return _stringProperty;
            }
            set
            {
                if (!Equals(value, _stringProperty))
                {
                    _stringProperty = value;
                    FirePropertyChanged();
                }
            }
        }

        private int _intProperty = 0;
        public int IntProperty
        {
            get
            {
                return _intProperty;
            }
            set
            {
                if (!Equals(value, _intProperty))
                {
                    _intProperty = value;
                    FirePropertyChanged();
                }
            }
        }

        private void FirePropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class MVVMInteropBehaviorTests : BaseQmlMVVMTestsWithInstance<ViewModelContainer>
    {
        [Fact]
        public void Does_register_property_changed_signal()
        {
            NetTestHelper.RunQml(qmlApplicationEngine,
            @"
                import QtQuick 2.0
                import tests 1.0
                ViewModelContainer {
                    id: viewModelContainer
                    Component.onCompleted: function() {
                        var vm = viewModelContainer.viewModel
                        vm.stringPropertyChanged.connect(function() {
                            viewModelContainer.testResult = true
                        })
                        viewModelContainer.changeStringPropertyTo('new value')
                    }
                }
            ");

            Instance.TestResult.Should().Be(true);
        }

        [Fact]
        public void Does_unregister_signal_on_ref_destroy()
        {
            qmlApplicationEngine.LoadData(@"
                import QtQuick 2.0
                import tests 1.0
                import testContext 1.0

                Item {
                    TestContext {
                        id: tc
                    }

                    Timer {
                        id: checkAndQuitTimer
                        running: false
                        interval: 1000
			            onTriggered: {
                            viewModelContainer.testResult = true;
                            viewModelContainer.changeStringPropertyTo('new value')
                            tc.quit()
			            }
                    }

                    ViewModelContainer {
                        id: viewModelContainer
                        Component.onCompleted: function() {
                            var vm = viewModelContainer.viewModel
                            vm.stringPropertyChanged.connect(function() {
                                viewModelContainer.testResult = false
                            })
                            vm = null
                            gc()

                            checkAndQuitTimer.running = true
                        }
                    }
                }
            ");

            ExecApplicationWithTimeout(3000).Should().Be(0);

            Instance.TestResult.Should().Be(true);
        }
    }
}
