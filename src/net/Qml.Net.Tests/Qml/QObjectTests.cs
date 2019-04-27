using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using Xunit;

namespace Qml.Net.Tests.Qml
{
    public class QObjectTests : BaseQmlTests<QObjectTests.QObjectTestsQml>
    {
        public class QObjectTestsQml
        {
            public virtual void Method(dynamic value)
            {
            }
        }

        [Fact]
        public void Exception_throw_when_using_invalid_methods()
        {
            AssertQObject(qObject =>
            {
                // Valid
                qObject.InvokeMethod("testSlot");
                Assert.Throws<Exception>(() => { qObject.InvokeMethod("nonexistant"); });
            });
        }

        [Fact]
        public void Exception_thrown_when_using_invalid_properties()
        {
            AssertQObject(qObject =>
            {
                // Valid
                qObject.SetProperty("readAndWrite", 3);
                Assert.Throws<Exception>(() => { qObject.SetProperty("nonexistant", 3); });
                Assert.Throws<Exception>(() => { qObject.GetProperty("nonexistant"); });
            });
        }

        [Fact]
        public void Exception_thrown_when_attaching_to_invalid_signal()
        {
            AssertQObject(qObject =>
            {
                // Valid
                var handler = qObject.AttachSignal("testSignal", parameters =>
                { 
                });
                handler.Dispose();
                Assert.Throws<Exception>(() =>
                {
                    qObject.AttachSignal(
                        "nonexistant",
                        parameters =>
                        {
                        });
                });
            });
        }
        
        [Fact]
        public void Exception_thrown_when_attaching_to_invalid_notify_signal()
        {
            AssertQObject(qObject =>
            {
                // Valid
                var handler = qObject.AttachNotifySignal("propWithSignal", parameters =>
                {
                });
                handler.Dispose();
                Assert.Throws<Exception>(() =>
                { 
                    qObject.AttachNotifySignal(
                        "readAndWrite",
                        parameters =>
                        { 
                        });
                });
                Assert.Throws<Exception>(() =>
                {
                    qObject.AttachNotifySignal(
                        "nonexistant",
                        parameters =>
                        {
                        });
                });
            });
        }
        
        [Fact]
        public void Can_get_property_on_qobject()
        {
            AssertQObject(qObject =>
            {
                qObject.GetProperty("readOnly").Should().Be(3);
            });
        }
        
        [Fact]
        public void Can_set_property_on_qobject()
        {
            AssertQObject(qObject =>
            {
                // No real way to test this.
                // I suppose it doesn't throw, eh?
                qObject.SetProperty("writeOnly", 3);
            });
        }
        
        [Fact]
        public void Can_set_and_get_property_on_qobject()
        {
            AssertQObject(qObject =>
            {
                qObject.SetProperty("readAndWrite", 340);
                qObject.GetProperty("readAndWrite").Should().Be(340);
            });
        }

        [Fact]
        public void Can_get_set_object_name()
        {
            AssertQObject(qObject =>
            {
                var d = (dynamic)qObject;
                d.objectName = "testtt";
                ((string)d.objectName).Should().Be("testtt");
            });
        }

        [Fact]
        public void Can_invoke_method_on_qobject_as_dynamic()
        {
            AssertQObject(qObject =>
            {
                var d = (dynamic)qObject;
                var result = (int)d.testSlotInt(23);
                result.Should().Be(23);
            });
        }

        [Fact]
        public void Can_set_random_types_on_property()
        {
            AssertQObject(qObject =>
            {
                qObject.GetProperty("variantProperty").Should().BeNull();
                
                qObject.SetProperty("variantProperty", true);
                qObject.GetProperty("variantProperty").Should().Be(true);
                
                qObject.SetProperty("variantProperty", 'T');
                qObject.GetProperty("variantProperty").Should().Be('T');
                
                qObject.SetProperty("variantProperty", int.MinValue);
                qObject.GetProperty("variantProperty").Should().Be(int.MinValue);
                
                qObject.SetProperty("variantProperty", uint.MaxValue);
                qObject.GetProperty("variantProperty").Should().Be(uint.MaxValue);
         
                qObject.SetProperty("variantProperty", long.MaxValue);
                qObject.GetProperty("variantProperty").Should().Be(long.MaxValue);

                qObject.SetProperty("variantProperty", ulong.MaxValue);
                qObject.GetProperty("variantProperty").Should().Be(ulong.MaxValue);

                qObject.SetProperty("variantProperty", float.MaxValue);
                qObject.GetProperty("variantProperty").Should().Be(float.MaxValue);

                qObject.SetProperty("variantProperty", double.MaxValue);
                qObject.GetProperty("variantProperty").Should().Be(double.MaxValue);

                qObject.SetProperty("variantProperty", "");
                qObject.GetProperty("variantProperty").Should().Be("");
                
                qObject.SetProperty("variantProperty", "test");
                qObject.GetProperty("variantProperty").Should().Be("test");

                var dateTime = DateTimeOffset.Now;
                dateTime = new DateTimeOffset(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond, dateTime.Offset);
                qObject.SetProperty("variantProperty", dateTime);
                var resultDateTime = (DateTimeOffset)qObject.GetProperty("variantProperty");
                resultDateTime = new DateTimeOffset(resultDateTime.Year, resultDateTime.Month, resultDateTime.Day, resultDateTime.Hour, resultDateTime.Minute, resultDateTime.Second, resultDateTime.Millisecond, resultDateTime.Offset);
                resultDateTime.Should().Be(dateTime);
                
                var o = new object();
                qObject.SetProperty("variantProperty", o);
                qObject.GetProperty("variantProperty").Should().BeSameAs(o);
                
                qObject.SetProperty("objectName", "ttttt");
                qObject.SetProperty("variantProperty", qObject);
                var result = qObject.GetProperty("variantProperty");
                result.Should().NotBeNull();
                var resultQObject = result as INetQObject;
                resultQObject.Should().NotBeNull();
                resultQObject.GetProperty("objectName").Should().Be("ttttt");
            });
        }

        [Fact]
        public void Can_return_random_types_from_method()
        {
            AssertQObject(qObject =>
            {
                qObject.InvokeMethod("testVariantReturn").Should().Be(null);
                
                qObject.SetProperty("variantProperty", true);
                qObject.InvokeMethod("testVariantReturn").Should().Be(true);

                qObject.SetProperty("variantProperty", 'T');
                qObject.InvokeMethod("testVariantReturn").Should().Be('T');
                
                qObject.SetProperty("variantProperty", int.MinValue);
                qObject.InvokeMethod("testVariantReturn").Should().Be(int.MinValue);
                
                qObject.SetProperty("variantProperty", uint.MaxValue);
                qObject.InvokeMethod("testVariantReturn").Should().Be(uint.MaxValue);
         
                qObject.SetProperty("variantProperty", long.MaxValue);
                qObject.InvokeMethod("testVariantReturn").Should().Be(long.MaxValue);

                qObject.SetProperty("variantProperty", ulong.MaxValue);
                qObject.InvokeMethod("testVariantReturn").Should().Be(ulong.MaxValue);

                qObject.SetProperty("variantProperty", float.MaxValue);
                qObject.InvokeMethod("testVariantReturn").Should().Be(float.MaxValue);

                qObject.SetProperty("variantProperty", double.MaxValue);
                qObject.InvokeMethod("testVariantReturn").Should().Be(double.MaxValue);

                qObject.SetProperty("variantProperty", "");
                qObject.InvokeMethod("testVariantReturn").Should().Be("");
                
                qObject.SetProperty("variantProperty", "test");
                qObject.InvokeMethod("testVariantReturn").Should().Be("test");

                var dateTime = DateTimeOffset.Now;
                dateTime = new DateTimeOffset(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond, dateTime.Offset);
                qObject.SetProperty("variantProperty", dateTime);
                var resultDateTime = (DateTimeOffset)qObject.InvokeMethod("testVariantReturn");
                resultDateTime = new DateTimeOffset(resultDateTime.Year, resultDateTime.Month, resultDateTime.Day, resultDateTime.Hour, resultDateTime.Minute, resultDateTime.Second, resultDateTime.Millisecond, resultDateTime.Offset);
                resultDateTime.Should().Be(dateTime);
                
                var o = new object();
                qObject.SetProperty("variantProperty", o);
                qObject.InvokeMethod("testVariantReturn").Should().BeSameAs(o);
                
                qObject.SetProperty("objectName", "ttttt");
                qObject.SetProperty("variantProperty", qObject);
                var result = qObject.InvokeMethod("testVariantReturn");
                result.Should().NotBeNull();
                var resultQObject = result as INetQObject;
                resultQObject.Should().NotBeNull();
                resultQObject.GetProperty("objectName").Should().Be("ttttt");
            });
        }

        [Fact]
        public void Can_set_qobject_on_global_context_property()
        {
            AssertQObject(qObject =>
            {
                var d = (dynamic)qObject;
                var property = Guid.NewGuid().ToString().Replace("-", "");
                d.objectName = property;
                qmlApplicationEngine.SetContextProperty(property, d);
                var result = qmlApplicationEngine.GetContextProperty(property);
                (result is INetQObject).Should().BeTrue();
                result.Should().NotBeNull();
                ((string)d.objectName).Should().Be(property);
            });
        }

        [Fact]
        public void Can_invoke_method_on_qobject()
        {
            AssertQObject(qObject =>
            {
                // TODO: Assert
                qObject.InvokeMethod("testSlot");
            });
        }

        [Fact]
        public void Can_attach_notify_signal_to_qobject()
        {
            AssertQObject(qObject =>
            {
                var raised = false;
                var expected = 0;
                var handler = qObject.AttachNotifySignal("propWithSignal", parameters =>
                {
                    raised = true;
                    parameters.Count.Should().Be(1);
                    parameters[0].Should().Be(expected);
                });

                expected = 43;
                qObject.SetProperty("propWithSignal", 43);
                raised.Should().BeTrue();

                raised = false;
                qObject.SetProperty("propWithSignal", 43);
                raised.Should().BeFalse();

                expected = 44;
                raised = false;
                qObject.SetProperty("propWithSignal", 44);
                raised.Should().BeTrue();
            });
        }
        
        [Fact]
        public void Can_attach_signal_to_qobject()
        {
            AssertQObject(qObject =>
            {
                var raised = 0;
                var handler = qObject.AttachSignal("testSignal", parameters =>
                {
                    raised++;
                });
                handler.Should().NotBeNull();
                
                qObject.InvokeMethod("testSlot");
                raised.Should().Be(1);
                
                handler.Dispose();

                qObject.InvokeMethod("testSlot");
                raised.Should().Be(1);
            });
        }

        [Fact]
        public void Can_attach_signal_with_arg_to_qobject()
        {
            AssertQObject(qObject =>
            {
                var raised = false;
                var handler = qObject.AttachSignal("testSignalWithArg", parameters =>
                {
                    raised = true;
                    parameters.Should().NotBeNull();
                    parameters.Count.Should().Be(1);
                    parameters[0].Should().Be(33);
                });

                qObject.InvokeMethod("testSlotWithArg", 33);
                
                handler.Dispose();
                raised = false;
                
                qObject.InvokeMethod("testSlotWithArg", 33);
                raised.Should().BeFalse();
            });
        }

        [Fact]
        public void Can_use_bool_on_qobject()
        {
            AssertQObject(qObject =>
            {
                AssertValue(qObject, "Bool", true);
                AssertValue(qObject, "Bool", false);
            });
        }
        
        [Fact]
        public void Can_use_char_on_qobject()
        {
            AssertQObject(qObject =>
            {
                AssertValue(qObject, "Char", 'T');
            });
        }
        
        [Fact]
        public void Can_use_int_on_qobject()
        {
            AssertQObject(qObject =>
            {
                AssertValue(qObject, "Int", int.MinValue);
                AssertValue(qObject, "Int", int.MaxValue);
            });
        }
        
        [Fact]
        public void Can_use_uint_on_qobject()
        {
            AssertQObject(qObject =>
            {
                AssertValue(qObject, "UInt", uint.MinValue);
                AssertValue(qObject, "UInt", uint.MaxValue);
            });
        }
        
        [Fact]
        public void Can_use_long_on_qobject()
        {
            AssertQObject(qObject =>
            {
                AssertValue(qObject, "Long", -23423);
            });
        }
        
        [Fact]
        public void Can_use_ulong_on_qobject()
        {
            AssertQObject(qObject =>
            {
                AssertValue(qObject, "ULong", 2323);
            });
        }

        [Fact]
        public void Can_use_float_with_qobject()
        {
            AssertQObject(qObject =>
            {
                AssertValue(qObject, "Float", float.MinValue);
                AssertValue(qObject, "Float", float.MaxValue);
            });
        }
        
        [Fact]
        public void Can_use_double_with_qobject()
        {
            AssertQObject(qObject =>
            {
                AssertValue(qObject, "Double", double.MinValue);
                AssertValue(qObject, "Double", double.MaxValue);
            });
        }
        
        [Fact]
        public void Can_use_string_with_qobject()
        {
            AssertQObject(qObject =>
            {
                AssertValue(qObject, "String", "test");
                AssertValue(qObject, "String", "");
                AssertValue(qObject, "String", null);
            });
        }
        
        [Fact]
        public void Can_use_datetime_with_qobject()
        {
            AssertQObject(qObject =>
            {
                var value = DateTimeOffset.Now;
                value = new DateTimeOffset(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second, value.Millisecond, value.Offset);
                AssertValue(qObject, "DateTime", value);
            });
        }
        
        [Fact]
        public void Can_use_qobject_with_qobject()
        {
            AssertQObject(qObject =>
            {
                qObject.SetProperty("objectName", "testttdd");
                AssertValue(qObject, "QObject", qObject, result =>
                {
                    result.Should().BeAssignableTo<INetQObject>()
                        .Subject.GetProperty("objectName").Should().Be("testttdd");
                });
            });
        }
        
        [Fact]
        public void Can_used_typed_base_qobject_with_qobject()
        {
            AssertQObject(qObject =>
            {
                qObject.SetProperty("objectName", "wer");
                AssertValue(qObject, "TypedBaseQObject", qObject, result =>
                {
                    var resultQObject = result.Should().NotBeNull().And.BeAssignableTo<INetQObject>().Subject;
                    resultQObject.GetProperty("objectName").Should().Be("wer");
                });
            });
        }
        
        [Fact]
        public void Can_used_typed_qobject_with_qobject()
        {
            AssertQObject(qObject =>
            {
                qObject.SetProperty("objectName", "wer");
                AssertValue(qObject, "TypedQObject", qObject, result =>
                {
                    var resultQObject = result.Should().NotBeNull().And.BeAssignableTo<INetQObject>().Subject;
                    resultQObject.GetProperty("objectName").Should().Be("wer");
                });
            });
        }
        
        [Fact]
        public void Value_is_null_when_using_invalid_qobject_type_with_qobject()
        {
            AssertQObject(qObject =>
            {
                AssertValue(qObject, "TypedDerivedQObject", qObject, result =>
                {
                    result.Should().BeNull();
                });
            });
        }
        
        [Fact]
        public void Can_use_object_with_qobject()
        {
            AssertQObject(qObject =>
            {
                var o = new object();
                AssertValue(qObject, "QObject", o, result =>
                {
                    result.Should().BeSameAs(o);
                });
                
                AssertValue(qObject, "QObject", null, result =>
                {
                    result.Should().BeNull();
                });
            });
        }
        
        [Fact]
        public void Can_use_qint32_with_qobject()
        {
            AssertQObject(qObject =>
            {
                AssertValue(qObject, "QInt32", int.MinValue);
                AssertValue(qObject, "QInt32", int.MaxValue);
            });
        }
        
        [Fact]
        public void Can_use_quint32_with_qobject()
        {
            AssertQObject(qObject =>
            {
                AssertValue(qObject, "QUInt32", uint.MinValue);
                AssertValue(qObject, "QUInt32", uint.MaxValue);
            });
        }
        
        [Fact]
        public void Can_use_qint64_with_qobject()
        {
            AssertQObject(qObject =>
            {
                AssertValue(qObject, "QInt64", long.MinValue);
                AssertValue(qObject, "QInt64", long.MaxValue);
            });
        }
        
        [Fact]
        public void Can_use_quint64_with_qobject()
        {
            AssertQObject(qObject =>
            {
                AssertValue(qObject, "QUInt64", ulong.MinValue);
                AssertValue(qObject, "QUInt64", ulong.MaxValue);
            });
        }

        [Fact]
        public void Can_use_variant_list_with_qobject()
        {
            AssertQObject(qObject =>
            {
                AssertValue(qObject, "QVariantList", new List<int> { 3 }, result =>
                {
                    result.Should().NotBeNull();
                    var list = result.Should().BeAssignableTo<IList<object>>().Subject;
                    list.Should().HaveCount(1);
                    list[0].Should().Be(3);
                });

                qObject.SetProperty("objectName", "tetttt");
                AssertValue(qObject, "QVariantList", new List<object> { qObject }, result =>
                {
                    result.Should().NotBeNull();
                    var list = result.Should().BeAssignableTo<IList<object>>().Subject;
                    list.Should().HaveCount(1);
                    var resultQObject = list[0].Should().BeAssignableTo<INetQObject>().Subject;
                    resultQObject.GetProperty("objectName").Should().Be("tetttt");
                });
            });
        }

        [Fact]
        public void Exception_thrown_when_using_wrong_number_of_parameters()
        {
            AssertQObject(qObject =>
            {
                Assert.Throws<Exception>(() => { qObject.InvokeMethod("testSlotInt"); });
                Assert.Throws<Exception>(() => { qObject.InvokeMethod("testSlotInt", 1, 1); });
            });
        }

        [Fact]
        public void Can_invoke_signal()
        {
            AssertQObject(qObject =>
            {
                var raised = false;
                using (qObject.AttachSignal("testSignalInt", parameters =>
                {
                    raised = true;
                    parameters.Count.Should().Be(1);
                    parameters[0].Should().Be(3);
                }))
                {
                    qObject.InvokeMethod("testSignalInt", 3).Should().BeNull();
                }

                raised.Should().BeTrue();
            });
        }

        [Fact]
        public void Can_build_qobject()
        {
            AssertQObject(_ =>
            {
                var qObject = Qt.BuildQObject("TestQObject*");
                qObject.Should().NotBeNull();
                qObject.Dispose();
            });
        }

        [Fact]
        public void Invalid_type_names_return_null_when_building_qobjects()
        {
            AssertQObject(_ =>
            {
                Qt.BuildQObject("sdfsdfsfsd").Should().BeNull();
                Qt.BuildQObject("int").Should().BeNull();
                Qt.BuildQObject("TestQObject").Should().BeNull(); // No * at end.
            });
        }

        private void AssertValue(INetQObject qObject, string method, object value)
        {   
            var raised = false;
            var handler = qObject.AttachSignal($"testSignal{method}", parameters =>
            {
                raised = true;
                parameters.Count.Should().Be(1);
                parameters[0].Should().Be(value);
            });
            using (handler)
            {
                var invokeResult = qObject.InvokeMethod($"testSlot{method}", value);
                invokeResult.Should().Be(value);
                raised.Should().BeTrue();
            }
        }
        
        private void AssertValue(INetQObject qObject, string method, object value, Action<object> assert)
        {
            var raised = false;
            var handler = qObject.AttachSignal($"testSignal{method}", parameters =>
            {
                raised = true;
                parameters.Count.Should().Be(1);
                assert(parameters[0]);
            });
            using (handler)
            {
                assert(qObject.InvokeMethod($"testSlot{method}", value));
                raised.Should().BeTrue();
            }
        }

        private void AssertQObject(Action<INetQObject> action)
        {
            Exception assertException = null;
            
            Mock.Setup(x => x.Method(It.IsAny<INetQObject>()))
                .Callback(new Action<dynamic>(x =>
                {
                    try
                    {
                        action(x as INetQObject);
                    }
                    catch (Exception ex)
                    {
                        assertException = ex;
                    }
                }));
            
            RunQmlTest(
                "test",
                @"
                    test.method(testQObject)
                ");

            Mock.Verify(x => x.Method(It.IsAny<INetQObject>()), Times.Once);
            if (assertException != null)
            {
                throw new Exception(assertException.Message, assertException);
            }
        }
    }
}