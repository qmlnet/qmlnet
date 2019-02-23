using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Castle.DynamicProxy.Internal;
using FluentAssertions;
using Moq;
using Qml.Net.Internal.Qml;
using Qml.Net.Internal.Types;
using Xunit;

namespace Qml.Net.Tests.CodeGen
{
    public class CodeGenReturnTypeTests : CodeGenBase<CodeGenReturnTypeTests.TestObject>
    {
        public class TestObject
        {
            public virtual bool ReturnTypeBool()
            {
                return false;
            }

            public virtual bool? ReturnTypeBoolNullable()
            {
                return false;
            }

            public virtual char ReturnTypeChar()
            {
                return '0';
            }

            public virtual char? ReturnTypeCharNullable()
            {
                return '0';
            }

            public virtual int ReturnTypeInt()
            {
                return 0;
            }

            public virtual int? ReturnTypeIntNullable()
            {
                return 0;
            }

            public virtual uint ReturnTypeUInt()
            {
                return 0;
            }

            public virtual uint? ReturnTypeUIntNullable()
            {
                return 0;
            }

            public virtual long ReturnTypeLong()
            {
                return 0;
            }

            public virtual long? ReturnTypeLongNullable()
            {
                return 0;
            }

            public virtual ulong ReturnTypeULong()
            {
                return 0;
            }

            public virtual ulong? ReturnTypeULongNullable()
            {
                return 0;
            }

            public virtual float ReturnTypeFloat()
            {
                return 0;
            }

            public virtual float? ReturnTypeFloatNullable()
            {
                return 0;
            }

            public virtual double ReturnTypeDouble()
            {
                return 0;
            }

            public virtual double? ReturnTypeDoubleNullable()
            {
                return 0;
            }

            public virtual string ReturnTypeString()
            {
                return "";
            }

            public virtual DateTimeOffset ReturnTypeDateTime()
            {
                return DateTimeOffset.Now;
            }

            public virtual DateTimeOffset? ReturnTypeDateTimeNullable()
            {
                return DateTimeOffset.Now;
            }

            public virtual object ReturnTypeObject()
            {
                return null;
            }

            public virtual RandomType ReturnTypeObjectTyped()
            {
                return null;
            }

            public virtual RandomStruct ReturnTypeStruct()
            {
                return new RandomStruct();
            }

            public virtual RandomStruct? ReturnTypeStructNullable()
            {
                return null;
            }

            public virtual Task ReturnTypeTask()
            {
                return null;
            }
        }

        public class RandomType
        {
        }

        public struct RandomStruct
        {
            public int Value1;
        }

        [Fact]
        public void Can_return_bool()
        {
            Test(x => x.ReturnTypeBool(), true, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Bool);
                result.Bool.Should().BeTrue();
            });
            Test(x => x.ReturnTypeBoolNullable(), true, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Bool);
                result.Bool.Should().BeTrue();
            });
            Test(x => x.ReturnTypeBoolNullable(), null, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Invalid);
            });
        }

        [Fact]
        public void Can_return_char()
        {
            Test(x => x.ReturnTypeChar(), 'C', result =>
            {
                result.VariantType.Should().Be(NetVariantType.Char);
                result.Char.Should().Be('C');
            });
            Test(x => x.ReturnTypeCharNullable(), 'C', result =>
            {
                result.VariantType.Should().Be(NetVariantType.Char);
                result.Char.Should().Be('C');
            });
            Test(x => x.ReturnTypeCharNullable(), null, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Invalid);
            });
        }

        [Fact]
        public void Can_return_int()
        {
            Test(x => x.ReturnTypeInt(), 20, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Int);
                result.Int.Should().Be(20);
            });
            Test(x => x.ReturnTypeIntNullable(), 20, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Int);
                result.Int.Should().Be(20);
            });
            Test(x => x.ReturnTypeIntNullable(), null, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Invalid);
            });
        }

        [Fact]
        public void Can_return_uint()
        {
            Test(x => x.ReturnTypeUInt(), (uint)20, result =>
            {
                result.VariantType.Should().Be(NetVariantType.UInt);
                result.UInt.Should().Be(20);
            });
            Test(x => x.ReturnTypeUIntNullable(), (uint)20, result =>
            {
                result.VariantType.Should().Be(NetVariantType.UInt);
                result.UInt.Should().Be(20);
            });
            Test(x => x.ReturnTypeUIntNullable(), null, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Invalid);
            });
        }

        [Fact]
        public void Can_return_long()
        {
            Test(x => x.ReturnTypeLong(), 20, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Long);
                result.Long.Should().Be(20);
            });
            Test(x => x.ReturnTypeLongNullable(), 20, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Long);
                result.Long.Should().Be(20);
            });
            Test(x => x.ReturnTypeLongNullable(), null, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Invalid);
            });
        }

        [Fact]
        public void Can_return_ulong()
        {
            Test(x => x.ReturnTypeULong(), (ulong)20, result =>
            {
                result.VariantType.Should().Be(NetVariantType.ULong);
                result.ULong.Should().Be(20);
            });
            Test(x => x.ReturnTypeULongNullable(), (ulong)20, result =>
            {
                result.VariantType.Should().Be(NetVariantType.ULong);
                result.ULong.Should().Be(20);
            });
            Test(x => x.ReturnTypeULongNullable(), null, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Invalid);
            });
        }

        [Fact]
        public void Can_return_float()
        {
            Test(x => x.ReturnTypeFloat(), 20, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Float);
                result.Float.Should().Be(20);
            });
            Test(x => x.ReturnTypeFloatNullable(), 20, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Float);
                result.Float.Should().Be(20);
            });
            Test(x => x.ReturnTypeFloatNullable(), null, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Invalid);
            });
        }

        [Fact]
        public void Can_return_double()
        {
            Test(x => x.ReturnTypeDouble(), 20, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Double);
                result.Double.Should().Be(20);
            });
            Test(x => x.ReturnTypeDoubleNullable(), 20, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Double);
                result.Double.Should().Be(20);
            });
            Test(x => x.ReturnTypeDoubleNullable(), null, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Invalid);
            });
        }

        [Fact]
        public void Can_return_string()
        {
            Test(x => x.ReturnTypeString(), "test", result =>
            {
                result.VariantType.Should().Be(NetVariantType.String);
                result.String.Should().Be("test");
            });
            Test(x => x.ReturnTypeString(), "", result =>
            {
                result.VariantType.Should().Be(NetVariantType.String);
                result.String.Should().Be("");
            });
            Test(x => x.ReturnTypeString(), null, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Invalid);
            });
        }

        [Fact]
        public void Can_return_date_time()
        {
            var time = DateTimeOffset.Now;
            time = new DateTimeOffset(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second, time.Millisecond, time.Offset);
            Test(x => x.ReturnTypeDateTime(), time, result =>
            {
                result.VariantType.Should().Be(NetVariantType.DateTime);
                result.DateTime.Should().Be((time));
            });
            Test(x => x.ReturnTypeDateTimeNullable(), time, result =>
            {
                result.VariantType.Should().Be(NetVariantType.DateTime);
                result.DateTime.Should().Be(time);
            });
            Test(x => x.ReturnTypeDateTimeNullable(), null, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Invalid);
            });
        }

        [Fact]
        public void Can_return_object()
        {
            var o = new object();
            Test(x => x.ReturnTypeObject(), o, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Object);
                result.Instance.Instance.Should().BeSameAs(o);
            });
            Test(x => x.ReturnTypeObject(), null, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Invalid);
            });
        }

        [Fact]
        public void Can_return_typed_object()
        {
            var o = new RandomType();
            Test(x => x.ReturnTypeObjectTyped(), o, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Object);
                result.Instance.Instance.Should().BeSameAs(o);
            });
            Test(x => x.ReturnTypeObjectTyped(), null, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Invalid);
            });
        }

        [Fact]
        public void Can_return_struct()
        {
            var value = new RandomStruct();
            Test(x => x.ReturnTypeStruct(), value, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Object);
                result.Instance.Instance.Should().Be(value);
            });
            Test(x => x.ReturnTypeStructNullable(), value, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Object);
                result.Instance.Instance.Should().Be(value);
            });
            Test(x => x.ReturnTypeStructNullable(), null, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Invalid);
            });
        }

        [Fact]
        public void Can_get_instance_of_task_if_return_type_task()
        {
            var del = (Net.Internal.CodeGen.CodeGen.InvokeMethodDelegate)BuildInvokeMethodDelegate(nameof(TestObject.ReturnTypeTask));
            var task = Task.CompletedTask;
            _mock.Setup(x => x.ReturnTypeTask()).Returns(task);

            Task outputTask = null;
            del(NetReference.CreateForObject(_mock.Object), NetVariantList.From(), new NetVariant(), ref outputTask);

            outputTask.Should().Be(task);
        }

        private void Test<TResult>(Expression<Func<TestObject, TResult>> expression, TResult value, Action<NetVariant> assert)
        {
            _mock.Reset();
            _mock.Setup(expression).Returns(value);

            var del = (Net.Internal.CodeGen.CodeGen.InvokeMethodDelegate)BuildInvokeMethodDelegate(((MethodCallExpression)expression.Body).Method.Name);

            using (var netReference = NetReference.CreateForObject(_mock.Object))
            {
                using (var result = new NetVariant())
                {
                    Task task = null;
                    del(netReference, NetVariantList.From(), result, ref task);
                    _mock.Verify(expression, Times.Once);
                    assert(result);
                }
            }
        }
    }
}