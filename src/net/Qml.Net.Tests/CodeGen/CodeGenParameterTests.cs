using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FluentAssertions;
using Moq;
using Qml.Net.Internal.Qml;
using Qml.Net.Internal.Types;
using Xunit;

namespace Qml.Net.Tests.CodeGen
{
    public class CodeGenParameterTests : CodeGenBase<CodeGenParameterTests.TestObject>
    {
        public class TestObject
        {
            public virtual void TestBool(bool param)
            {
            }

            public virtual void TestBoolNullable(bool? param)
            {
            }

            public virtual void TestChar(char param)
            {
            }

            public virtual void TestCharNullable(char? param)
            {
            }

            public virtual void TestInt(int param)
            {
            }

            public virtual void TestIntNullable(int? param)
            {
            }

            public virtual void TestUInt(uint param)
            {
            }

            public virtual void TestUIntNullable(uint? param)
            {
            }

            public virtual void TestLong(long param)
            {
            }

            public virtual void TestLongNullable(long? param)
            {
            }

            public virtual void TestULong(ulong param)
            {
            }

            public virtual void TestULongNullable(ulong? param)
            {
            }

            public virtual void TestFloat(float param)
            {
            }

            public virtual void TestFloatNullable(float? param)
            {
            }

            public virtual void TestDouble(double param)
            {
            }

            public virtual void TestDoubleNullable(double? param)
            {
            }

            public virtual void TestString(string param)
            {
            }

            public virtual void TestDateTime(DateTimeOffset param)
            {
            }

            public virtual void TestDateTimeNullable(DateTimeOffset? param)
            {
            }

            public virtual void TestObj(object param)
            {
            }

            public virtual void TestObjOfType(RandomType param)
            {
            }

            public virtual void TestStruct(RandomStruct param)
            {
            }

            public virtual void TestStructNullable(RandomStruct? param)
            {
            }

            public virtual void TestEnum(RandomEnum param)
            {
            }

            public virtual void TestEnumNullable(RandomEnum? param)
            {
            }

            public virtual void MultipleParams(int param1, long param2)
            {
            }

            public virtual int MultipleParamsWithReturn(int param1, long param2)
            {
                return 0;
            }
        }

        public class RandomType
        {
        }

        public struct RandomStruct
        {
            public int Value;
        }

        public enum RandomEnum
        {
            Value1,
            Value2
        }

        [Fact]
        public void Can_call_method_with_bool_parameter()
        {
            Test(x => x.TestBool(It.Is<bool>(v => v)), true);
            Test(x => x.TestBoolNullable(It.Is<bool?>(v => v == true)), true);
            Test(x => x.TestBoolNullable(It.Is<bool?>(v => v == null)), (bool?)null);
        }

        [Fact]
        public void Can_call_method_with_char_parameter()
        {
            Test(x => x.TestChar(It.Is<char>(v => v == 'C')), 'C');
            Test(x => x.TestCharNullable(It.Is<char?>(v => v == 'C')), 'C');
            Test(x => x.TestCharNullable(It.Is<char?>(v => v == null)), (char?)null);
        }

        [Fact]
        public void Can_call_method_with_int_parameter()
        {
            Test(x => x.TestInt(It.Is<int>(v => v == 20)), 20);
            Test(x => x.TestIntNullable(It.Is<int?>(v => v == 20)), 20);
            Test(x => x.TestIntNullable(It.Is<int?>(v => v == null)), (int?)null);
        }

        [Fact]
        public void Can_call_method_with_uint_parameter()
        {
            Test(x => x.TestUInt(It.Is<uint>(v => v == 20)), 20);
            Test(x => x.TestUIntNullable(It.Is<uint?>(v => v == 20)), 20);
            Test(x => x.TestUIntNullable(It.Is<uint?>(v => v == null)), (int?)null);
        }

        [Fact]
        public void Can_call_method_with_long_parameter()
        {
            Test(x => x.TestLong(It.Is<long>(v => v == 20)), 20);
            Test(x => x.TestLongNullable(It.Is<long?>(v => v == 20)), 20);
            Test(x => x.TestLongNullable(It.Is<long?>(v => v == null)), (long?)null);
        }

        [Fact]
        public void Can_call_method_with_ulong_parameter()
        {
            Test(x => x.TestULong(It.Is<ulong>(v => v == 20)), 20);
            Test(x => x.TestULongNullable(It.Is<ulong?>(v => v == 20)), 20);
            Test(x => x.TestULongNullable(It.Is<ulong?>(v => v == null)), (long?)null);
        }

        [Fact]
        public void Can_call_method_with_float_parameter()
        {
            Test(x => x.TestFloat(It.Is<float>(v => v == 20)), 20);
            Test(x => x.TestFloatNullable(It.Is<float?>(v => v == 20)), 20);
            Test(x => x.TestFloatNullable(It.Is<float?>(v => v == null)), (float?)null);
        }

        [Fact]
        public void Can_call_method_with_double_parameter()
        {
            Test(x => x.TestDouble(It.Is<double>(v => v == 20)), 20);
            Test(x => x.TestDoubleNullable(It.Is<double?>(v => v == 20)), 20);
            Test(x => x.TestDoubleNullable(It.Is<double?>(v => v == null)), (double?)null);
        }

        [Fact]
        public void Can_call_method_with_string_parameter()
        {
            Test(x => x.TestString(It.Is<string>(v => v == "")), "");
            Test(x => x.TestString(It.Is<string>(v => v == "value")), "value");
            Test(x => x.TestString(It.Is<string>(v => v == null)), (string)null);
        }

        [Fact]
        public void Can_call_method_with_date_time_parameter()
        {
            var time = DateTimeOffset.Now;
            time = new DateTimeOffset(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second, time.Millisecond, time.Offset);
            Test(x => x.TestDateTime(It.Is<DateTimeOffset>(v => v == time)), time);
            Test(x => x.TestDateTimeNullable(It.Is<DateTimeOffset?>(v => v == time)), time);
            Test(x => x.TestDateTimeNullable(It.Is<DateTimeOffset?>(v => v == null)), (DateTimeOffset?)null);
        }

        [Fact]
        public void Can_call_method_with_object_parameter()
        {
            var o = new object();
            Test(x => x.TestObj(It.Is<object>(v => v.Equals(o))), o);
        }

        [Fact]
        public void Can_call_method_with_typed_object_parameter()
        {
            var o = new RandomType();
            Test(x => x.TestObjOfType(It.Is<RandomType>(v => v.Equals(o))), o);
        }

        [Fact]
        public void Can_call_method_with_struct_parameter()
        {
            var o = new RandomStruct { Value = 2 };
            Test(x => x.TestStruct(It.Is<RandomStruct>(v => v.Equals(o))), o);
            Test(x => x.TestStructNullable(It.Is<RandomStruct>(v => v.Equals(o))), o);
            Test(x => x.TestStructNullable(It.Is<RandomStruct?>(v => v == null)), (RandomStruct?)null);
        }

        [Fact]
        public void Can_call_method_with_enum()
        {
            Test(x => x.TestEnum(It.Is<RandomEnum>(v => v == RandomEnum.Value2)), RandomEnum.Value2);
            Test(x => x.TestEnumNullable(It.Is<RandomEnum?>(v => v == RandomEnum.Value2)), RandomEnum.Value2);
            Test(x => x.TestEnumNullable(It.Is<RandomEnum?>(v => v == null)), (RandomEnum?)null);
        }

        [Fact]
        public void Can_call_method_with_multiple_parameters()
        {
            _mock.Setup(x => x.MultipleParams(20, 100));

            var del = (Net.Internal.CodeGen.CodeGen.InvokeMethodDelegate)BuildInvokeMethodDelegate(nameof(TestObject.MultipleParams));

            using (var netReference = NetReference.CreateForObject(_mock.Object))
            {
                del(netReference, NetVariantList.From(NetVariant.From(20), NetVariant.From<long>(100)), null);
                _mock.Verify(x => x.MultipleParams(20, 100));
            }
        }

        [Fact]
        public void Can_use_default_values_when_no_parameters_given()
        {
            _mock.Setup(x => x.MultipleParams(default(int), default(long)));

            var del = (Net.Internal.CodeGen.CodeGen.InvokeMethodDelegate)BuildInvokeMethodDelegate(nameof(TestObject.MultipleParams));

            using (var netReference = NetReference.CreateForObject(_mock.Object))
            {
                del(netReference, NetVariantList.From(), null);
                _mock.Verify(x => x.MultipleParams(default(int), default(long)));
            }
        }

        [Fact]
        public void Can_use_multiple_parameters_with_return_type()
        {
            _mock.Setup(x => x.MultipleParamsWithReturn(20, 30)).Returns(40);

            var del = (Net.Internal.CodeGen.CodeGen.InvokeMethodDelegate)BuildInvokeMethodDelegate(nameof(TestObject.MultipleParamsWithReturn));

            using (var netReference = NetReference.CreateForObject(_mock.Object))
            {
                using (var result = new NetVariant())
                {
                    del(netReference, NetVariantList.From(NetVariant.From<int>(20), NetVariant.From<long>(30)), result);
                    _mock.Verify(x => x.MultipleParamsWithReturn(20, 30), Times.Once);
                    result.VariantType.Should().Be(NetVariantType.Int);
                    result.Int.Should().Be(40);
                }
            }
        }

        private void Test<TResult>(Expression<Action<TestObject>> expression, TResult value)
        {
            _mock.Reset();
            _mock.Setup(expression);

            var del = (Net.Internal.CodeGen.CodeGen.InvokeMethodDelegate)BuildInvokeMethodDelegate(((MethodCallExpression)expression.Body).Method.Name);

            using (var netReference = NetReference.CreateForObject(_mock.Object))
            {
                using (var result = new NetVariant())
                {
                    del(netReference, NetVariantList.From(NetVariant.From(value)), result);
                    _mock.Verify(expression, Times.Once);
                }
            }
        }
    }
}