using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Qml.Net.Internal.Qml;
using Qml.Net.Internal.Types;
using Xunit;

namespace Qml.Net.Tests.CodeGen
{
    public class CodeGenPropertyTests : CodeGenBase<CodeGenPropertyTests.TestObject>
    {
        public class TestObject
        {
            public virtual int this[int index]
            {
                get => 0;
                set { }
            }

            public virtual bool Bool { get; set; }

            public virtual bool? BoolNullable { get; set; }

            public virtual char Char { get; set; }

            public virtual char? CharNullable { get; set; }

            public virtual int Int { get; set; }

            public virtual int? IntNullable { get; set; }

            public virtual uint UInt { get; set; }

            public virtual uint? UIntNullable { get; set; }

            public virtual long Long { get; set; }

            public virtual long? LongNullable { get; set; }

            public virtual ulong ULong { get; set; }

            public virtual ulong? ULongNullable { get; set; }

            public virtual float Float { get; set; }

            public virtual float? FloatNullable { get; set; }

            public virtual double Double { get; set; }

            public virtual double? DoubleNullable { get; set; }

            public virtual string String { get; set; }

            public virtual DateTimeOffset DateTime { get; set; }

            public virtual DateTimeOffset? DateTimeNullable { get; set; }

            public virtual object Obj { get; set; }

            public virtual RandomType ObjTyped { get; set; }

            public virtual RandomStruct Struct { get; set; }

            public virtual RandomStruct? StructNullable { get; set; }

            public virtual RandomEnum Enum { get; set; }

            public virtual RandomEnum? EnumNullable { get; set; }
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
        public void Can_use_indexer()
        {
            _mock.SetupGet(x => x[10]).Returns(20);

            var del = (Net.Internal.CodeGen.CodeGen.InvokeMethodDelegate)BuildReadPropertyDelegate("Item");

            using (var netReference = NetReference.CreateForObject(_mock.Object))
            {
                using (var result = new NetVariant())
                {
                    Task task = null;
                    del(netReference, NetVariantList.From(NetVariant.From(10)), result, ref task);
                    result.VariantType.Should().Be(NetVariantType.Int);
                    result.Int.Should().Be(20);
                }
            }

            _mock.Reset();
            _mock.SetupSet(x => x[10] = 20);

            del = (Net.Internal.CodeGen.CodeGen.InvokeMethodDelegate)BuildSetPropertyDelegate("Item");

            using (var netReference = NetReference.CreateForObject(_mock.Object))
            {
                using (var result = new NetVariant())
                {
                    Task task = null;
                    del(netReference, NetVariantList.From(NetVariant.From(10), NetVariant.From(20)), null, ref task);
                    _mock.VerifySet(x => x[10] = 20);
                }
            }
        }

        [Fact]
        public void Can_use_prop_bool()
        {
            TestGet(x => x.Bool, true, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Bool);
                result.Bool.Should().BeTrue();
            });
            TestGet(x => x.BoolNullable, true, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Bool);
                result.Bool.Should().BeTrue();
            });
            TestGet(x => x.BoolNullable, null, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Invalid);
            });

            TestSet(x => x.Bool, NetVariant.From(true), x => x.Bool = true);
            TestSet(x => x.BoolNullable, NetVariant.From(true), x => x.BoolNullable = true);
            TestSet(x => x.BoolNullable, new NetVariant(), x => x.BoolNullable = null);
        }

        [Fact]
        public void Can_use_prop_char()
        {
            TestGet(x => x.Char, 'C', result =>
            {
                result.VariantType.Should().Be(NetVariantType.Char);
                result.Char.Should().Be('C');
            });
            TestGet(x => x.CharNullable, 'C', result =>
            {
                result.VariantType.Should().Be(NetVariantType.Char);
                result.Char.Should().Be('C');
            });
            TestGet(x => x.CharNullable, (char?)null, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Invalid);
            });

            TestSet(x => x.Char, NetVariant.From('C'), x => x.Char = 'C');
            TestSet(x => x.CharNullable, NetVariant.From('C'), x => x.CharNullable = 'C');
            TestSet(x => x.CharNullable, new NetVariant(), x => x.CharNullable = null);
        }

        [Fact]
        public void Can_use_prop_int()
        {
            TestGet(x => x.Int, 20, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Int);
                result.Int.Should().Be(20);
            });
            TestGet(x => x.IntNullable, 20, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Int);
                result.Int.Should().Be(20);
            });
            TestGet(x => x.IntNullable, (char?)null, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Invalid);
            });

            TestSet(x => x.Int, NetVariant.From(20), x => x.Int = 20);
            TestSet(x => x.IntNullable, NetVariant.From(20), x => x.IntNullable = 20);
            TestSet(x => x.IntNullable, new NetVariant(), x => x.IntNullable = null);
        }

        [Fact]
        public void Can_use_prop_uint()
        {
            TestGet(x => x.UInt, (uint)20, result =>
            {
                result.VariantType.Should().Be(NetVariantType.UInt);
                result.UInt.Should().Be(20);
            });
            TestGet(x => x.UIntNullable, (uint)20, result =>
            {
                result.VariantType.Should().Be(NetVariantType.UInt);
                result.UInt.Should().Be(20);
            });
            TestGet(x => x.UIntNullable, null, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Invalid);
            });

            TestSet(x => x.UInt, NetVariant.From(20), x => x.UInt = 20);
            TestSet(x => x.UIntNullable, NetVariant.From((uint)20), x => x.UIntNullable = 20);
            TestSet(x => x.UIntNullable, new NetVariant(), x => x.UIntNullable = null);
        }

        [Fact]
        public void Can_use_prop_long()
        {
            TestGet(x => x.Long, 20, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Long);
                result.Long.Should().Be(20);
            });
            TestGet(x => x.LongNullable, (uint)20, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Long);
                result.Long.Should().Be(20);
            });
            TestGet(x => x.LongNullable, null, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Invalid);
            });

            TestSet(x => x.Long, NetVariant.From((long)20), x => x.Long = 20);
            TestSet(x => x.LongNullable, NetVariant.From((long)20), x => x.LongNullable = 20);
            TestSet(x => x.LongNullable, new NetVariant(), x => x.LongNullable = null);
        }

        [Fact]
        public void Can_use_prop_ulong()
        {
            TestGet(x => x.ULong, (ulong)20, result =>
            {
                result.VariantType.Should().Be(NetVariantType.ULong);
                result.ULong.Should().Be(20);
            });
            TestGet(x => x.ULongNullable, (uint)20, result =>
            {
                result.VariantType.Should().Be(NetVariantType.ULong);
                result.ULong.Should().Be(20);
            });
            TestGet(x => x.ULongNullable, null, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Invalid);
            });

            TestSet(x => x.ULong, NetVariant.From((ulong)20), x => x.ULong = 20);
            TestSet(x => x.ULongNullable, NetVariant.From((ulong)20), x => x.ULongNullable = 20);
            TestSet(x => x.ULongNullable, new NetVariant(), x => x.ULongNullable = null);
        }

        [Fact]
        public void Can_use_prop_float()
        {
            TestGet(x => x.Float, 20, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Float);
                result.Float.Should().Be(20);
            });
            TestGet(x => x.FloatNullable, 20, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Float);
                result.Float.Should().Be(20);
            });
            TestGet(x => x.FloatNullable, null, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Invalid);
            });

            TestSet(x => x.Float, NetVariant.From((float)20), x => x.Float = 20);
            TestSet(x => x.FloatNullable, NetVariant.From((float)20), x => x.FloatNullable = 20);
            TestSet(x => x.FloatNullable, new NetVariant(), x => x.FloatNullable = null);
        }

        [Fact]
        public void Can_use_prop_double()
        {
            TestGet(x => x.Double, 20, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Double);
                result.Double.Should().Be(20);
            });
            TestGet(x => x.DoubleNullable, 20, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Double);
                result.Double.Should().Be(20);
            });
            TestGet(x => x.DoubleNullable, null, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Invalid);
            });

            TestSet(x => x.Double, NetVariant.From((double)20), x => x.Double = 20);
            TestSet(x => x.DoubleNullable, NetVariant.From((double)20), x => x.DoubleNullable = 20);
            TestSet(x => x.DoubleNullable, new NetVariant(), x => x.DoubleNullable = null);
        }

        [Fact]
        public void Can_use_prop_string()
        {
            TestGet(x => x.String, "", result =>
            {
                result.VariantType.Should().Be(NetVariantType.String);
                result.String.Should().Be("");
            });
            TestGet(x => x.String, "test", result =>
            {
                result.VariantType.Should().Be(NetVariantType.String);
                result.String.Should().Be("test");
            });
            TestGet(x => x.String, null, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Invalid);
            });

            TestSet(x => x.String, NetVariant.From(""), x => x.String = "");
            TestSet(x => x.String, NetVariant.From("test"), x => x.String = "test");
            TestSet(x => x.String, NetVariant.From((string)null), x => x.String = null);
            TestSet(x => x.String, new NetVariant(), x => x.String = null);
        }

        [Fact]
        public void Can_use_prop_date_time()
        {
            var time = DateTimeOffset.Now;
            time = new DateTimeOffset(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second, time.Millisecond, time.Offset);
            TestGet(x => x.DateTime, time, result =>
            {
                result.VariantType.Should().Be(NetVariantType.DateTime);
                result.DateTime.Should().Be(time);
            });
            TestGet(x => x.DateTimeNullable, time, result =>
            {
                result.VariantType.Should().Be(NetVariantType.DateTime);
                result.DateTime.Should().Be(time);
            });
            TestGet(x => x.DateTimeNullable, null, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Invalid);
            });

            TestSet(x => x.DateTime, NetVariant.From(time), x => x.DateTime = time);
            TestSet(x => x.DateTimeNullable, NetVariant.From(time), x => x.DateTimeNullable = time);
            TestSet(x => x.DateTimeNullable, new NetVariant(), x => x.DateTimeNullable = null);
            TestSet(x => x.DateTimeNullable, NetVariant.From((DateTimeOffset?)null), x => x.DateTimeNullable = null);
        }

        [Fact]
        public void Can_use_prop_object()
        {
            var o = new object();
            TestGet(x => x.Obj, o, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Object);
                result.Instance.Instance.Should().BeSameAs(o);
            });
            TestGet(x => x.Obj, null, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Invalid);
            });

            TestSet(x => x.Obj, NetVariant.From((object)null), x => x.Obj = null);
            TestSet(x => x.Obj, NetVariant.From(o), x => x.Obj = o);
        }

        [Fact]
        public void Can_use_prop_typed_object()
        {
            var o = new RandomType();
            TestGet(x => x.ObjTyped, o, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Object);
                result.Instance.Instance.Should().BeSameAs(o);
            });
            TestGet(x => x.ObjTyped, null, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Invalid);
            });

            TestSet(x => x.ObjTyped, NetVariant.From((RandomType)null), x => x.ObjTyped = null);
            TestSet(x => x.ObjTyped, NetVariant.From(o), x => x.ObjTyped = o);
        }

        [Fact]
        public void Can_use_prop_struct()
        {
            var o = new RandomStruct();
            o.Value = 3;
            TestGet(x => x.Struct, o, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Object);
                result.Instance.Instance.Should().Be(o);
            });
            TestGet(x => x.StructNullable, o, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Object);
                result.Instance.Instance.Should().Be(o);
            });
            TestGet(x => x.StructNullable, null, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Invalid);
            });

            TestSet(x => x.Struct, NetVariant.From(o), x => x.Struct = o);
            TestSet(x => x.StructNullable, NetVariant.From(o), x => x.StructNullable = o);
            TestSet(x => x.StructNullable, new NetVariant(), x => x.StructNullable = null);
        }

        [Fact]
        public void Can_use_prop_enum()
        {
            TestGet(x => x.Enum, RandomEnum.Value2, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Int);
                result.Int.Should().Be((int)RandomEnum.Value2);
            });
            TestGet(x => x.EnumNullable, RandomEnum.Value2, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Int);
                result.Int.Should().Be((int)RandomEnum.Value2);
            });
            TestGet(x => x.EnumNullable, null, result =>
            {
                result.VariantType.Should().Be(NetVariantType.Invalid);
            });

            TestSet(x => x.Enum, NetVariant.From(RandomEnum.Value2), x => x.Enum = RandomEnum.Value2);
            TestSet(x => x.EnumNullable, NetVariant.From(RandomEnum.Value2), x => x.EnumNullable = RandomEnum.Value2);
            TestSet(x => x.EnumNullable, new NetVariant(), x => x.EnumNullable = null);
        }

        private void TestGet<TProperty>(Expression<Func<TestObject, TProperty>> expression, TProperty value, Action<NetVariant> assert)
        {
            _mock.Reset();
            _mock.SetupGet(expression).Returns(value);

            var del = (Net.Internal.CodeGen.CodeGen.InvokeMethodDelegate)BuildReadPropertyDelegate(((MemberExpression)expression.Body).Member.Name);

            using (var netReference = NetReference.CreateForObject(_mock.Object))
            {
                using (var result = new NetVariant())
                {
                    Task task = null;
                    del(netReference, null, result, ref task);
                    _mock.VerifyGet(expression);
                    assert(result);
                }
            }
        }

        private void TestSet<TProperty>(Expression<Func<TestObject, TProperty>> expression, NetVariant value, Action<TestObject> verify)
        {
            _mock.Reset();
            #pragma warning disable CS0618
            _mock.SetupSet(expression);
            #pragma warning restore CS0618

            var del = (Net.Internal.CodeGen.CodeGen.InvokeMethodDelegate)BuildSetPropertyDelegate(((MemberExpression)expression.Body).Member.Name);

            using (var netReference = NetReference.CreateForObject(_mock.Object))
            {
                using (var list = NetVariantList.From(value))
                {
                    Task task = null;
                    del(netReference, list, null, ref task);
                }

                _mock.VerifySet(verify, Times.Once);
            }
        }
    }
}